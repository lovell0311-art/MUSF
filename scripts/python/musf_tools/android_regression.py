from __future__ import annotations

import json
import subprocess
import sys
import time
from pathlib import Path
from typing import Any

from .common import adb, adb_path, musf_root, read_json, timestamp_iso, write_json
from .publish import (
    adb_lines,
    capture_device_screenshot,
    image_region_metrics,
    load_png_pixels,
    push_file_to_remote,
    remote_files_dir,
    remote_owner_group,
)


DEFAULT_ACCOUNT = "blaike"
DEFAULT_PASSWORD = "123456"
EXPECTED_UI_NAME = "破军之刃"
HOTFIX_FOLDER_NAME = EXPECTED_UI_NAME
DISALLOWED_UI_MARKERS = ("命运王座", "mingyun", "mingyunwangzuo")
REFERENCE_WIDTH = 1920
REFERENCE_HEIGHT = 1080
START_BUTTON_POINTS = ((1768, 930), (1768, 962), (1768, 995))
SHIP_ENTER_GAME_POINT = (960, 980)
AGE_PROMPT_CLOSE_POINT = (1588, 119)
ACCOUNT_FIELD_POINT = (1208, 296)
PASSWORD_FIELD_POINT = (960, 426)
LOGIN_BUTTON_POINT = (959, 561)
KEYBOARD_CONFIRM_POINT = (1780, 1000)
SERVER_GROUP_POINT = (365, 172)
SERVER_LINE_POINTS = ((676, 141), (706, 142), (676, 200))
SERVER_ENTER_POINTS = ((960, 1000), (960, 1032), (960, 1060))
ROLE_CHARACTER_POINTS = ((960, 500), (925, 520), (995, 520))
BAG_BUTTON_POINT = (1698, 953)
SKILLS_BUTTON_POINT = (1709, 806)
CLOSE_PANEL_POINT = (1873, 41)
def canary_state_dir() -> Path:
    return musf_root() / "canary-state" / "login-stack"


def log_progress(message: str) -> None:
    line = f"[android-regression] {message}\n"
    try:
        sys.stdout.write(line)
        sys.stdout.flush()
        return
    except (UnicodeEncodeError, OSError):
        pass

    encoding = sys.stdout.encoding or "utf-8"
    safe_line = line.encode(encoding, errors="replace")
    buffer = getattr(sys.stdout, "buffer", None)
    if buffer is not None:
        try:
            buffer.write(safe_line)
            buffer.flush()
            return
        except OSError:
            pass

    sys.__stdout__.write(safe_line.decode(encoding, errors="replace"))
    sys.__stdout__.flush()


def login_stack_manifest() -> dict[str, Any]:
    return read_json(musf_root() / "configs" / "login-stack-manifest.json")


def ui_family_policy() -> dict[str, Any]:
    manifest = login_stack_manifest()
    expected_name = str(manifest.get("UiBaselineName") or EXPECTED_UI_NAME)
    rejected = tuple(manifest.get("RejectedUiFamilies") or list(DISALLOWED_UI_MARKERS))
    return {
        "expectedUiName": expected_name,
        "rejectedUiFamilies": [str(item) for item in rejected],
        "visualReferencePath": str(manifest["VisualSmoke"]["ReferenceScreenshotPath"]),
    }


def reference_images() -> dict[str, Path]:
    root = musf_root()
    return {
        "login_screen": root / "reports" / "references" / "pojun-login-baseline.png",
        "role_select": root / "reports" / "references" / "pojun-role-select-baseline.png",
        "main_hud": root / "reports" / "references" / "pojun-main-hud-baseline.png",
        "bag": root / "reports" / "references" / "pojun-bag-baseline.png",
    }


def ensure_reference_images() -> None:
    policy = ui_family_policy()
    for name, path in reference_images().items():
        if not path.exists():
            raise FileNotFoundError(f"Regression reference image missing for {name}: {path}")
        lowered = path.name.lower()
        for marker in policy["rejectedUiFamilies"]:
            if marker and marker.lower() in lowered:
                raise RuntimeError(f"Disallowed UI reference marker '{marker}' found in active reference: {path}")

    visual_reference = Path(policy["visualReferencePath"])
    if not visual_reference.exists():
        raise FileNotFoundError(f"Login-stack visual reference missing: {visual_reference}")
    lowered = visual_reference.name.lower()
    for marker in policy["rejectedUiFamilies"]:
        if marker and marker.lower() in lowered:
            raise RuntimeError(f"Disallowed UI reference marker '{marker}' found in login-stack manifest: {visual_reference}")


def ensure_canary_account_state(account: str = DEFAULT_ACCOUNT, password: str = DEFAULT_PASSWORD) -> Path:
    state_dir = canary_state_dir()
    state_dir.mkdir(parents=True, exist_ok=True)
    path = state_dir / "LocalJsonData_UserAccount.json"
    path.write_text(
        json.dumps({"account": account, "password": password, "IsRead": True}, ensure_ascii=False, separators=(",", ":")),
        encoding="utf-8",
    )
    return path


