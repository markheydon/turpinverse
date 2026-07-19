# Turpinverse documentation site (Hugo)

Hugo source lives here. Built output goes to `../docs/` (GitHub Pages).

You do **not** need Hugo installed locally if you use Docker or Podman.

## Prerequisites

| Tool | Required for |
|------|----------------|
| .NET SDK 10.x | Generating `content/` and `data/` from canon JSON |
| Docker or Podman | Building and previewing the Hugo site |

## 1. Generate content from canon

Run from the repository root:

```powershell
dotnet run --project src/Turpinverse.Tools.GenerateHugoContent
```

This writes markdown under `site/content/` and `site/data/organisations.json` and `events.json`.

The site uses the [PaperMod](https://github.com/adityatelange/hugo-PaperMod) theme as a git submodule at `site/themes/PaperMod`, with Turpinverse brand styling in `assets/css/extended/turpinverse.css`.

After cloning the repository, initialise submodules:

```powershell
git submodule update --init --recursive
```

## 2. Build or preview with a container

Use the helper scripts from the repo root (recommended on Windows):

```powershell
.\scripts\hugo.ps1 build    # writes static site to docs/
.\scripts\hugo.ps1 serve    # live preview at http://localhost:1313
.\scripts\hugo.ps1 preview  # build + serve built docs/ at http://localhost:8080
```

Or run the container commands directly:

### Build to `docs/`

```powershell
# Podman
podman run --rm -v "${PWD}:/src:Z" -w /src/site docker.io/hugomods/hugo:latest hugo --minify

# Docker
docker run --rm -v "${PWD}:/src" -w /src/site docker.io/hugomods/hugo:latest hugo --minify
```

### Live dev server (layout/content changes)

```powershell
podman run --rm -p 1313:1313 -v "${PWD}:/src:Z" -w /src/site docker.io/hugomods/hugo:latest `
  hugo server --bind 0.0.0.0 --baseURL http://localhost:1313
```

Open http://localhost:1313

### Preview built static files (matches GitHub Pages output)

```powershell
# Build first, then:
podman run --rm -p 8080:80 -v "${PWD}/docs:/usr/share/nginx/html:ro,Z" docker.io/library/nginx:alpine
```

Open http://localhost:8080

## Windows notes

- Run commands from the **repository root** (`turpinverse/`), not from `site/`.
- If volume mounts fail with `${PWD}`, set an explicit path:

  ```powershell
  $root = (Get-Location).Path -replace '\\', '/'
  podman run --rm -v "${root}:/src:Z" -w /src/site docker.io/hugomods/hugo:latest hugo --minify
  ```

- Podman Desktop and Docker Desktop both work; replace `podman` with `docker` if needed.
- The `:Z` volume flag is for SELinux on Linux and is harmless on Windows.

## Container image

[`hugomods/hugo`](https://hub.docker.com/r/hugomods/hugo) — Hugo **Extended** edition, matching CI (`.github/workflows/hugo-pages.yml`).

## Troubleshooting

| Issue | Fix |
|-------|-----|
| `module not found` / PaperMod theme error | Run `git submodule update --init --recursive` from the repo root |
| `statfs .../docs: no such file or directory` | Hugo build failed or was skipped — run `.\scripts\hugo.ps1 build` first; `docs/` is created by Hugo, not checked into git |
| Empty `docs/` or missing pages | Run the content generator before `hugo build` |
| `site/content/personas/` is empty | `dotnet run --project src/Turpinverse.Tools.GenerateHugoContent` |
| Port already in use | Change `-p 1313:1313` to e.g. `-p 1314:1313` and update `--baseURL` |
| Broken links in local preview | Use `hugo server` with `--baseURL http://localhost:1313`, or `preview` after `build` |

### Recommended workflow (Windows + Podman)

```powershell
# One command — generates content, builds, and serves
.\scripts\hugo.ps1 serve

# Or build then preview static output (like GitHub Pages)
.\scripts\hugo.ps1 preview
```

Do **not** run the nginx preview command until `docs/index.html` exists.
