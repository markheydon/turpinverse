# Build or preview the Hugo documentation site via Docker/Podman (no local Hugo install required).
# Usage: .\scripts\Invoke-HugoSite.ps1 build|serve|preview

param(
    [Parameter(Position = 0)]
    [ValidateSet("build", "serve", "preview")]
    [string]$Command = "build",

    [string]$Runtime = "podman",
    [int]$ServePort = 1313,
    [int]$PreviewPort = 8080
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$rootForMount = $repoRoot -replace '\\', '/'

function Test-ContainerRuntime {
    <#
    .SYNOPSIS
        Verifies that the requested container runtime is available on PATH.
    #>
    param([string]$Name)
    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        Write-Error "Container runtime '$Name' not found. Install Podman Desktop or Docker Desktop, or pass -Runtime docker."
    }
}

function Initialize-HugoThemeSubmodule {
    <#
    .SYNOPSIS
        Ensures the PaperMod Hugo theme submodule is checked out before build/serve.
    #>
    $themeMarker = Join-Path $repoRoot "site/themes/PaperMod/layouts/_partials/head.html"
    if (Test-Path $themeMarker) {
        return
    }

    Write-Host "PaperMod theme submodule is missing; initialising..." -ForegroundColor Yellow
    if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
        Write-Error "PaperMod theme not found at site/themes/PaperMod. Install Git and run: git submodule update --init --recursive"
    }

    Push-Location $repoRoot
    try {
        git submodule update --init --recursive site/themes/PaperMod
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Failed to initialise PaperMod theme submodule. Run from the repo root: git submodule update --init --recursive"
        }
    }
    finally {
        Pop-Location
    }

    if (-not (Test-Path $themeMarker)) {
        Write-Error "PaperMod theme still missing after submodule init. Run: git submodule update --init --recursive"
    }
}

function Invoke-HugoContentGenerator {
    <#
    .SYNOPSIS
        Generates Hugo content from canon JSON via the .NET content generator tool.
    #>
    Write-Host "Generating Hugo content from canon JSON..." -ForegroundColor Cyan
    Push-Location $repoRoot
    try {
        dotnet run --project src/Turpinverse.Tools.GenerateHugoContent
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }
    finally {
        Pop-Location
    }
}

function Invoke-HugoBuild {
    <#
    .SYNOPSIS
        Builds the Hugo site into site/public using a containerized Hugo runtime.
    #>
    & $Runtime run --rm `
        -v "${rootForMount}:/src:Z" `
        -w /src/site `
        docker.io/hugomods/hugo:latest `
        hugo --minify
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Hugo build failed (exit $LASTEXITCODE). Fix errors above before previewing."
    }
    $publicPath = Join-Path $repoRoot "site/public"
    if (-not (Test-Path (Join-Path $publicPath "index.html"))) {
        Write-Error "Hugo did not produce site/public/index.html. Run content generation first: dotnet run --project src/Turpinverse.Tools.GenerateHugoContent"
    }
}

Test-ContainerRuntime -Name $Runtime
Initialize-HugoThemeSubmodule

switch ($Command) {
    "build" {
        Invoke-HugoContentGenerator
        Write-Host "Building Hugo site to site/public..." -ForegroundColor Cyan
        Invoke-HugoBuild
        Write-Host "Done. Output: $repoRoot\site\public" -ForegroundColor Green
    }

    "serve" {
        Invoke-HugoContentGenerator
        Write-Host "Starting Hugo dev server at http://localhost:$ServePort ..." -ForegroundColor Cyan
        & $Runtime run --rm -p "${ServePort}:1313" `
            -v "${rootForMount}:/src:Z" `
            -w /src/site `
            docker.io/hugomods/hugo:latest `
            hugo server --bind 0.0.0.0 --baseURL "http://localhost:$ServePort"
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }

    "preview" {
        Invoke-HugoContentGenerator
        Write-Host "Building Hugo site..." -ForegroundColor Cyan
        Invoke-HugoBuild
        Write-Host "Serving site/public at http://localhost:$PreviewPort ..." -ForegroundColor Cyan
        & $Runtime run --rm -p "${PreviewPort}:80" `
            -v "${rootForMount}/site/public:/usr/share/nginx/html:ro,Z" `
            docker.io/library/nginx:alpine
    }
}
