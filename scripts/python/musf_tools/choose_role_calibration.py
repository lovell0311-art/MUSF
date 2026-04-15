from __future__ import annotations

import math
from pathlib import Path
from typing import Any

from PIL import Image, ImageDraw

from .common import ensure_parent, musf_root, now_stamp, write_json


CAMERA_POSITION = (-5.0, 7.0, 25.0)
CAMERA_EULER = (0.0, 180.0, 0.0)
CAMERA_FOV = 45.0
DEFAULT_PLANE_Y = 1.15

CURRENT_PEDESTALS = [
    {"index": 0, "position": (7.10, 1.15, 5.04), "yaw": -10.0},
    {"index": 1, "position": (4.61, 1.15, 1.96), "yaw": -5.0},
    {"index": 2, "position": (-0.84, 1.15, 3.76), "yaw": 0.0},
    {"index": 3, "position": (-4.95, 1.15, 4.44), "yaw": 5.0},
    {"index": 4, "position": (-10.01, 1.15, 4.05), "yaw": 10.0},
]


def _rotate_x(vector: tuple[float, float, float], degrees: float) -> tuple[float, float, float]:
    radians = math.radians(degrees)
    cos_value = math.cos(radians)
    sin_value = math.sin(radians)
    x, y, z = vector
    return (x, y * cos_value - z * sin_value, y * sin_value + z * cos_value)


def _rotate_y(vector: tuple[float, float, float], degrees: float) -> tuple[float, float, float]:
    radians = math.radians(degrees)
    cos_value = math.cos(radians)
    sin_value = math.sin(radians)
    x, y, z = vector
    return (x * cos_value + z * sin_value, y, -x * sin_value + z * cos_value)


def _rotate_z(vector: tuple[float, float, float], degrees: float) -> tuple[float, float, float]:
    radians = math.radians(degrees)
    cos_value = math.cos(radians)
    sin_value = math.sin(radians)
    x, y, z = vector
    return (x * cos_value - y * sin_value, x * sin_value + y * cos_value, z)


def _rotate_world_to_camera(vector: tuple[float, float, float]) -> tuple[float, float, float]:
    x_rotated = _rotate_z(vector, -CAMERA_EULER[2])
    y_rotated = _rotate_x(x_rotated, -CAMERA_EULER[0])
    return _rotate_y(y_rotated, -CAMERA_EULER[1])


def _rotate_camera_to_world(vector: tuple[float, float, float]) -> tuple[float, float, float]:
    y_rotated = _rotate_y(vector, CAMERA_EULER[1])
    x_rotated = _rotate_x(y_rotated, CAMERA_EULER[0])
    return _rotate_z(x_rotated, CAMERA_EULER[2])


def world_to_screen(
    world_point: tuple[float, float, float],
    width: int,
    height: int,
) -> dict[str, Any]:
    relative = (
        world_point[0] - CAMERA_POSITION[0],
        world_point[1] - CAMERA_POSITION[1],
        world_point[2] - CAMERA_POSITION[2],
    )
    camera_space = _rotate_world_to_camera(relative)
    z_value = camera_space[2]
    if z_value <= 0.001:
        return {"visible": False, "screen": None, "cameraSpace": camera_space}

    aspect = width / float(height)
    tangent = math.tan(math.radians(CAMERA_FOV) * 0.5)
    ndc_x = camera_space[0] / (z_value * tangent * aspect)
    ndc_y = camera_space[1] / (z_value * tangent)
    screen_x = (ndc_x + 1.0) * 0.5 * width
    screen_y = (1.0 - ndc_y) * 0.5 * height
    return {
        "visible": True,
        "screen": (screen_x, screen_y),
        "cameraSpace": camera_space,
    }


