# create-selfsigned-cert.ps1
# Génère un certificat auto-signé pour signer le MSIX en local ou en CI.
#
# Usage:
#   .\tools\create-selfsigned-cert.ps1
#   .\tools\create-selfsigned-cert.ps1 -Publisher "CN=Gaëtan THOUVENIN" -Password "MonMotDePasse"
#
# Résultat :
#   artifacts/corral-signing.pfx       → à stocker comme secret GitHub (encodé Base64)
#   artifacts/corral-signing.cer       → à distribuer aux utilisateurs pour installer le certificat
#
# Pour encoder le PFX en Base64 (à coller dans le secret GitHub SIGNING_CERT_PFX_BASE64) :
#   [Convert]::ToBase64String([IO.File]::ReadAllBytes("artifacts\corral-signing.pfx"))
#
# IMPORTANT : un certificat auto-signé affiche un avertissement SmartScreen à l'installation.
# Les utilisateurs devront installer le .cer dans "Autorités de certification racines de confiance"
# avant d'installer le .msix. Un certificat Code Signing acheté (DigiCert, Sectigo...) évite ça.

[CmdletBinding()]
param(
  [string]$Publisher = "CN=Gaëtan THOUVENIN",
  [string]$Password  = "CorralSigning2026",
  [string]$OutputDir = "$PSScriptRoot\..\artifacts"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

$pfxPath = Join-Path $OutputDir "corral-signing.pfx"
$cerPath = Join-Path $OutputDir "corral-signing.cer"

# Générer le certificat auto-signé (valable 5 ans)
$cert = New-SelfSignedCertificate `
  -Subject $Publisher `
  -Type CodeSigningCert `
  -CertStoreLocation "Cert:\CurrentUser\My" `
  -NotAfter (Get-Date).AddYears(5) `
  -HashAlgorithm SHA256 `
  -KeyUsage DigitalSignature

Write-Host "Certificat créé : $($cert.Thumbprint)"

# Exporter le PFX (clé privée + certificat)
$securePassword = ConvertTo-SecureString $Password -AsPlainText -Force
Export-PfxCertificate -Cert $cert -FilePath $pfxPath -Password $securePassword | Out-Null

# Exporter le CER (certificat public seul, à distribuer aux utilisateurs)
Export-Certificate -Cert $cert -FilePath $cerPath | Out-Null

# Supprimer du store (nettoyage)
Remove-Item "Cert:\CurrentUser\My\$($cert.Thumbprint)" -Force

Write-Host ""
Write-Host "Fichiers générés dans : $OutputDir"
Write-Host "  corral-signing.pfx  → secret GitHub SIGNING_CERT_PFX_BASE64"
Write-Host "  corral-signing.cer  → à distribuer aux utilisateurs finaux"
Write-Host ""
Write-Host "Encodage Base64 du PFX (à copier dans le secret GitHub) :"
$base64 = [Convert]::ToBase64String([IO.File]::ReadAllBytes($pfxPath))
Write-Host $base64
Write-Host ""
Write-Host "Mot de passe du certificat (secret GitHub SIGNING_CERT_PASSWORD) : $Password"
Write-Host ""
Write-Host "Instructions d'installation pour les utilisateurs :"
Write-Host "  1. Double-cliquer sur corral-signing.cer"
Write-Host "  2. 'Installer le certificat' → 'Ordinateur local' → 'Autorités de certification racines de confiance'"
Write-Host "  3. Installer le .msix normalement"
