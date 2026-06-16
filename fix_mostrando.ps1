$dir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\ViewModels"

Get-ChildItem $dir -Filter '*ViewModel.vb' | ForEach-Object {
    $lines = Get-Content $_.FullName -Encoding UTF8
    $result = [System.Collections.Generic.List[string]]::new()
    $skip = $false
    $skipUntil = ""

    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]

        # Skip backing field
        if ($line -match '^\s+Private _mostrandoFormulario As Boolean') {
            continue
        }

        # Detect start of MostrandoFormulario property
        if ($line -match '^\s+Public Property MostrandoFormulario As Boolean') {
            $skip = $true
        }

        if ($skip) {
            # Look for End Property to stop skipping
            if ($line -match '^\s+End Property') {
                $skip = $false
                continue
            }
            continue
        }

        $result.Add($line)
    }

    Set-Content $_.FullName -Value $result -Encoding UTF8
    $removed = $lines.Count - $result.Count
    if ($removed -gt 0) {
        Write-Host "Cleaned $removed lines from $($_.Name)"
    }
}
Write-Host "Done."
