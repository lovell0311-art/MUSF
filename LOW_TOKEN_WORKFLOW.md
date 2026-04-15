# Low Token Workflow

Installed local tools:
- `rg` for fast text search
- `fd` for fast file discovery
- `sg` (ast-grep) for structural code search

PowerShell helpers:
- `csearch <pattern> [path...]`
- `ffind <pattern> [path]`
- `astfind <pattern> [path] [-Lang csharp]`
- `unity-errors [-ProjectPath <path>]`

Applied project ignores:
- `F:\MUSF\.ignore`
- `F:\MUSF\Client\Unity\.ignore`
- `F:\MUSF\Server\Unity\.ignore`

Why this saves tokens:
- Search stays local first, so only small result sets need AI context.
- Unity generated folders and logs are excluded by default from code search.
- `unity-errors` filters logs down to errors and exceptions only.

Not auto-enabled:
- Crashlytics / Sentry still need project credentials and service setup.
