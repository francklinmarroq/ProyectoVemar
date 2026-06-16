$viewDir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\Views"

Get-ChildItem $viewDir -Filter "*.xaml" | ForEach-Object {
    $c = Get-Content $_.FullName -Raw -Encoding UTF8

    $marker = '<Border Grid.Row="3"'
    $markerIdx = $c.IndexOf($marker)
    if ($markerIdx -lt 0) { return }

    # Use the LAST </Grid> in the file - that's always the root Grid closing
    $gridClose = '</Grid>'
    $gridIdx = $c.LastIndexOf($gridClose)

    if ($gridIdx -lt 0 -or $gridIdx -lt $markerIdx) {
        Write-Host "Could not find closing </Grid> after marker in: $($_.Name)"
        return
    }

    # Remove everything from marker to gridIdx (keep </Grid> and beyond)
    $before = $c.Substring(0, $markerIdx).TrimEnd()
    $after = $c.Substring($gridIdx)
    $c = $before + "`r`n" + $after

    # Also remove the last RowDefinition (Auto) from the RowDefinitions if still present
    $c = $c -replace '<RowDefinition Height="Auto"/><RowDefinition Height="12"/><RowDefinition Height="\*"/><RowDefinition Height="Auto"/>',
                     '<RowDefinition Height="Auto"/><RowDefinition Height="12"/><RowDefinition Height="*"/>'

    Set-Content $_.FullName -Value $c -Encoding UTF8
    Write-Host "Cleaned: $($_.Name)"
}
Write-Host "Done."