def remote_file_exists(serial: str, remote_path: str) -> bool:
    result = adb(["-s", serial, "shell", "ls", "-1", remote_path], check=False)
    output = "\n".join(part for part in ((result.stdout or "").strip(), (result.stderr or "").strip()) if part).strip()
    if not output:
        return False
    lowered = output.lower()
    if "no such file" in lowered or "cannot access" in lowered:
        return False
    return True


def pull_remote_file_if_exists(serial: str, remote_path: str, output_path: Path) -> bool:
    if not remote_file_exists(serial, remote_path):
        return False
    output_path.parent.mkdir(parents=True, exist_ok=True)
    adb_lines(["-s", serial, "pull", remote_path, str(output_path)])
    return True


def resolve_remote_hotfix_dir(serial: str, package_name: str) -> str:
    files_dir = remote_files_dir(package_name)
    android_dir = f"{files_dir}/Android"
    entries = adb_lines(["-s", serial, "shell", "ls", "-1", android_dir], check=False)
    candidates = [entry.strip() for entry in entries if entry.strip() and not entry.lower().startswith("ls:")]
    preferred = f"{android_dir}/{HOTFIX_FOLDER_NAME}"
    if remote_file_exists(serial, f"{preferred}/Version.txt") or remote_file_exists(serial, f"{preferred}/code.unity3d"):
        return preferred

    for candidate in candidates:
        remote_dir = f"{android_dir}/{candidate}"
        if remote_file_exists(serial, f"{remote_dir}/Version.txt") or remote_file_exists(serial, f"{remote_dir}/code.unity3d"):
            return remote_dir

    return preferred


def backup_device_state(serial: str, package_name: str, run_dir: Path) -> dict[str, Any]:
    backup_dir = run_dir / "device_backup"
    backup_dir.mkdir(parents=True, exist_ok=True)

    files_dir = remote_files_dir(package_name)
    hotfix_dir = resolve_remote_hotfix_dir(serial, package_name)
    package_path = adb(["-s", serial, "shell", "pm", "path", package_name], check=False)
    file_listing = adb(["-s", serial, "shell", "ls", "-1", files_dir], check=False)
    hotfix_listing = adb(["-s", serial, "shell", "ls", "-1", hotfix_dir], check=False)

    pulled_files: list[str] = []
    for name in ("LocalJsonData_UserAccount.json", "LocalJsonData_LastRoleInfo.json"):
        remote_path = f"{files_dir}/{name}"
        local_path = backup_dir / name
        if pull_remote_file_if_exists(serial, remote_path, local_path):
            pulled_files.append(str(local_path))

    snapshot = {
        "generatedAt": timestamp_iso(),
        "serial": serial,
        "packageName": package_name,
        "packagePath": (package_path.stdout or package_path.stderr or "").strip(),
        "filesDir": files_dir,
        "hotfixDir": hotfix_dir,
        "filesListing": (file_listing.stdout or file_listing.stderr or "").splitlines(),
        "hotfixListing": (hotfix_listing.stdout or hotfix_listing.stderr or "").splitlines(),
        "pulledFiles": pulled_files,
    }
    snapshot_path = backup_dir / "snapshot.json"
    write_json(snapshot_path, snapshot)
    snapshot["snapshotPath"] = str(snapshot_path)
    return snapshot


def prepare_regression_login_state(serial: str, package_name: str) -> dict[str, Any]:
    state_dir = canary_state_dir()
    account_path = ensure_canary_account_state()
    last_role_path = state_dir / "LocalJsonData_LastRoleInfo.json"
    files_dir = remote_files_dir(package_name)
    adb_lines(["-s", serial, "shell", "mkdir", "-p", files_dir])
    owner_group = remote_owner_group(serial, files_dir)

    pushed: list[str] = []
    for local_path in (account_path, last_role_path):
        if not local_path.exists():
            continue
        remote_path = f"{files_dir}/{local_path.name}"
        push_file_to_remote(serial, local_path, remote_path)
        adb_lines(
            [
                "-s",
                serial,
                "shell",
                f"su 0 sh -c 'chown {owner_group} \"{remote_path}\" && chmod 660 \"{remote_path}\"'",
            ],
            check=False,
        )
        pushed.append(remote_path)

    return {
        "generatedAt": timestamp_iso(),
        "serial": serial,
        "packageName": package_name,
        "filesDir": files_dir,
        "pushed": pushed,
        "localAccountStatePath": str(account_path),
        "localLastRolePath": str(last_role_path),
    }


def start_logcat_capture(serial: str, output_path: Path) -> tuple[subprocess.Popen[str], Any]:
    adb(["-s", serial, "logcat", "-c"], check=False)
    output_path.parent.mkdir(parents=True, exist_ok=True)
    handle = output_path.open("w", encoding="utf-8")
    proc = subprocess.Popen(
        [str(adb_path()), "-s", serial, "logcat", "-v", "time"],
        stdout=handle,
        stderr=subprocess.STDOUT,
        text=True,
    )
    return proc, handle


