# Build or preview the Hugo documentation site via Docker/Podman (no local Hugo install required).
# Usage: .\scripts\hugo.ps1 build|serve|preview

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
    param([string]$Name)
    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        Write-Error "Container runtime '$Name' not found. Install Podman Desktop or Docker Desktop, or pass -Runtime docker."
    }
}

function Invoke-HugoContentGenerator {
    Write-Host "Generating Hugo content from canon JSON..." -ForegroundColor Cyan
    Push-Location $repoRoot
    try {
        dotnet run --project tools/generate-hugo-content/Turpinverse.Tools.GenerateHugoContent
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }
    finally {
        Pop-Location
    }
}

function Invoke-HugoBuild {
    & $Runtime run --rm `
        -v "${rootForMount}:/src:Z" `
        -w /src/site `
        docker.io/hugomods/hugo:latest `
        hugo --minify
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Hugo build failed (exit $LASTEXITCODE). Fix errors above before previewing."
    }
    $docsPath = Join-Path $repoRoot "docs"
    if (-not (Test-Path (Join-Path $docsPath "index.html"))) {
        Write-Error "Hugo did not produce docs/index.html. Run content generation first: dotnet run --project tools/generate-hugo-content/Turpinverse.Tools.GenerateHugoContent"
    }
}

Test-ContainerRuntime -Name $Runtime

switch ($Command) {
    "build" {
        Invoke-HugoContentGenerator
        Write-Host "Building Hugo site to docs/..." -ForegroundColor Cyan
        Invoke-HugoBuild
        Write-Host "Done. Output: $repoRoot\docs" -ForegroundColor Green
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
        Write-Host "Serving docs/ at http://localhost:$PreviewPort ..." -ForegroundColor Cyan
        & $Runtime run --rm -p "${PreviewPort}:80" `
            -v "${rootForMount}/docs:/usr/share/nginx/html:ro,Z" `
            docker.io/library/nginx:alpine
    }
}
