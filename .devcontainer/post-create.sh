#!/usr/bin/env bash
# Dev container bootstrap after features are installed.
# Pin Hugo with .github/workflows/hugo-build.yml and .cursor/install.sh (Go via devcontainer feature).
set -euo pipefail

HUGO_VERSION="0.165.0"

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

dotnet restore "${REPO_ROOT}/Turpinverse.slnx"

(
  cd "${REPO_ROOT}/src/Turpinverse.Web"
  npm ci
)

(
  cd "${REPO_ROOT}/site"
  go mod download
)

if ! command -v hugo >/dev/null 2>&1 || ! hugo version 2>/dev/null | grep -q "v${HUGO_VERSION}"; then
  ARCH="$(uname -m)"
  case "${ARCH}" in
    x86_64) HUGO_ARCH="linux-amd64" ;;
    aarch64|arm64) HUGO_ARCH="linux-arm64" ;;
    *)
      echo "Unsupported arch for Hugo install: ${ARCH}" >&2
      exit 1
      ;;
  esac
  curl -sSfL \
    "https://github.com/gohugoio/hugo/releases/download/v${HUGO_VERSION}/hugo_extended_${HUGO_VERSION}_${HUGO_ARCH}.tar.gz" \
    -o /tmp/hugo.tar.gz
  sudo tar -C /usr/local/bin -xzf /tmp/hugo.tar.gz hugo
  rm -f /tmp/hugo.tar.gz
fi

if ! pwsh -NoProfile -Command "Get-Module -ListAvailable PSScriptAnalyzer" 2>/dev/null | grep -q PSScriptAnalyzer; then
  pwsh -NoProfile -Command \
    "Set-PSRepository -Name PSGallery -InstallationPolicy Trusted; Install-Module -Name PSScriptAnalyzer -Force -Scope CurrentUser -Repository PSGallery"
fi
