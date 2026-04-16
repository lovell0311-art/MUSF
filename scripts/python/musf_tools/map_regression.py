from __future__ import annotations

import re
import time
from pathlib import Path
from typing import Any, Iterable

from .common import musf_root, timestamp_iso, write_json
from .webgm_client import WebGMError, ensure_session, game_status, player_search, role_data


DEFAULT_REQUIRED_SCENES = (
    "\u52c7\u8005\u5927\u9646",
    "\u4ed9\u8e2a\u6797",
    "\u51b0\u98ce\u8c37",
    "\u5e7d\u6697\u68ee\u6797",
    "\u53e4\u6218\u573a",
)

SCENE_NAME_BY_ID = {
    1: "\u52c7\u8005\u5927\u9646",
    2: "\u4ed9\u8e2a\u6797",
    4: "\u51b0\u98ce\u8c37",
    9: "\u5e7d\u6697\u68ee\u6797",
    102: "\u53e4\u6218\u573a",
    112: "\u53e4\u6218\u573a",
}

SCENE_NAME_BY_MINIMAP = {
    "YongZheDaLu": "\u52c7\u8005\u5927\u9646",
    "XianZongLin": "\u4ed9\u8e2a\u6797",
    "BingFengGu": "\u51b0\u98ce\u8c37",
    "YouAnSenLin": "\u5e7d\u6697\u68ee\u6797",
    "GuZhanChang": "\u53e4\u6218\u573a",
    "GuZhanChang2": "\u53e4\u6218\u573a",
}

MOVE_START_RE = re.compile(
    r"MoveToAsync\s+start\s+from=(?P<fromX>-?\d+(?:\.\d+)?),(?P<fromY>-?\d+(?:\.\d+)?),(?P<fromZ>-?\d+(?:\.\d+)?)\s+to=(?P<toX>-?\d+(?:\.\d+)?),(?P<toY>-?\d+(?:\.\d+)?),(?P<toZ>-?\d+(?:\.\d+)?)",
    re.IGNORECASE,
)
MOVE_END_RE = re.compile(
    r"MoveToAsync\s+(?P<phase>complete|stop)\s+at=(?P<x>-?\d+(?:\.\d+)?),(?P<y>-?\d+(?:\.\d+)?),(?P<z>-?\d+(?:\.\d+)?)",
    re.IGNORECASE,
)
SWITCH_MINIMAP_RE = re.compile(
    r"SwitchMiniMap sceneId:(?P<sceneId>-?\d+)\s+sceneName:(?P<sceneName>\S+)\s+minimap:(?P<minimap>\S+)\s+offset:\((?P<offsetX>-?\d+),(?P<offsetY>-?\d+)\)\s+scale:(?P<scale>[0-9.]+)"
)


def normalize_scene_name(scene_id: int, raw_scene_name: str, minimap_name: str) -> str:
    return SCENE_NAME_BY_ID.get(scene_id) or SCENE_NAME_BY_MINIMAP.get(minimap_name) or raw_scene_name


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


