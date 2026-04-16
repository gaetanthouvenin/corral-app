# create-msix.ps1
# Crée un package MSIX pour Corral en utilisant makeappx.exe (Windows SDK)
#
# Usage:
#   .\tools\create-msix.ps1
#   .\tools\create-msix.ps1 -Version 1.2.3
#   .\tools\create-msix.ps1 -Version 1.2.3 -PackageName "12345GaetanThouvenin.Corral" -Publisher "CN=XXXX"
#
# Prérequis: Windows SDK installé (makeappx.exe)

[CmdletBinding()]
param(
  [string]$Version     = "1.0.0",
  [string]$OutputDir   = "$PSScriptRoot\..\artifacts",
  [string]$ProjectDir  = "$PSScriptRoot\..\src\Corral.Desktop",
  # Valeurs fournies par Partner Center — facultatives (utilise les valeurs du manifeste si absent)
  [string]$PackageName = "",
  [string]$Publisher   = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# ─── Fonctions utilitaires ───────────────────────────────────────────────────

function Find-MakeAppx {
  $kitsBin = "C:\Program Files (x86)\Windows Kits\10\bin"
  $candidates = Get-ChildItem -Path $kitsBin -Filter "makeappx.exe" -Recurse |
    Where-Object { $_.FullName -like "*x64*" } |
    Sort-Object { [version]($_.DirectoryName -split '\\' | Where-Object { $_ -match '^\d+\.\d+\.\d+\.\d+$' }) } -Descending
  if (-not $candidates) {
    throw "makeappx.exe introuvable. Installez le Windows SDK : https://developer.microsoft.com/windows/downloads/windows-sdk/"
  }
  return $candidates[0].FullName
}

# ─── 1. Résolution des chemins ───────────────────────────────────────────────

$makeAppx = Find-MakeAppx
Write-Host "makeappx : $makeAppx"

$publishDir = Join-Path $PSScriptRoot "..\artifacts\publish"
$stagingDir = Join-Path $PSScriptRoot "..\artifacts\msix-staging"
$msixPath   = Join-Path $OutputDir "Corral-$Version.msix"

# Nettoyage
Remove-Item $publishDir  -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item $stagingDir  -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Force -Path $publishDir  | Out-Null
New-Item -ItemType Directory -Force -Path $stagingDir  | Out-Null
New-Item -ItemType Directory -Force -Path $OutputDir   | Out-Null

# ─── 2. Build + Publish (self-contained, win-x64) ───────────────────────────

Write-Host ""
Write-Host "Publication de l'application..."
$csproj = Join-Path $ProjectDir "Corral.Desktop.csproj"
& dotnet publish $csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -o $publishDir `
  /p:Version=$Version `
  /p:AssemblyVersion="$Version.0" `
  /p:FileVersion="$Version.0" `
  /p:DebugType=None `
  /p:DebugSymbols=false

if ($LASTEXITCODE -ne 0) { throw "dotnet publish a échoué (exit code $LASTEXITCODE)" }

# ─── 3. Staging : binaires ──────────────────────────────────────────────────

Write-Host ""
Write-Host "Préparation du staging..."
Copy-Item "$publishDir\*" $stagingDir -Recurse -Force

# ─── 4. Staging : assets MSIX ───────────────────────────────────────────────

$msixAssetsDir = Join-Path $stagingDir "Assets"
New-Item -ItemType Directory -Force -Path $msixAssetsDir | Out-Null

$sourceAssets = Join-Path $ProjectDir "Assets\Msix"
if (-not (Test-Path $sourceAssets)) {
  Write-Host "  Assets MSIX manquants — génération depuis l'icône..."
  & "$PSScriptRoot\generate-msix-assets.ps1" -OutputDir $sourceAssets
}
Copy-Item "$sourceAssets\*" $msixAssetsDir -Force

# ─── 5. Staging : AppxManifest ──────────────────────────────────────────────

$manifestSrc = Join-Path $ProjectDir "Package.appxmanifest"
$manifestDst = Join-Path $stagingDir "AppxManifest.xml"

# Injecter la version dans le manifeste
$msixVersion = "$Version.0"
$content = (Get-Content $manifestSrc -Encoding UTF8) -replace 'Version="\d+\.\d+\.\d+\.\d+"', "Version=`"$msixVersion`""

# Injecter l'identité Store si fournie
if ($PackageName -ne "") {
  $content = $content -replace 'Name="[^"]+"', "Name=`"$PackageName`""
}
if ($Publisher -ne "") {
  $content = $content -replace 'Publisher="[^"]+"', "Publisher=`"$Publisher`""
}

$content | Set-Content $manifestDst -Encoding UTF8
Write-Host "  Manifeste : $manifestDst (v$msixVersion)"

# ─── 6. Packaging makeappx ──────────────────────────────────────────────────

Write-Host ""
Write-Host "Création du package MSIX..."
& $makeAppx pack /d $stagingDir /p $msixPath /o /nv
if ($LASTEXITCODE -ne 0) { throw "makeappx a échoué (exit code $LASTEXITCODE)" }

# ─── 7. Résultat ────────────────────────────────────────────────────────────

$size = (Get-Item $msixPath).Length / 1MB
Write-Host ""
Write-Host "Package créé : $msixPath ($([Math]::Round($size, 1)) MB)"
Write-Host ""
Write-Host "Pour INSTALLER sur cette machine (certificat auto-signé requis) :"
Write-Host "  Add-AppxPackage '$msixPath'"
Write-Host ""
Write-Host "Note : sans signature, l'utilisateur doit activer"
Write-Host "  'Sideloading apps' dans les paramètres Windows."
