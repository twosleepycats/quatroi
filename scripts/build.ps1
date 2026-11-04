$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
Push-Location $projectRoot
try {
    New-Item -ItemType Directory -Force dist | Out-Null
    & (Join-Path $PSScriptRoot 'build-icon.ps1')
    $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
    & $compiler /nologo /target:winexe /optimize+ /platform:anycpu /win32icon:assets\app.ico /out:dist\QuatRoi.exe /reference:System.Drawing.dll /reference:System.Windows.Forms.dll /resource:assets\app.ico,app.ico /resource:assets\whip-atlas.png,whip-atlas.png /resource:assets\sword-atlas.png,sword-atlas.png /resource:assets\hammer-atlas.png,hammer-atlas.png /resource:assets\whip-crack.wav,whip-crack.wav src\Program.cs src\LayeredWindow.cs src\Weapon.cs
    if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
    Write-Output 'Built dist\QuatRoi.exe'
} finally { Pop-Location }
