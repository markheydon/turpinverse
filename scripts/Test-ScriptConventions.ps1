# Validates PowerShell script filenames in scripts/ follow Verb-Noun.ps1 with approved verbs.
# Usage: ./scripts/Test-ScriptConventions.ps1

$ErrorActionPreference = 'Stop'

$scriptsDir = $PSScriptRoot
$approvedVerbs = [System.Collections.Generic.HashSet[string]]::new(
    [string[]](Get-Verb).Verb,
    [System.StringComparer]::OrdinalIgnoreCase
)

$verbNounPattern = '^[A-Z][a-z]+(?:[A-Z][a-z]+)*-[A-Z][a-z]+(?:[A-Z][a-z]+)*$'
$failures = @()

Get-ChildItem -Path $scriptsDir -Filter '*.ps1' -File |
    Where-Object { $_.Name -ne 'Test-ScriptConventions.ps1' } |
    ForEach-Object {
        $baseName = $_.BaseName

        if ($baseName -notmatch $verbNounPattern) {
            $failures += "Script '$($_.Name)' must be named Verb-Noun.ps1 (PascalCase, hyphen-separated)."
            return
        }

        $verb = $baseName.Split('-')[0]
        if (-not $approvedVerbs.Contains($verb)) {
            $failures += "Script '$($_.Name)' uses unapproved verb '$verb'. See Get-Verb for approved verbs."
        }
    }

if ($failures.Count -gt 0) {
    $failures | ForEach-Object { Write-Error $_ }
    exit 1
}

Write-Host "All script filenames follow Verb-Noun conventions with approved verbs." -ForegroundColor Green