def stop_logcat_capture(proc: subprocess.Popen[str], handle: Any) -> None:
    if proc.poll() is None:
        proc.terminate()
        try:
            proc.wait(timeout=5)
        except subprocess.TimeoutExpired:
            proc.kill()
            proc.wait(timeout=5)
    handle.close()


def logcat_fatal_summary(path: Path) -> dict[str, Any]:
    if not path.exists():
        return {"ok": False, "matches": ["logcat not found"]}
    matches: list[str] = []
    for line in path.read_text(encoding="utf-8", errors="ignore").splitlines():
        normalized = line.upper()
        if "FATAL EXCEPTION" in normalized or "FATAL SIGNAL" in normalized or "UNABLE TO INITIALIZE THE UNITY ENGINE" in normalized:
            matches.append(line.strip())
            continue
        if "ANDROIDRUNTIME" in normalized and "EXCEPTION" in normalized:
            matches.append(line.strip())
            continue
        if " CRASH " in normalized or normalized.endswith(" CRASH"):
            matches.append(line.strip())
    return {"ok": not matches, "matches": matches[:20]}


def scale_point(point: tuple[int, int], screenshot_path: Path) -> tuple[int, int]:
    width, height, _, _ = load_png_pixels(screenshot_path)
    x = round(point[0] * width / REFERENCE_WIDTH)
    y = round(point[1] * height / REFERENCE_HEIGHT)
    return x, y


def tap_point(serial: str, point: tuple[int, int], screenshot_path: Path) -> tuple[int, int]:
    x, y = scale_point(point, screenshot_path)
    adb_lines(["-s", serial, "shell", "input", "tap", str(x), str(y)])
    return x, y


def tap_points(serial: str, points: tuple[tuple[int, int], ...], screenshot_path: Path, *, interval_seconds: float = 0.5) -> list[tuple[int, int]]:
    taps: list[tuple[int, int]] = []
    for index, point in enumerate(points):
        taps.append(tap_point(serial, point, screenshot_path))
        if index < len(points) - 1:
            time.sleep(interval_seconds)
    return taps


def capture_named_screenshot(serial: str, output_dir: Path, name: str) -> Path:
    output_dir.mkdir(parents=True, exist_ok=True)
    path = output_dir / name
    return capture_device_screenshot(serial, path)


def capture_state_once(serial: str, output_dir: Path, name: str, *, delay_seconds: int = 0) -> dict[str, Any]:
    if delay_seconds > 0:
        time.sleep(delay_seconds)
    screenshot = capture_named_screenshot(serial, output_dir, name)
    detection = detect_screen_state(screenshot)
    detection["screenshotPath"] = str(screenshot)
    return detection


def capture_transition_state_once(serial: str, output_dir: Path, name: str, *, delay_seconds: int = 0) -> dict[str, Any]:
    if delay_seconds > 0:
        time.sleep(delay_seconds)
    screenshot = capture_named_screenshot(serial, output_dir, name)
    detection = detect_transition_state(screenshot)
    detection["screenshotPath"] = str(screenshot)
    return detection


def compare_region_with_reference(
    candidate_path: Path,
    reference_path: Path,
    rect: tuple[int, int, int, int],
    tolerance: dict[str, float],
) -> dict[str, Any]:
    ref_width, ref_height, _, _ = load_png_pixels(reference_path)
    cand_width, cand_height, _, _ = load_png_pixels(candidate_path)
    x, y, width, height = rect
    scaled_rect = (
        round(x * cand_width / ref_width),
        round(y * cand_height / ref_height),
        max(2, round(width * cand_width / ref_width)),
        max(2, round(height * cand_height / ref_height)),
    )
    reference_metrics = image_region_metrics(reference_path, x, y, width, height)
    candidate_metrics = image_region_metrics(candidate_path, *scaled_rect)

    passed = True
    deltas: dict[str, float] = {}
    for key, allowed_delta in tolerance.items():
        delta = abs(float(candidate_metrics[key]) - float(reference_metrics[key]))
        deltas[key] = round(delta, 4)
        if delta > allowed_delta:
            passed = False

    return {
        "rect": {"x": scaled_rect[0], "y": scaled_rect[1], "width": scaled_rect[2], "height": scaled_rect[3]},
        "referenceMetrics": reference_metrics,
        "candidateMetrics": candidate_metrics,
        "tolerance": tolerance,
        "deltas": deltas,
        "passed": passed,
    }