def parse_move_entries(log_path: Path) -> list[dict[str, Any]]:
    if not log_path.exists():
        return []

    entries: list[dict[str, Any]] = []
    run_dir = log_path.parent
    screenshot_path = latest_screenshot_for_run(run_dir)
    scene_markers = parse_switch_minimap_entries(log_path)
    marker_index = 0
    active_marker: dict[str, Any] | None = None
    lines = log_path.read_text(encoding="utf-8", errors="ignore").splitlines()
    for line_number, raw_line in enumerate(lines, start=1):
        while marker_index < len(scene_markers) and int(scene_markers[marker_index]["lineNumber"]) <= line_number:
            active_marker = scene_markers[marker_index]
            marker_index += 1

        start_match = MOVE_START_RE.search(raw_line)
        if start_match:
            entries.append(
                {
                    "phase": "start",
                    "sceneId": active_marker["sceneId"] if active_marker else None,
                    "sceneName": active_marker["sceneName"] if active_marker else "",
                    "minimapName": active_marker["minimapName"] if active_marker else "",
                    "from": {
                        "x": float(start_match.group("fromX")),
                        "y": float(start_match.group("fromY")),
                        "z": float(start_match.group("fromZ")),
                    },
                    "to": {
                        "x": float(start_match.group("toX")),
                        "y": float(start_match.group("toY")),
                        "z": float(start_match.group("toZ")),
                    },
                    "mapPosition": {
                        "x": float(start_match.group("toX")),
                        "y": float(start_match.group("toZ")),
                    },
                    "logPath": str(log_path),
                    "runDir": str(run_dir),
                    "screenshotPath": screenshot_path,
                    "lineNumber": line_number,
                    "rawLine": raw_line.strip(),
                }
            )
            continue

        end_match = MOVE_END_RE.search(raw_line)
        if not end_match:
            continue
        entries.append(
            {
                "phase": end_match.group("phase").lower(),
                "sceneId": active_marker["sceneId"] if active_marker else None,
                "sceneName": active_marker["sceneName"] if active_marker else "",
                "minimapName": active_marker["minimapName"] if active_marker else "",
                "at": {
                    "x": float(end_match.group("x")),
                    "y": float(end_match.group("y")),
                    "z": float(end_match.group("z")),
                },
                "mapPosition": {
                    "x": float(end_match.group("x")),
                    "y": float(end_match.group("z")),
                },
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


def collect_webgm_evidence(profile_name: str, *, character_name: str, zone_id: int, server_id: int) -> dict[str, Any]:
    if not character_name.strip():
        return {"enabled": False, "success": False, "notes": ["No character name provided; WebGM role lookup skipped."]}

    evidence: dict[str, Any] = {
        "enabled": True,
        "success": False,
        "characterName": character_name,
        "zoneId": zone_id,
        "serverId": server_id,
    }
    try:
        token = ensure_session(profile_name)
        server_state = game_status(profile_name, token, server_id=server_id)
        roles = player_search(profile_name, token, zone_id=zone_id, role_name=character_name, limit=1)
        if not roles:
            evidence["notes"] = [f"No role matched character name '{character_name}'."]
            return evidence
        role_summary = roles[0]
        detailed = role_data(profile_name, token, zone_id=zone_id, game_user_id=str(role_summary["GameUserId"]))
        unit_data = dict(detailed.get("UnitData") or {})
        live_scene_name = SCENE_NAME_BY_ID.get(int(unit_data.get("MapId", 0)), str(unit_data.get("MapId", "")))
        evidence.update(
            {
                "success": True,
                "serverStatus": server_state,
                "roleSummary": role_summary,
                "roleData": detailed,
                "liveSceneName": live_scene_name,
            }
        )
        return evidence
    except WebGMError as exc:
        evidence["notes"] = [str(exc)]
        return evidence


def recent_movement(entries: list[dict[str, Any]], *, limit: int = 12) -> list[dict[str, Any]]:
    if len(entries) <= limit:
        return entries
    return entries[-limit:]


def log_sort_key(path: Path) -> tuple[float, str]:
    try:
        return (path.stat().st_mtime, str(path).lower())
    except OSError:
        return (0.0, str(path).lower())


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
    selected_logs = sorted(collect_log_paths(run_dirs=run_dirs, log_paths=log_paths), key=log_sort_key)

    minimap_entries: list[dict[str, Any]] = []
    move_entries: list[dict[str, Any]] = []
    for log_path in selected_logs:
        minimap_entries.extend(parse_switch_minimap_entries(log_path))
        move_entries.extend(parse_move_entries(log_path))

    latest_by_scene: dict[str, dict[str, Any]] = {}
    for entry in minimap_entries:
        latest_by_scene[entry["sceneName"]] = entry

    webgm = collect_webgm_evidence(profile_name, character_name=character_name, zone_id=zone_id, server_id=server_id)

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

    current_role_snapshot = None
    if webgm.get("success"):
        role_data_payload = dict(webgm.get("roleData") or {})
        unit_data = dict(role_data_payload.get("UnitData") or {})
        live_scene_name = str(webgm.get("liveSceneName") or "")
        current_role_snapshot = {
            "sceneName": live_scene_name,
            "mapId": unit_data.get("MapId"),
            "x": unit_data.get("X"),
            "y": unit_data.get("Y"),
            "angle": unit_data.get("Angle"),
            "matchingLogEvidence": latest_by_scene.get(live_scene_name),
        }

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
        "sceneCount": len({entry["sceneName"] for entry in minimap_entries}),
        "coverage": coverage,
        "latestEntry": minimap_entries[-1] if minimap_entries else None,
        "recentMovement": recent_movement(move_entries),
        "currentRoleSnapshot": current_role_snapshot,
        "webgm": webgm,
        "notes": [
            "This report validates map/minimap runtime evidence from logcat SwitchMiniMap lines.",
            "When characterName is provided, it also pulls read-only WebGM game_status, search, and role_data evidence.",
            "Big-map screenshots, NPC markers, and transport marker assertions are not yet automated in this control plane.",
        ],
    }

    output = musf_root() / "reports" / "releases" / f"map-regression-{time.strftime('%Y%m%d-%H%M%S')}.json"
    write_json(output, report)
    return output
