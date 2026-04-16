from __future__ import annotations

import json
from typing import Any
from urllib.error import HTTPError, URLError
from urllib.parse import urljoin
from urllib.request import Request, urlopen

from .common import env_profile


DEFAULT_ADMIN_NAME = "admin"
DEFAULT_ADMIN_PASSWORD = "123456"


class WebGMError(RuntimeError):
    pass


def webgm_base_url(profile_name: str = "local-lan") -> str:
    base_url = str(env_profile(profile_name)["gm"]["webUrl"]).rstrip("/")
    return base_url + "/"


def _json_request(
    profile_name: str,
    method: str,
    path: str,
    *,
    payload: dict[str, Any] | None = None,
    token: str = "",
    timeout: float = 10.0,
) -> dict[str, Any]:
    url = urljoin(webgm_base_url(profile_name), path.lstrip("/"))
    data = None
    headers = {"Accept": "application/json"}
    if payload is not None:
        data = json.dumps(payload, ensure_ascii=False).encode("utf-8")
        headers["Content-Type"] = "application/json"
    if token:
        headers["Authorization"] = token

    request = Request(url, data=data, headers=headers, method=method.upper())
    try:
        with urlopen(request, timeout=timeout) as response:
            body = response.read().decode("utf-8", errors="replace")
            return json.loads(body) if body else {}
    except HTTPError as exc:
        body = exc.read().decode("utf-8", errors="replace")
        raise WebGMError(f"WebGM HTTP {exc.code}: {body or exc.reason}") from exc
    except URLError as exc:
        raise WebGMError(f"WebGM connection error: {exc.reason}") from exc
    except TimeoutError as exc:
        raise WebGMError(f"WebGM request timed out: {url}") from exc


def init_admin(profile_name: str = "local-lan") -> dict[str, Any]:
    return _json_request(profile_name, "GET", "/api/users/init")


def login(profile_name: str = "local-lan", *, name: str = DEFAULT_ADMIN_NAME, password: str = DEFAULT_ADMIN_PASSWORD) -> str:
    payload = _json_request(profile_name, "POST", "/api/users/login", payload={"name": name, "password": password})
    if not payload.get("success") or not payload.get("token"):
        raise WebGMError(f"WebGM login failed: {payload}")
    return str(payload["token"])


def ensure_session(profile_name: str = "local-lan", *, name: str = DEFAULT_ADMIN_NAME, password: str = DEFAULT_ADMIN_PASSWORD) -> str:
    try:
        return login(profile_name, name=name, password=password)
    except WebGMError:
        init_admin(profile_name)
        return login(profile_name, name=name, password=password)


def game_status(profile_name: str, token: str, *, server_id: int = 1) -> dict[str, Any]:
    payload = _json_request(profile_name, "POST", "/api/game/server/game_status", payload={"serverId": server_id}, token=token)
    if not payload.get("success"):
        raise WebGMError(f"WebGM game_status failed: {payload}")
    return payload.get("data") or {}


def player_search(profile_name: str, token: str, *, zone_id: int, role_name: str, skip: int = 0, limit: int = 1) -> list[dict[str, Any]]:
    payload = _json_request(
        profile_name,
        "POST",
        "/api/game/player/search",
        payload={"zoneId": zone_id, "roleName": role_name, "skip": skip, "limit": limit},
        token=token,
    )
    if not payload.get("success"):
        raise WebGMError(f"WebGM player_search failed: {payload}")
    return list(payload.get("data") or [])


def role_data(profile_name: str, token: str, *, zone_id: int, game_user_id: str) -> dict[str, Any]:
    payload = _json_request(
        profile_name,
        "POST",
        "/api/game/player/role_data",
        payload={"zoneId": zone_id, "gameUserId": game_user_id},
        token=token,
    )
    if not payload.get("success"):
        raise WebGMError(f"WebGM role_data failed: {payload}")
    return payload.get("data") or {}
