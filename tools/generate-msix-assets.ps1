# generate-msix-assets.ps1
# Genere les PNG requis par MSIX a partir de l'icone Haute Definition app_icon_hd.png
# Prerequis: aucun (utilise System.Drawing integre)
# Usage: .\tools\generate-msix-assets.ps1

[CmdletBinding()]
param(
  [string]$SrcIcoPath = "$PSScriptRoot\..\src\Corral.Desktop\Assets\app_icon_hd.png",
  [string]$OutputDir = "$PSScriptRoot\..\src\Corral.Desktop\Assets\Msix"
)

Add-Type -AssemblyName System.Drawing

if (-not (Test-Path $SrcIcoPath)) {
  Write-Error "Image introuvable : $SrcIcoPath"
  exit 1
}

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

# Tailles requises par le manifeste MSIX (Scale = 1.0 pour remplir au max la dimension courte)
$assets = @(
  @{ Name = "Square44x44Logo.png";   Width = 44;  Height = 44;  Scale = 1.0 },
  @{ Name = "Square150x150Logo.png"; Width = 150; Height = 150; Scale = 1.0 },
  @{ Name = "Wide310x150Logo.png";   Width = 310; Height = 150; Scale = 1.0 },
  @{ Name = "StoreLogo.png";         Width = 50;  Height = 50;  Scale = 1.0 },
  @{ Name = "SplashScreen.png";      Width = 620; Height = 300; Scale = 0.8 } # leger padding vertical pour le splash
)

$srcBitmap = [System.Drawing.Bitmap]::FromFile($SrcIcoPath)

foreach ($asset in $assets) {
  $destPath = Join-Path $OutputDir $asset.Name
  $bitmap = New-Object System.Drawing.Bitmap($asset.Width, $asset.Height)
  $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
  $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
  $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
  $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
  $graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality

  # Fond de la base
  $graphics.Clear([System.Drawing.Color]::Transparent)
  
  # Si l'image est un carre complet on peut vouloir peindre le fond en noir pour relier 
  # ou juste transparent (l'OS / store gère la bordure invisible). 
  # On va garder transparent.

  # Centrer l'icone avec le scaling defini par rapport a la plus petite dimension
  $minDim = [Math]::Min($asset.Width, $asset.Height)
  $iconSize = [int]($minDim * $asset.Scale)
  $x = [int](($asset.Width - $iconSize) / 2)
  $y = [int](($asset.Height - $iconSize) / 2)
  $destRect = New-Object System.Drawing.Rectangle($x, $y, $iconSize, $iconSize)
  
  $graphics.DrawImage($srcBitmap, $destRect)

  $bitmap.Save($destPath, [System.Drawing.Imaging.ImageFormat]::Png)
  $graphics.Dispose()
  $bitmap.Dispose()

  Write-Host "  + $($asset.Name) ($($asset.Width)x$($asset.Height))"
}

$srcBitmap.Dispose()

Write-Host ""
Write-Host "Assets MSIX generes dans : $OutputDir"