def screen_to_world_on_plane(
    screen_x: float,
    screen_y: float,
    width: int,
    height: int,
    plane_y: float,
) -> dict[str, Any]:
    aspect = width / float(height)
    tangent = math.tan(math.radians(CAMERA_FOV) * 0.5)
    ndc_x = (screen_x / float(width)) * 2.0 - 1.0
    ndc_y = 1.0 - (screen_y / float(height)) * 2.0

    direction_camera = (
        ndc_x * aspect * tangent,
        ndc_y * tangent,
        1.0,
    )
    length = math.sqrt(
        direction_camera[0] * direction_camera[0]
        + direction_camera[1] * direction_camera[1]
        + direction_camera[2] * direction_camera[2]
    )
    direction_camera = (
        direction_camera[0] / length,
        direction_camera[1] / length,
        direction_camera[2] / length,
    )
    direction_world = _rotate_camera_to_world(direction_camera)
    if abs(direction_world[1]) < 0.0001:
        return {"ok": False, "reason": "ray-parallel-plane"}

    distance = (plane_y - CAMERA_POSITION[1]) / direction_world[1]
    if distance <= 0:
        return {"ok": False, "reason": "plane-behind-camera"}

    world_point = (
        CAMERA_POSITION[0] + direction_world[0] * distance,
        CAMERA_POSITION[1] + direction_world[1] * distance,
        CAMERA_POSITION[2] + direction_world[2] * distance,
    )
    return {"ok": True, "world": world_point}


def _parse_point(value: str) -> dict[str, Any]:
    label = f"p{value}"
    raw = value
    if "=" in value:
        label, raw = value.split("=", 1)
    x_text, y_text = [part.strip() for part in raw.split(",", 1)]
    return {"label": label.strip(), "screen": (float(x_text), float(y_text))}


def _draw_cross(draw: ImageDraw.ImageDraw, x_value: float, y_value: float, color: tuple[int, int, int]) -> None:
    x_int = int(round(x_value))
    y_int = int(round(y_value))
    draw.line((x_int - 14, y_int, x_int + 14, y_int), fill=color, width=3)
    draw.line((x_int, y_int - 14, x_int, y_int + 14), fill=color, width=3)


def choose_role_calibrate(
    image_path: str,
    point_args: list[str],
    plane_y: float = DEFAULT_PLANE_Y,
    annotated_out: str = "",
    json_out: str = "",
) -> str:
    image_file = Path(image_path)
    if not image_file.exists():
        raise FileNotFoundError(f"Image not found: {image_file}")

    image = Image.open(image_file).convert("RGB")
    width, height = image.size

    projected = []
    for pedestal in CURRENT_PEDESTALS:
        projection = world_to_screen(pedestal["position"], width, height)
        projected.append(
            {
                "index": pedestal["index"],
                "position": pedestal["position"],
                "yaw": pedestal["yaw"],
                "projection": projection,
            }
        )

    requested_points = [_parse_point(value) for value in point_args]
    solved = []
    for item in requested_points:
        result = screen_to_world_on_plane(item["screen"][0], item["screen"][1], width, height, plane_y)
        solved.append(
            {
                "label": item["label"],
                "screen": item["screen"],
                "solve": result,
            }
        )

    stamp = now_stamp()
    report_dir = musf_root() / "reports" / "choose-role-calibration"
    json_path = Path(json_out) if json_out else report_dir / f"choose-role-calibration-{stamp}.json"
    annotated_path = Path(annotated_out) if annotated_out else report_dir / f"choose-role-calibration-{stamp}.png"

    report = {
        "imagePath": str(image_file),
        "screenSize": {"width": width, "height": height},
        "camera": {
            "position": CAMERA_POSITION,
            "euler": CAMERA_EULER,
            "fov": CAMERA_FOV,
            "planeY": plane_y,
        },
        "currentPedestals": projected,
        "requestedPoints": solved,
        "snippet": [
            f"new Vector3({entry['solve']['world'][0]:0.2f}f, {entry['solve']['world'][1]:0.2f}f, {entry['solve']['world'][2]:0.2f}f),"
            for entry in solved
            if entry["solve"].get("ok")
        ],
    }

    annotated = image.copy()
    draw = ImageDraw.Draw(annotated)
    for item in projected:
        projection = item["projection"]
        if not projection["visible"] or projection["screen"] is None:
            continue
        screen_x, screen_y = projection["screen"]
        _draw_cross(draw, screen_x, screen_y, (255, 210, 0))
        draw.text((screen_x + 8, screen_y - 28), f"P{item['index']}", fill=(255, 210, 0))

    for item in solved:
        screen_x, screen_y = item["screen"]
        _draw_cross(draw, screen_x, screen_y, (0, 255, 255))
        draw.text((screen_x + 8, screen_y - 28), item["label"], fill=(0, 255, 255))

    ensure_parent(json_path)
    write_json(json_path, report)
    ensure_parent(annotated_path)
    annotated.save(annotated_path)
    return str(json_path)
