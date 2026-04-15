---
name: musf-map-regression
description: Validate MUSF role, NPC, minimap, big-map, and scene-bundle consistency. Use when investigating map drift, minimap scaling errors, NPC or player marker offsets, ancient battlefield scaling issues, scene bundle mismatches, or repeated regressions where the map image no longer matches the in-game coordinates.
---

# MUSF Map Regression

Focus on coordinate alignment, not UI restyling.

## Guardrails

- Treat UI baseline bundles as frozen
- Prefer fixing shared map math, config, or bundle consistency before per-map hard patches
- Use paired screenshots, coordinates, and scene names as evidence
- Keep validation scoped to local/LAN server and the known test account unless told otherwise

## Workflow

1. Record the actual world coordinate, map name, and scene bundle in use.
2. Capture both the big map and minimap state.
3. Compare player arrow, NPC markers, and transport markers against the expected map image.
4. Check:
   - map config values
   - minimap scale and offset logic
   - scene bundle identity
   - stale hot-update resources
5. Prefer reproducing on at least two maps:
   - one known-good map
   - one failing map
6. Report whether the root cause is:
   - math/transform logic
   - per-map config
   - stale cache
   - wrong bundle/resource line

## Required Coverage

- 勇者大陆
- 仙踪林
- 冰风谷
- 幽暗森林
- 古战场

## Reporting

- Include map name, coordinate, screenshot path, and the suspected failure class.
- Say explicitly whether the next step is cache cleanup, config correction, code fix, or resource republish.
