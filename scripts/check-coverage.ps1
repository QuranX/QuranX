#requires -Version 7

<#
.SYNOPSIS
    Verifies that the merged Cobertura coverage report meets the configured minimum thresholds.
.DESCRIPTION
    Reads TestResults/CoverageReport/Cobertura.xml (produced by reportgenerator) and fails the
    build with a non-zero exit code if line or branch coverage falls below the gate.

    Run after the standard coverage pipeline:
        dotnet test ... --collect:"XPlat Code Coverage" --settings coverlet.runsettings
        dotnet reportgenerator -reports:TestResults/**/coverage.cobertura.xml ...
        ./scripts/check-coverage.ps1
.PARAMETER ReportPath
    Path to the merged Cobertura.xml file. Defaults to TestResults/CoverageReport/Cobertura.xml.
.PARAMETER MinimumLineCoverage
    Minimum acceptable line-rate as a fraction (0.0 - 1.0). Defaults to 0.50.
.PARAMETER MinimumBranchCoverage
    Minimum acceptable branch-rate as a fraction (0.0 - 1.0). Defaults to 0.40.
#>

param(
    [string]$ReportPath = "TestResults/CoverageReport/Cobertura.xml",
    [double]$MinimumLineCoverage = 0.50,
    [double]$MinimumBranchCoverage = 0.40
)

if (-not (Test-Path $ReportPath)) {
    Write-Error "Coverage report not found at $ReportPath. Run reportgenerator first."
    exit 2
}

[xml]$report = Get-Content -Path $ReportPath
$lineRate = [double]$report.coverage.'line-rate'
$branchRate = [double]$report.coverage.'branch-rate'

$linePercent = [math]::Round($lineRate * 100, 1)
$branchPercent = [math]::Round($branchRate * 100, 1)
$lineGate = [math]::Round($MinimumLineCoverage * 100, 1)
$branchGate = [math]::Round($MinimumBranchCoverage * 100, 1)

Write-Host "QuranX coverage gate"
Write-Host "  Line coverage:   $linePercent% (gate: $lineGate%)"
Write-Host "  Branch coverage: $branchPercent% (gate: $branchGate%)"

$failed = $false
if ($lineRate -lt $MinimumLineCoverage) {
    Write-Host "FAIL: line coverage $linePercent% is below the $lineGate% gate." -ForegroundColor Red
    $failed = $true
}
if ($branchRate -lt $MinimumBranchCoverage) {
    Write-Host "FAIL: branch coverage $branchPercent% is below the $branchGate% gate." -ForegroundColor Red
    $failed = $true
}

if ($failed) {
    exit 1
}

Write-Host "PASS: coverage thresholds met." -ForegroundColor Green
exit 0