def state_specs() -> dict[str, dict[str, Any]]:
    refs = reference_images()
    return {
        "role-select": {
            "reference": refs["role_select"],
            "required": 2,
            "regions": [
                {
                    "name": "start_button",
                    "rect": (1618, 900, 230, 140),
                    "tolerance": {"DarkRatio": 0.22, "GoldRatio": 0.03, "BrightRatio": 0.05},
                },
                {
                    "name": "create_button",
                    "rect": (40, 900, 260, 140),
                    "tolerance": {"DarkRatio": 0.25, "GoldRatio": 0.02},
                },
                {
                    "name": "spotlight",
                    "rect": (760, 160, 420, 480),
                    "tolerance": {"DarkRatio": 0.2, "GoldRatio": 0.03, "BrightRatio": 0.04},
                },
            ],
        },
        "login-screen": {
            "reference": refs["login_screen"],
            "required": 2,
            "regions": [
                {
                    "name": "title",
                    "rect": (760, 70, 420, 140),
                    "tolerance": {"DarkRatio": 0.18, "GoldRatio": 0.03, "BrightRatio": 0.04},
                },
                {
                    "name": "enter_button",
                    "rect": (620, 490, 680, 140),
                    "tolerance": {"DarkRatio": 0.2, "GoldRatio": 0.03, "BrightRatio": 0.04},
                },
                {
                    "name": "agreement",
                    "rect": (650, 800, 700, 140),
                    "tolerance": {"DarkRatio": 0.15, "GoldRatio": 0.03, "BrightRatio": 0.05},
                },
            ],
        },
        "main-hud": {
            "reference": refs["main_hud"],
            "required": 2,
            "regions": [
                {
                    "name": "top_left_hud",
                    "rect": (0, 0, 420, 140),
                    "tolerance": {"DarkRatio": 0.22, "GoldRatio": 0.04, "BrightRatio": 0.05},
                },
                {
                    "name": "minimap",
                    "rect": (1520, 0, 400, 290),
                    "tolerance": {"DarkRatio": 0.2, "GoldRatio": 0.05, "BrightRatio": 0.05},
                },
                {
                    "name": "right_menu",
                    "rect": (1530, 580, 370, 430),
                    "tolerance": {"DarkRatio": 0.18, "GoldRatio": 0.03, "BrightRatio": 0.04},
                },
            ],
        },
        "bag-open": {
            "reference": refs["bag"],
            "required": 2,
            "regions": [
                {
                    "name": "title_bar",
                    "rect": (620, 0, 740, 120),
                    "tolerance": {"DarkRatio": 0.18, "GoldRatio": 0.03, "BrightRatio": 0.03},
                },
                {
                    "name": "inventory_grid",
                    "rect": (1220, 140, 520, 560),
                    "tolerance": {"DarkRatio": 0.18, "BrightRatio": 0.03},
                },
                {
                    "name": "bottom_buttons",
                    "rect": (1330, 980, 460, 90),
                    "tolerance": {"DarkRatio": 0.18, "GoldRatio": 0.03, "BrightRatio": 0.04},
                },
            ],
        },
    }


def evaluate_state(candidate_path: Path, state_name: str) -> dict[str, Any]:
    spec = state_specs()[state_name]
    reference = spec["reference"]
    results: list[dict[str, Any]] = []
    passed_count = 0
    for region in spec["regions"]:
        comparison = compare_region_with_reference(candidate_path, reference, region["rect"], region["tolerance"])
        comparison["name"] = region["name"]
        results.append(comparison)
        if comparison["passed"]:
            passed_count += 1
    passed = passed_count >= int(spec["required"])
    return {
        "state": state_name,
        "referencePath": str(reference),
        "required": int(spec["required"]),
        "passedCount": passed_count,
        "passed": passed,
        "regions": results,
    }


def looks_like_unity_init_error(candidate_path: Path) -> dict[str, Any]:
    dialog = image_region_metrics(candidate_path, 380, 350, 1160, 420)
    outer = image_region_metrics(candidate_path, 0, 0, 1920, 1080)
    passed = dialog["BrightRatio"] >= 0.55 and outer["DarkRatio"] >= 0.75
    return {
        "state": "unity-init-error",
        "passed": passed,
        "dialog": dialog,
        "outer": outer,
    }


def looks_like_skills_panel(candidate_path: Path) -> dict[str, Any]:
    main_panel = image_region_metrics(candidate_path, 760, 40, 1100, 930)
    title_bar = image_region_metrics(candidate_path, 760, 10, 520, 120)
    passed = (
        main_panel["DarkRatio"] >= 0.55
        and title_bar["DarkRatio"] >= 0.45
        and title_bar["GoldRatio"] >= 0.002
        and not evaluate_state(candidate_path, "main-hud")["passed"]
        and not evaluate_state(candidate_path, "bag-open")["passed"]
    )
    return {
        "state": "skills-panel",
        "passed": passed,
        "mainPanel": main_panel,
        "titleBar": title_bar,
    }


