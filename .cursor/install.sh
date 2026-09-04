#!/usr/bin/env bash
# Idempotent Cloud Agent bootstrap for the Turpinverse repo.
# Installs the toolchains that are not part of the base image (.NET SDK, Go,
# Hugo Extended, PowerShell), then restores/builds repository dependencies.
# Safe to run repeatedly: every tool install is guarded by a version check.
set -euo pipefail

# Pinned versions (keep in sync with global.json and .github/workflows).
DOTNET_VERSION="10.0.300"
GO_VERSION="1.26.5"
HUGO_VERSION="0.165.0"
POWERSHELL_VERSION="7.5.4"

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ARCH="$(uname -m)"

log() { printf '\n\033[1;34m==> %s\033[0m\n' "$*"; }

# --- System packages -------------------------------------------------------
log "Installing base system packages"
export DEBIAN_FRONTEND=noninteractive
sudo apt-get update -y
sudo apt-get install -y --no-install-recommends \
  curl ca-certificates tar gzip git shellcheck libicu74 libssl3

# --- .NET SDK --------------------------------------------------------------
if ! command -v dotnet >/dev/null 2>&1 || [[ "$(dotnet --version 2>/dev/null || true)" != "${DOTNET_VERSION}" ]]; then
  log "Installing .NET SDK ${DOTNET_VERSION}"
  curl -sSfL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  chmod +x /tmp/dotnet-install.sh
  sudo mkdir -p /usr/local/dotnet
  sudo /tmp/dotnet-install.sh --version "${DOTNET_VERSION}" --install-dir /usr/local/dotnet --no-path
  sudo ln -sf /usr/local/dotnet/dotnet /usr/local/bin/dotnet
else
  log ".NET SDK ${DOTNET_VERSION} already present"
fi
export DOTNET_ROOT=/usr/local/dotnet
export PATH="/usr/local/dotnet:${PATH}"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

# --- Go (required by Hugo modules) -----------------------------------------
if ! /usr/local/go/bin/go version 2>/dev/null | grep -q "go${GO_VERSION}"; then
  log "Installing Go ${GO_VERSION}"
  case "${ARCH}" in
    x86_64) GO_ARCH="amd64" ;;
    aarch64|arm64) GO_ARCH="arm64" ;;
    *) echo "Unsupported arch: ${ARCH}" >&2; exit 1 ;;
  esac
  curl -sSfL "https://go.dev/dl/go${GO_VERSION}.linux-${GO_ARCH}.tar.gz" -o /tmp/go.tar.gz
  sudo rm -rf /usr/local/go
  sudo tar -C /usr/local -xzf /tmp/go.tar.gz
  sudo ln -sf /usr/local/go/bin/go /usr/local/bin/go
  sudo ln -sf /usr/local/go/bin/gofmt /usr/local/bin/gofmt
else
  log "Go ${GO_VERSION} already present"
fi

# --- Hugo Extended ---------------------------------------------------------
if ! command -v hugo >/dev/null 2>&1 || ! hugo version 2>/dev/null | grep -q "v${HUGO_VERSION}"; then
  log "Installing Hugo Extended ${HUGO_VERSION}"
  case "${ARCH}" in
    x86_64) HUGO_ARCH="linux-amd64" ;;
    aarch64|arm64) HUGO_ARCH="linux-arm64" ;;
    *) echo "Unsupported arch: ${ARCH}" >&2; exit 1 ;;
  esac
  curl -sSfL "https://github.com/gohugoio/hugo/releases/download/v${HUGO_VERSION}/hugo_extended_${HUGO_VERSION}_${HUGO_ARCH}.tar.gz" -o /tmp/hugo.tar.gz
  sudo tar -C /usr/local/bin -xzf /tmp/hugo.tar.gz hugo
else
  log "Hugo Extended ${HUGO_VERSION} already present"
fi

# --- PowerShell ------------------------------------------------------------
if ! command -v pwsh >/dev/null 2>&1 || ! pwsh --version 2>/dev/null | grep -q "${POWERSHELL_VERSION}"; then
  log "Installing PowerShell ${POWERSHELL_VERSION}"
  case "${ARCH}" in
    x86_64) PWSH_ARCH="x64" ;;
    aarch64|arm64) PWSH_ARCH="arm64" ;;
    *) echo "Unsupported arch: ${ARCH}" >&2; exit 1 ;;
  esac
  curl -sSfL "https://github.com/PowerShell/PowerShell/releases/download/v${POWERSHELL_VERSION}/powershell-${POWERSHELL_VERSION}-linux-${PWSH_ARCH}.tar.gz" -o /tmp/powershell.tar.gz
  sudo mkdir -p /opt/microsoft/powershell/7
  sudo tar -C /opt/microsoft/powershell/7 -xzf /tmp/powershell.tar.gz
  sudo chmod +x /opt/microsoft/powershell/7/pwsh
  sudo ln -sf /opt/microsoft/powershell/7/pwsh /usr/local/bin/pwsh
else
  log "PowerShell ${POWERSHELL_VERSION} already present"
fi

# --- PSScriptAnalyzer (used by scripts/ lint) ------------------------------
if ! pwsh -NoProfile -Command "Get-Module -ListAvailable PSScriptAnalyzer" 2>/dev/null | grep -q PSScriptAnalyzer; then
  log "Installing PSScriptAnalyzer module"
  pwsh -NoProfile -Command "Set-PSRepository -Name PSGallery -InstallationPolicy Trusted; Install-Module -Name PSScriptAnalyzer -Force -Scope CurrentUser -Repository PSGallery" || true
fi

# --- Repository dependencies ----------------------------------------------
log "Restoring .NET solution"
dotnet restore "${REPO_ROOT}/Turpinverse.slnx"

log "Installing npm dependencies for Turpinverse.Web"
( cd "${REPO_ROOT}/src/Turpinverse.Web" && npm ci )

log "Pre-fetching Hugo Go modules"
( cd "${REPO_ROOT}/site" && go mod download )

log "Tool versions"
dotnet --version
go version
hugo version
pwsh --version

log "Bootstrap complete"
