#!/usr/bin/env bash
# Dev container bootstrap after features are installed.
# Hugo and Go versions are pinned in devcontainer.json features (align with .github/workflows/hugo-build.yml).
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

dotnet restore "${REPO_ROOT}/Turpinverse.slnx"

(
  cd "${REPO_ROOT}/site"
  go mod download
)

# Docker-based hugo invocations may have left root-owned artifacts before invoke-hugo-site.sh mapped container UID.
if [[ -e "${REPO_ROOT}/site/.hugo_build.lock" ]] && [[ ! -w "${REPO_ROOT}/site/.hugo_build.lock" ]]; then
  sudo chown -R "$(id -u):$(id -g)" "${REPO_ROOT}/site/.hugo_build.lock" "${REPO_ROOT}/site/public" 2>/dev/null || true
fi