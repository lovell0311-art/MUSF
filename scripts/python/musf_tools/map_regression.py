from __future__ import annotations

import re
import time
from pathlib import Path
from typing import Any, Iterable

from .common import musf_root, timestamp_iso, write_json


DEFAULT_REQUIRED_SCENES = (
    "勇者大陆",
    "仙踪林",
    "冰风谷",
    "幽暗森林",
    "古战场",
)

SCENE_NAME_BY_ID = {
    1: "勇者大陆",
    2: "仙踪林",
    4: "冰风谷",
    9: "幽暗森林",
    102: "古战场",
    112: "古战场",
}

SCENE_NAME_BY_MINIMAP = {
    "YongZheDaLu": "勇者大陆",
    "XianZongLin": "仙踪林",
    "BingFengGu": "冰风谷",
    "YouAnSenLin": "幽暗森林",
    "GuZhanChang": "古战场",
    "GuZhanChang2": "古战场",
}

SCENE_NAME_BY_RAW_NAME = {
    "勇者大陆": "勇者大陆",
    "仙踪林": "仙踪林",
    "冰风谷": "冰风谷",
    "幽暗森林": "幽暗森林",
    "古战场": "古战场",
    "古战场_1": "古战场",
    "古战场2": "古战场",
}

SWITCH_MINIMAP_RE = re.compile(
    r"SwitchMiniMap sceneId:(?P<sceneId>-?\d+)\s+sceneName:(?P<sceneName>\S+)\s+minimap:(?P<minimap>\S+)\s+offset:\((?P<offsetX>-?\d+),(?P<offsetY>-?\d+)\)\s+scale:(?P<scale>[0-9.]+)"
)


def normalize_scene_name(scene_id: int, raw_scene_name: str, minimap_name: str) -> str:
    return (
        SCENE_NAME_BY_ID.get(scene_id)
        or SCENE_NAME_BY_MINIMAP.get(minimap_name)
        or SCENE_NAME_BY_RAW_NAME.get(raw_scene_name)
        or raw_scene_name
    )


def default_release_runs_root() -> Path:
    return musf_root() / "reports" / "releases" / "runs"


def latest_release_run_dirs(*, limit: int = 30) -> list[Path]:
    runs_root = default_release_runs_root()
    if not runs_root.exists():
        return []
    directories = [path for path in runs_root.iterdir() if path.is_dir()]
    directories.sort(key=lambda item: item.stat().st_mtime, reverse=True)
    return directories[:limit]


def latest_screenshot_for_run(run_dir: Path) -> str:
    screens_dir = run_dir / "screens"
    if not screens_dir.exists():
        return ""
    screenshots = [path for path in screens_dir.rglob("*.png") if path.is_file()]
    if not screenshots:
        return ""
    screenshots.sort(key=lambda item: item.stat().st_mtime, reverse=True)
    return str(screenshots[0])


def parse_switch_minimap_entries(log_path: Path) -> list[dict[str, Any]]:
    if not log_path.exists():
        return []

    entries: list[dict[str, Any]] = []
    run_dir = log_path.parent
    screenshot_path = latest_screenshot_for_run(run_dir)
    for line_number, raw_line in enumerate(log_path.read_text(encoding="utf-8", errors="ignore").splitlines(), start=1):
        match = SWITCH_MINIMAP_RE.search(raw_line)
        if not match:
            continue
        scene_id = int(match.group("sceneId"))
        raw_scene_name = match.group("sceneName")
        minimap_name = match.group("minimap")
        entries.append(
            {
                "sceneId": scene_id,
                "sceneName": normalize_scene_name(scene_id, raw_scene_name, minimap_name),
                "rawSceneName": raw_scene_name,
                "minimapName": minimap_name,
                "offset": {
                    "x": int(match.group("offsetX")),
                    "y": int(match.group("offsetY")),
                },
                "scale": float(match.group("scale")),
                "logPath": str(log_path),
                "runDir": str(run_dir),
                "screenshotPath": screenshot_path,
                "lineNumber": line_number,
                "rawLine": raw_line.strip(),
            }
        )
    return entries


def extract_latest_scene_entry(log_path: Path) -> dict[str, Any] | None:
    entries = parse_switch_minimap_entries(log_path)
    if not entries:
        return None
    return entries[-1]


def collect_log_paths(run_dirs: Iterable[str] | None = None, log_paths: Iterable[str] | None = None) -> list[Path]:
    resolved: list[Path] = []
    seen: set[str] = set()

    for value in log_paths or []:
        path = Path(value)
        normalized = str(path.resolve(strict=False)).lower()
        if normalized not in seen:
            seen.add(normalized)
            resolved.append(path)

    for value in run_dirs or []:
        run_dir = Path(value)
        log_path = run_dir / "logcat.txt"
        normalized = str(log_path.resolve(strict=False)).lower()
        if normalized not in seen:
            seen.add(normalized)
            resolved.append(log_path)

    if resolved:
        return resolved

    for run_dir in latest_release_run_dirs():
        log_path = run_dir / "logcat.txt"
        if not log_path.exists():
            continue
        normalized = str(log_path.resolve(strict=False)).lower()
        if normalized not in seen:
            seen.add(normalized)
            resolved.append(log_path)
    return resolved


def map_regression(
    profile_name: str,
    *,
    run_dirs: Iterable[str] | None = None,
    log_paths: Iterable[str] | None = None,
    required_scenes: Iterable[str] | None = None,
    character_name: str = "",
    zone_id: int = 1,
    server_id: int = 1,
) -> Path:
    required = [str(scene) for scene in (required_scenes or DEFAULT_REQUIRED_SCENES)]
    selected_logs = collect_log_paths(run_dirs=run_dirs, log_paths=log_paths)

    all_entries: list[dict[str, Any]] = []
    for log_path in selected_logs:
        all_entries.extend(parse_switch_minimap_entries(log_path))

    latest_by_scene: dict[str, dict[str, Any]] = {}
    for entry in all_entries:
        latest_by_scene[entry["sceneName"]] = entry

    coverage: list[dict[str, Any]] = []
    for scene_name in required:
        entry = latest_by_scene.get(scene_name)
        if entry is None:
            coverage.append(
                {
                    "sceneName": scene_name,
                    "status": "fail",
                    "details": "No SwitchMiniMap evidence found in the selected log set.",
                    "evidence": None,
                }
            )
            continue
        coverage.append(
            {
                "sceneName": scene_name,
                "status": "pass",
                "details": f"{entry['minimapName']} offset=({entry['offset']['x']},{entry['offset']['y']}) scale={entry['scale']}",
                "evidence": entry,
            }
        )

    overall = "pass" if coverage and all(item["status"] == "pass" for item in coverage) else "fail"
    report = {
        "generatedAt": timestamp_iso(),
        "profile": profile_name,
        "characterName": character_name,
        "zoneId": zone_id,
        "serverId": server_id,
        "overall": overall,
        "requiredScenes": required,
        "selectedLogs": [str(path) for path in selected_logs],
        "sceneCount": len({entry["sceneName"] for entry in all_entries}),
        "coverage": coverage,
        "latestEntry": all_entries[-1] if all_entries else None,
        "notes": [
            "This report currently validates map/minimap runtime evidence from logcat SwitchMiniMap lines.",
            "Scene names are normalized through sceneId and minimap aliases before coverage evaluation.",
            "Big-map screenshots, NPC markers, and transport marker assertions are not yet automated in this control plane.",
        ],
    }

    output = musf_root() / "reports" / "releases" / f"map-regression-{time.strftime('%Y%m%d-%H%M%S')}.json"
    write_json(output, report)
    return output