def looks_like_main_hud_variant(candidate_path: Path) -> dict[str, Any]:
    top_left_hud = image_region_metrics(candidate_path, 0, 0, 420, 140)
    minimap = image_region_metrics(candidate_path, 1520, 0, 400, 290)
    bottom_menu = image_region_metrics(candidate_path, 580, 980, 760, 100)
    passed = (
        top_left_hud["GoldRatio"] >= 0.03
        and top_left_hud["BrightRatio"] >= 0.03
        and minimap["DarkRatio"] >= 0.45
        and bottom_menu["GoldRatio"] >= 0.02
        and bottom_menu["DarkRatio"] <= 0.45
    )
    return {
        "state": "main-hud",
        "passed": passed,
        "heuristic": "layout-variant",
        "topLeftHud": top_left_hud,
        "minimap": minimap,
        "bottomMenu": bottom_menu,
    }


def looks_like_role_select_variant(candidate_path: Path) -> dict[str, Any]:
    start_button = image_region_metrics(candidate_path, 1618, 900, 230, 140)
    spotlight = image_region_metrics(candidate_path, 760, 160, 420, 480)
    bottom_label = image_region_metrics(candidate_path, 643, 780, 286, 85)
    left_create = image_region_metrics(candidate_path, 58, 902, 210, 144)
    center_characters = image_region_metrics(candidate_path, 140, 250, 1120, 560)
    normal_pass = (
        start_button["DarkRatio"] >= 0.45
        and start_button["GoldRatio"] >= 0.006
        and spotlight["GoldRatio"] >= 0.03
        and bottom_label["GoldRatio"] >= 0.03
    )
    magenta_pass = (
        start_button["GoldRatio"] >= 0.02
        and start_button["BrightRatio"] >= 0.02
        and spotlight["BrightRatio"] >= 0.14
        and bottom_label["GoldRatio"] >= 0.02
        and bottom_label["BrightRatio"] >= 0.08
    )
    snowy_pass = (
        start_button["GoldRatio"] >= 0.007
        and start_button["BrightRatio"] >= 0.15
        and spotlight["GoldRatio"] >= 0.012
        and spotlight["BrightRatio"] >= 0.16
        and left_create["GoldRatio"] >= 0.012
        and left_create["BrightRatio"] >= 0.14
        and center_characters["GoldRatio"] >= 0.03
        and center_characters["BrightRatio"] >= 0.25
    )
    passed = normal_pass or magenta_pass or snowy_pass
    return {
        "state": "role-select",
        "passed": passed,
        "heuristic": "snowy-variant",
        "normalPass": normal_pass,
        "magentaPass": magenta_pass,
        "snowyPass": snowy_pass,
        "startButton": start_button,
        "spotlight": spotlight,
        "bottomLabel": bottom_label,
        "leftCreate": left_create,
        "centerCharacters": center_characters,
    }


def looks_like_age_prompt(candidate_path: Path) -> dict[str, Any]:
    dialog = image_region_metrics(candidate_path, 260, 80, 1400, 820)
    close_button = image_region_metrics(candidate_path, 1520, 40, 120, 120)
    enter_button = image_region_metrics(candidate_path, 740, 930, 440, 120)
    passed = (
        dialog["DarkRatio"] >= 0.45
        and dialog["GoldRatio"] >= 0.02
        and close_button["GoldRatio"] >= 0.01
        and enter_button["DarkRatio"] >= 0.45
        and enter_button["GoldRatio"] >= 0.01
    )
    return {
        "state": "age-prompt",
        "passed": passed,
        "dialog": dialog,
        "closeButton": close_button,
        "enterButton": enter_button,
    }


def looks_like_ship_launcher(candidate_path: Path) -> dict[str, Any]:
    center_panel = image_region_metrics(candidate_path, 540, 930, 420, 120)
    ocean_region = image_region_metrics(candidate_path, 1200, 300, 500, 500)
    passed = (
        center_panel["DarkRatio"] >= 0.35
        and center_panel["GoldRatio"] >= 0.01
        and ocean_region["BrightRatio"] >= 0.10
        and ocean_region["DarkRatio"] <= 0.55
    )
    return {
        "state": "ship-launcher",
        "passed": passed,
        "centerPanel": center_panel,
        "oceanRegion": ocean_region,
    }


def looks_like_server_select(candidate_path: Path) -> dict[str, Any]:
    panel = image_region_metrics(candidate_path, 300, 110, 1260, 760)
    left_group = image_region_metrics(candidate_path, 290, 145, 250, 90)
    top_lines = image_region_metrics(candidate_path, 760, 110, 760, 180)
    bottom_enter = image_region_metrics(candidate_path, 735, 920, 470, 120)
    bottom_label = image_region_metrics(candidate_path, 680, 760, 560, 120)
    passed = (
        panel["DarkRatio"] >= 0.92
        and left_group["DarkRatio"] >= 0.88
        and top_lines["DarkRatio"] >= 0.90
        and top_lines["GoldRatio"] >= 0.008
        and top_lines["GreenRatio"] >= 0.002
        and bottom_enter["DarkRatio"] >= 0.65
        and bottom_enter["GoldRatio"] >= 0.006
        and bottom_label["DarkRatio"] >= 0.94
        and bottom_label["GreenRatio"] >= 0.001
    )
    return {
        "state": "server-select",
        "passed": passed,
        "panel": panel,
        "leftGroup": left_group,
        "topLines": top_lines,
        "bottomEnter": bottom_enter,
        "bottomLabel": bottom_label,
    }


