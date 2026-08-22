#!/usr/bin/env bash
# Build or preview the Hugo documentation site via Docker/Podman (no local Hugo install required).
# Usage: ./scripts/invoke-hugo-site.sh [build|serve|preview] [--runtime docker|podman] [--serve-port N] [--preview-port N]

set -euo pipefail

COMMAND="build"
RUNTIME=""
SERVE_PORT=1313
PREVIEW_PORT=8080

usage() {
    cat <<'EOF'
Usage: invoke-hugo-site.sh [build|serve|preview] [options]

Commands:
  build     Generate content + production build to site/public/ (default)
  serve     Hugo dev server with live reload
  preview   Generate content, build, then serve site/public/ via nginx

Options:
  --runtime RUNTIME   Container runtime: docker or podman (auto-detected if omitted)
  --serve-port PORT   Port for serve command (default: 1313)
  --preview-port PORT Port for preview command (default: 8080)
  -h, --help          Show this help

Examples:
  ./scripts/invoke-hugo-site.sh preview
  ./scripts/invoke-hugo-site.sh serve --runtime docker
  ./scripts/invoke-hugo-site.sh build
EOF
}

while [[ $# -gt 0 ]]; do
    case "$1" in
        build|serve|preview)
            COMMAND="$1"
            shift
            ;;
        --runtime)
            RUNTIME="${2:-}"
            shift 2
            ;;
        --serve-port)
            SERVE_PORT="${2:-}"
            shift 2
            ;;
        --preview-port)
            PREVIEW_PORT="${2:-}"
            shift 2
            ;;
        -h|--help)
            usage
            exit 0
            ;;
        *)
            echo "error: unknown argument '$1'" >&2
            usage >&2
            exit 1
            ;;
    esac
done

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
SITE_DIR="$REPO_ROOT/site"
PUBLIC_PATH="$SITE_DIR/public"
THEME_MARKER="$SITE_DIR/themes/PaperMod/layouts/_partials/head.html"
HUGO_IMAGE="docker.io/hugomods/hugo:latest"
NGINX_IMAGE="docker.io/library/nginx:alpine"

resolve_runtime() {
    if [[ -n "$RUNTIME" ]]; then
        if ! command -v "$RUNTIME" >/dev/null 2>&1; then
            echo "error: container runtime '$RUNTIME' not found on PATH" >&2
            exit 1
        fi
        return
    fi

    if command -v podman >/dev/null 2>&1; then
        RUNTIME="podman"
    elif command -v docker >/dev/null 2>&1; then
        RUNTIME="docker"
    else
        echo "error: no container runtime found. Install Podman or Docker, or pass --runtime." >&2
        exit 1
    fi
}

volume_mount() {
    local host_path="$1"
    local container_path="$2"
    local opts="${3:-}"

    if [[ "$RUNTIME" == "podman" ]]; then
        if [[ -n "$opts" ]]; then
            opts="${opts},Z"
        else
            opts="Z"
        fi
    fi

    if [[ -n "$opts" ]]; then
        printf '%s:%s:%s' "$host_path" "$container_path" "$opts"
    else
        printf '%s:%s' "$host_path" "$container_path"
    fi
}

initialize_hugo_theme_submodule() {
    if [[ -f "$THEME_MARKER" ]]; then
        return
    fi

    echo "PaperMod theme submodule is missing; initialising..." >&2
    if ! command -v git >/dev/null 2>&1; then
        echo "error: PaperMod theme not found at site/themes/PaperMod. Install Git and run: git submodule update --init --recursive" >&2
        exit 1
    fi

    if ! (cd "$REPO_ROOT" && git submodule update --init --recursive site/themes/PaperMod); then
        echo "error: failed to initialise PaperMod theme submodule. Run from the repo root: git submodule update --init --recursive" >&2
        exit 1
    fi

    if [[ ! -f "$THEME_MARKER" ]]; then
        echo "error: PaperMod theme still missing after submodule init. Run: git submodule update --init --recursive" >&2
        exit 1
    fi
}

generate_hugo_content() {
    echo "Generating Hugo content from canon JSON..."
    if ! (cd "$REPO_ROOT" && dotnet run --project src/Turpinverse.Tools.GenerateHugoContent); then
        exit 1
    fi
}

hugo_build() {
    if ! "$RUNTIME" run --rm \
        -v "$(volume_mount "$REPO_ROOT" /src)" \
        -w /src/site \
        "$HUGO_IMAGE" \
        hugo --minify; then
        echo "error: Hugo build failed. Fix errors above before previewing." >&2
        exit 1
    fi

    if [[ ! -f "$PUBLIC_PATH/index.html" ]]; then
        echo "error: Hugo did not produce site/public/index.html. Run content generation first: dotnet run --project src/Turpinverse.Tools.GenerateHugoContent" >&2
        exit 1
    fi
}

if [[ ! -d "$SITE_DIR" ]]; then
    echo "error: Hugo site directory not found at $SITE_DIR" >&2
    exit 1
fi

resolve_runtime
initialize_hugo_theme_submodule

case "$COMMAND" in
    build)
        generate_hugo_content
        echo "Building Hugo site to site/public..."
        hugo_build
        echo "Done. Output: $PUBLIC_PATH"
        ;;
    serve)
        generate_hugo_content
        echo "Starting Hugo dev server at http://localhost:${SERVE_PORT} ..."
        "$RUNTIME" run --rm -p "${SERVE_PORT}:1313" \
            -v "$(volume_mount "$REPO_ROOT" /src)" \
            -w /src/site \
            "$HUGO_IMAGE" \
            hugo server --bind 0.0.0.0 --baseURL "http://localhost:${SERVE_PORT}"
        ;;
    preview)
        generate_hugo_content
        echo "Building Hugo site..."
        hugo_build
        echo "Serving site/public at http://localhost:${PREVIEW_PORT} ..."
        "$RUNTIME" run --rm -p "${PREVIEW_PORT}:80" \
            -v "$(volume_mount "$REPO_ROOT/site/public" /usr/share/nginx/html ro)" \
            "$NGINX_IMAGE"
        ;;
esac
