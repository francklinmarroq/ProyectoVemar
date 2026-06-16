$viewDir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\Views"

Get-ChildItem $viewDir -Filter "*.xaml" | ForEach-Object {
    $c = Get-Content $_.FullName -Raw -Encoding UTF8

    $marker = '<Border Grid.Row="3"'
    $markerIdx = $c.IndexOf($marker)
    if ($markerIdx -lt 0) { return }

    # Find the closing </Grid> after the marker
    $gridClose = '    </Grid>'
    $gridIdx = $c.IndexOf($gridClose, $markerIdx)
    if ($gridIdx -lt 0) {
        # Try without leading spaces
        $gridClose = '</Grid>'
        $gridIdx = $c.IndexOf($gridClose, $markerIdx)
    }

    if ($gridIdx -lt 0) {
        Write-Host "Could not find </Grid> in: $($_.Name)"
        return
    }

    # Remove everything from marker to gridIdx (keep </Grid> and beyond)
    $before = $c.Substring(0, $markerIdx).TrimEnd()
    $after = $c.Substring($gridIdx)
    $c = $before + "`r`n" + $after

    Set-Content $_.FullName -Value $c -Encoding UTF8
    Write-Host "Cleaned: $($_.Name)"
}
Write-Host "Done."