def detect_transition_state(candidate_path: Path) -> dict[str, Any]:
    unity_error = looks_like_unity_init_error(candidate_path)
    if unity_error["passed"]:
        unity_error["screenshotPath"] = str(candidate_path)
        return unity_error

    age_prompt = looks_like_age_prompt(candidate_path)
    if age_prompt["passed"]:
        age_prompt["screenshotPath"] = str(candidate_path)
        return age_prompt

    ship_launcher = looks_like_ship_launcher(candidate_path)
    if ship_launcher["passed"]:
        ship_launcher["screenshotPath"] = str(candidate_path)
        return ship_launcher

    server_select = looks_like_server_select(candidate_path)
    if server_select["passed"]:
        server_select["screenshotPath"] = str(candidate_path)
        return server_select

    role_select_variant = looks_like_role_select_variant(candidate_path)
    if role_select_variant["passed"]:
        role_select_variant["screenshotPath"] = str(candidate_path)
        return role_select_variant

    main_hud_variant = looks_like_main_hud_variant(candidate_path)
    if main_hud_variant["passed"]:
        main_hud_variant["screenshotPath"] = str(candidate_path)
        return main_hud_variant

    return {"state": "unknown", "passed": False, "screenshotPath": str(candidate_path)}


def detect_screen_state(candidate_path: Path) -> dict[str, Any]:
    unity_error = looks_like_unity_init_error(candidate_path)
    if unity_error["passed"]:
        unity_error["screenshotPath"] = str(candidate_path)
        return unity_error

    age_prompt = looks_like_age_prompt(candidate_path)
    if age_prompt["passed"]:
        age_prompt["screenshotPath"] = str(candidate_path)
        return age_prompt

    ship_launcher = looks_like_ship_launcher(candidate_path)
    if ship_launcher["passed"]:
        ship_launcher["screenshotPath"] = str(candidate_path)
        return ship_launcher

    server_select = looks_like_server_select(candidate_path)
    if server_select["passed"]:
        server_select["screenshotPath"] = str(candidate_path)
        return server_select

    for state_name in ("main-hud", "role-select", "login-screen"):
        evaluation = evaluate_state(candidate_path, state_name)
        if evaluation["passed"]:
            evaluation["screenshotPath"] = str(candidate_path)
            return evaluation

    skills = looks_like_skills_panel(candidate_path)
    if skills["passed"]:
        skills["screenshotPath"] = str(candidate_path)
        return skills

    role_select_variant = looks_like_role_select_variant(candidate_path)
    if role_select_variant["passed"]:
        role_select_variant["screenshotPath"] = str(candidate_path)
        return role_select_variant

    main_hud_variant = looks_like_main_hud_variant(candidate_path)
    if main_hud_variant["passed"]:
        main_hud_variant["screenshotPath"] = str(candidate_path)
        return main_hud_variant

    bag = evaluate_state(candidate_path, "bag-open")
    if bag["passed"]:
        bag["screenshotPath"] = str(candidate_path)
        return bag

    return {"state": "unknown", "passed": False, "screenshotPath": str(candidate_path)}


