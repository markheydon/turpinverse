# Turpinverse documentation site (Hugo)

The public Hugo site is Turpinverse's **browsable reference**: human-readable personas,
organisations, timeline, career/portfolio, deals, and cases for anyone who wants to read or
reuse the fiction without running the export app. For interactive explore/filter/download,
see the Blazor app described in [docs/product-surfaces.md](../docs/product-surfaces.md).

Hugo source lives here. Built output goes to `public/` (gitignored; deployed via GitHub Actions to [turpinverse.uk](https://turpinverse.uk)).

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

This writes markdown under `site/content/` (including `deals/` and `cases/`) and `site/data/organisations.json`, `events.json`, `deals.json`, and `cases.json`.

Career portfolio images live in `site/static/images/` (projects and achievements). The same SVGs are mirrored under `src/Turpinverse.Web/wwwroot/images/` for Blazor contact detail parity — keep both in sync when adding assets.

The site uses the [PaperMod](https://github.com/adityatelange/hugo-PaperMod) theme via [Hugo modules](https://gohugo.io/hugo-modules/), with Turpinverse brand styling in `assets/css/extended/turpinverse.css`.

Hugo downloads the theme automatically on first build (`hugo mod get` is optional after clone). Module metadata lives in `site/go.mod` and `site/go.sum`.

To update PaperMod:

```powershell
cd site
hugo mod get -u github.com/adityatelange/hugo-PaperMod
```

## 2. Build or preview with a container

Use the helper scripts from the repo root (recommended on Windows):

```powershell
.\scripts\Invoke-HugoSite.ps1 build    # writes static site to site/public/
.\scripts\Invoke-HugoSite.ps1 serve    # live preview at http://localhost:1313
.\scripts\Invoke-HugoSite.ps1 preview  # build + serve built site/public/ at http://localhost:8080
```

Or run the container commands directly:

### Build to `site/public/`

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
podman run --rm -p 8080:80 -v "${PWD}/site/public:/usr/share/nginx/html:ro,Z" docker.io/library/nginx:alpine
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

[`hugomods/hugo`](https://hub.docker.com/r/hugomods/hugo) — Hugo **Extended** edition, matching CI (`.github/workflows/hugo-build.yml`, currently **0.165.0**).

## Custom domain (turpinverse.uk)

The site is published at **https://turpinverse.uk** via GitHub Pages. Hugo copies `site/static/CNAME` into `public/CNAME` on build.

### GitHub Pages setup

1. Repo → **Settings** → **Pages** → **Build and deployment** → set **Source** to **GitHub Actions**
2. Repo → **Settings** → **Pages** → **Custom domain** → enter `turpinverse.uk` → Save
3. Wait for DNS check to pass, then enable **Enforce HTTPS**

### Cloudflare DNS (apex domain)

Add four **A** records for `@`:

| Type | Name | Content |
|------|------|---------|
| A | `@` | `185.199.108.153` |
| A | `@` | `185.199.109.153` |
| A | `@` | `185.199.110.153` |
| A | `@` | `185.199.111.153` |

Optional **CNAME** for `www` → `markheydon.github.io` if you want `www.turpinverse.uk`.

Use **DNS only** (grey cloud) initially until the GitHub HTTPS certificate provisions, then re-enable the proxy if desired.

After merge and **Hugo Deploy**, verify `https://turpinverse.uk` loads.

## Troubleshooting

| Issue | Fix |
|-------|-----|
| `module not found` / PaperMod theme error | From `site/`, run `hugo mod get` (or rebuild — Hugo fetches modules on build) |
| `statfs .../site/public: no such file or directory` | Hugo build failed or was skipped — run `.\scripts\Invoke-HugoSite.ps1 build` first; `site/public/` is created by Hugo, not checked into git |
| Empty `site/public/` or missing pages | Run the content generator before `hugo build` |
| `site/content/personas/` is empty | `dotnet run --project src/Turpinverse.Tools.GenerateHugoContent` |
| Port already in use | Change `-p 1313:1313` to e.g. `-p 1314:1313` and update `--baseURL` |
| Broken links in local preview | Use `hugo server` with `--baseURL http://localhost:1313`, or `preview` after `build` |

### Recommended workflow (Windows + Podman)

```powershell
# One command — generates content, builds, and serves
.\scripts\Invoke-HugoSite.ps1 serve

# Or build then preview static output (like GitHub Pages)
.\scripts\Invoke-HugoSite.ps1 preview
```

Do **not** run the nginx preview command until `site/public/index.html` exists.
