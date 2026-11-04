$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$iconRoot = Split-Path $PSScriptRoot -Parent
$image = [System.Drawing.Image]::FromFile((Join-Path $iconRoot 'assets\app-icon.png'))
$sizes = @(16, 24, 32, 48, 64, 128, 256)
$frames = @()
try {
    foreach ($size in $sizes) {
        $bitmap = New-Object System.Drawing.Bitmap($size, $size)
        $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
        $frameStream = New-Object System.IO.MemoryStream
        try {
            $graphics.Clear([System.Drawing.Color]::Transparent)
            $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
            $graphics.DrawImage($image, 0, 0, $size, $size)
            $bitmap.Save($frameStream, [System.Drawing.Imaging.ImageFormat]::Png)
            $frames += ,$frameStream.ToArray()
        } finally { $graphics.Dispose(); $bitmap.Dispose(); $frameStream.Dispose() }
    }
    $output = [System.IO.File]::Create((Join-Path $iconRoot 'assets\app.ico'))
    $writer = New-Object System.IO.BinaryWriter($output)
    try {
        $writer.Write([uint16]0)
        $writer.Write([uint16]1)
        $writer.Write([uint16]$sizes.Count)
        $offset = 6 + 16 * $sizes.Count
        for ($index = 0; $index -lt $sizes.Count; $index++) {
            $dimension = if ($sizes[$index] -eq 256) { 0 } else { $sizes[$index] }
            $writer.Write([byte]$dimension)
            $writer.Write([byte]$dimension)
            $writer.Write([uint16]0)
            $writer.Write([uint16]1)
            $writer.Write([uint16]32)
            $writer.Write([uint32]$frames[$index].Length)
            $writer.Write([uint32]$offset)
            $offset += $frames[$index].Length
        }
        foreach ($frame in $frames) { $writer.Write([byte[]]$frame) }
    } finally { $writer.Dispose(); $output.Dispose() }
} finally { $image.Dispose() }