def wait_for_state(
    serial: str,
    output_dir: Path,
    target_states: set[str],
    *,
    timeout_seconds: int,
    poll_seconds: int = 5,
    prefix: str,
) -> dict[str, Any]:
    output_dir.mkdir(parents=True, exist_ok=True)
    max_attempts = max(1, (timeout_seconds + poll_seconds - 1) // poll_seconds)
    last_detection: dict[str, Any] = {"state": "unknown", "passed": False}

    for attempt in range(1, max_attempts + 1):
        screenshot = capture_named_screenshot(serial, output_dir, f"{prefix}-{attempt}.png")
        detection = detect_screen_state(screenshot)
        detection["attempt"] = attempt
        last_detection = detection
        if detection["state"] == "unity-init-error":
            return detection
        if detection["state"] in target_states:
            return detection
        if attempt < max_attempts:
            time.sleep(poll_seconds)

    return last_detection


def input_text(serial: str, value: str) -> None:
    adb_lines(["-s", serial, "shell", "input", "text", value])


def clear_focused_text(serial: str, repeats: int = 12) -> None:
    for _ in range(repeats):
        adb_lines(["-s", serial, "shell", "input", "keyevent", "67"], check=False)
        time.sleep(0.12)


def submit_login(serial: str, run_dir: Path, anchor_screenshot: Path, account: str = DEFAULT_ACCOUNT, password: str = DEFAULT_PASSWORD) -> dict[str, Any]:
    screens_dir = run_dir / "screens"
    log_progress("checking whether saved credentials can be reused")
    quick_login_tap = tap_point(serial, LOGIN_BUTTON_POINT, anchor_screenshot)
    quick_detection = capture_state_once(serial, screens_dir, "login-fast-path.png", delay_seconds=2)
    quick_detection["loginTap"] = {"x": quick_login_tap[0], "y": quick_login_tap[1]}
    if quick_detection["state"] != "login-screen":
        quick_detection["loginMethod"] = "saved-credentials"
        log_progress(f"saved credentials detected, fast login => {quick_detection['state']}")
        return quick_detection

    log_progress("saved credentials not available, submitting account and password")
    account_tap = tap_point(serial, ACCOUNT_FIELD_POINT, anchor_screenshot)
    time.sleep(0.35)
    clear_focused_text(serial)
    input_text(serial, account)
    time.sleep(0.35)
    account_confirm_tap = tap_point(serial, KEYBOARD_CONFIRM_POINT, anchor_screenshot)
    time.sleep(0.35)
    password_tap = tap_point(serial, PASSWORD_FIELD_POINT, anchor_screenshot)
    time.sleep(0.35)
    clear_focused_text(serial)
    input_text(serial, password)
    time.sleep(0.35)
    password_confirm_tap = tap_point(serial, KEYBOARD_CONFIRM_POINT, anchor_screenshot)
    time.sleep(0.35)
    login_tap = tap_point(serial, LOGIN_BUTTON_POINT, anchor_screenshot)
    time.sleep(1.5)
    screenshot = capture_named_screenshot(serial, screens_dir, "login-submit.png")
    log_progress(f"login submitted, captured {screenshot.name}")
    return {
        "state": "login-submitted",
        "loginMethod": "typed-credentials",
        "accountTap": {"x": account_tap[0], "y": account_tap[1]},
        "accountConfirmTap": {"x": account_confirm_tap[0], "y": account_confirm_tap[1]},
        "passwordTap": {"x": password_tap[0], "y": password_tap[1]},
        "passwordConfirmTap": {"x": password_confirm_tap[0], "y": password_confirm_tap[1]},
        "loginTap": {"x": login_tap[0], "y": login_tap[1]},
        "screenshotPath": str(screenshot),
    }


def advance_from_launch(serial: str, run_dir: Path, anchor_screenshot: Path) -> Path:
    screens_dir = run_dir / "screens"
    current = anchor_screenshot
    detection = detect_transition_state(current)

    if detection["state"] == "age-prompt":
        log_progress("dismissing startup age prompt")
        tap_point(serial, AGE_PROMPT_CLOSE_POINT, current)
        current = capture_named_screenshot(serial, screens_dir, "launch-after-age-close.png")
        time.sleep(2)
        detection = detect_transition_state(current)

    if detection["state"] == "ship-launcher":
        log_progress("launch screen detected, tapping 进入游戏")
        tap_point(serial, SHIP_ENTER_GAME_POINT, current)
        time.sleep(4)
        current = capture_named_screenshot(serial, screens_dir, "launch-after-enter-game.png")

    return current


def submit_server_select(serial: str, run_dir: Path, anchor_screenshot: Path, step_index: int) -> dict[str, Any]:
    screens_dir = run_dir / "screens"
    log_progress(f"selecting 永久区1线 and entering game, attempt {step_index}")
    group_tap = tap_point(serial, SERVER_GROUP_POINT, anchor_screenshot)
    time.sleep(0.4)
    server_line_taps = tap_points(serial, SERVER_LINE_POINTS, anchor_screenshot, interval_seconds=0.35)
    time.sleep(0.4)
    enter_taps = tap_points(serial, SERVER_ENTER_POINTS, anchor_screenshot, interval_seconds=0.35)
    time.sleep(1.2)
    confirm_taps = tap_points(serial, SERVER_ENTER_POINTS, anchor_screenshot, interval_seconds=0.25)
    detection = capture_transition_state_once(
        serial,
        screens_dir,
        f"server-enter-{step_index}.png",
        delay_seconds=4 if step_index == 1 else 5,
    )
    detection["stepIndex"] = step_index
    detection["groupTap"] = {"x": group_tap[0], "y": group_tap[1]}
    detection["serverLineTaps"] = [{"x": tap[0], "y": tap[1]} for tap in server_line_taps]
    detection["enterTaps"] = [{"x": tap[0], "y": tap[1]} for tap in enter_taps]
    detection["confirmTaps"] = [{"x": tap[0], "y": tap[1]} for tap in confirm_taps]
    log_progress(f"server-select attempt {step_index} => {detection['state']}")
    return detection


def summarize_server_attempts(server_attempts: list[dict[str, Any]]) -> list[dict[str, Any]]:
    return [
        {
            "stepIndex": attempt.get("stepIndex"),
            "state": attempt.get("state"),
            "screenshotPath": attempt.get("screenshotPath"),
        }
        for attempt in server_attempts
    ]


def advance_from_server_select(
    serial: str,
    run_dir: Path,
    screens_dir: Path,
    anchor_screenshot: Path,
    *,
    login_submit: dict[str, Any] | None = None,
) -> dict[str, Any]:
    server_attempts: list[dict[str, Any]] = []
    anchor = anchor_screenshot
    for step_index in (1, 2):
        server_result = submit_server_select(serial, run_dir, anchor, step_index)
        server_attempts.append(server_result)
        if server_result["state"] in {"main-hud", "role-select", "unity-init-error"}:
            result = dict(server_result)
            result["serverAttempts"] = summarize_server_attempts(server_attempts)
            if login_submit is not None:
                result["loginSubmit"] = login_submit
            return result

        anchor = Path(server_result["screenshotPath"])

    result = dict(server_attempts[-1])
    result["serverAttempts"] = summarize_server_attempts(server_attempts)
    if login_submit is not None:
        result["loginSubmit"] = login_submit
    return result


def submit_role_select(serial: str, run_dir: Path, anchor_screenshot: Path, step_index: int) -> dict[str, Any]:
    screens_dir = run_dir / "screens"
    log_progress(f"selecting role and tapping 开始, attempt {step_index}")
    role_taps = tap_points(serial, ROLE_CHARACTER_POINTS, anchor_screenshot, interval_seconds=0.35)
    time.sleep(0.4)
    start_taps = tap_points(serial, START_BUTTON_POINTS, anchor_screenshot, interval_seconds=0.35)
    detection = capture_transition_state_once(
        serial,
        screens_dir,
        f"role-enter-{step_index}.png",
        delay_seconds=4 if step_index == 1 else 5,
    )
    detection["stepIndex"] = step_index
    detection["roleTaps"] = [{"x": tap[0], "y": tap[1]} for tap in role_taps]
    detection["startTaps"] = [{"x": tap[0], "y": tap[1]} for tap in start_taps]
    log_progress(f"role-select attempt {step_index} => {detection['state']}")
    return detection


def drive_to_main_hud(serial: str, package_name: str, run_dir: Path) -> dict[str, Any]:
    log_progress(f"launching {package_name}")
    adb_lines(["-s", serial, "shell", "am", "force-stop", package_name], check=False)
    adb_lines(["-s", serial, "shell", "monkey", "-p", package_name, "-c", "android.intent.category.LAUNCHER", "1"])

    screens_dir = run_dir / "screens"
    time.sleep(6)
    anchor = capture_named_screenshot(serial, screens_dir, "launch-anchor.png")
    anchor = advance_from_launch(serial, run_dir, anchor)

    login_submit = submit_login(serial, run_dir, anchor)
    if login_submit["state"] not in {"main-hud", "role-select", "unity-init-error"}:
        entry = advance_from_server_select(
            serial,
            run_dir,
            screens_dir,
            Path(login_submit["screenshotPath"]),
            login_submit=login_submit,
        )
    else:
        entry = login_submit
    log_progress(f"pre-role => {entry['state']}")
    if entry["state"] in {"main-hud", "unity-init-error"}:
        return entry
    if entry["state"] != "role-select":
        final_check = capture_transition_state_once(serial, screens_dir, "final-check.png", delay_seconds=4)
        final_check["loginSubmit"] = login_submit
        return final_check

    result = submit_role_select(serial, run_dir, Path(entry["screenshotPath"]), 1)
    if result["state"] != "role-select":
        return result
    final_check = capture_transition_state_once(serial, screens_dir, "post-start-final.png", delay_seconds=4)
    final_check["roleSelectAttempt"] = result
    return final_check


def open_bag(serial: str, run_dir: Path, anchor_screenshot: Path) -> dict[str, Any]:
    screens_dir = run_dir / "screens"
    tap_point(serial, BAG_BUTTON_POINT, anchor_screenshot)
    time.sleep(2)
    screenshot = capture_named_screenshot(serial, screens_dir, "bag-open.png")
    detection = detect_screen_state(screenshot)
    detection["screenshotPath"] = str(screenshot)
    return detection


def close_panel(serial: str, anchor_screenshot: Path) -> None:
    tap_point(serial, CLOSE_PANEL_POINT, anchor_screenshot)
    time.sleep(1)


def open_skills(serial: str, run_dir: Path, anchor_screenshot: Path) -> dict[str, Any]:
    screens_dir = run_dir / "screens"
    tap_point(serial, SKILLS_BUTTON_POINT, anchor_screenshot)
    time.sleep(2)
    screenshot = capture_named_screenshot(serial, screens_dir, "skills-open.png")
    detection = looks_like_skills_panel(screenshot)
    detection["screenshotPath"] = str(screenshot)
    return detection


def write_android_regression_artifact(run_dir: Path, name: str, data: dict[str, Any]) -> Path:
    path = run_dir / f"{name}.json"
    write_json(path, data)
    return path
