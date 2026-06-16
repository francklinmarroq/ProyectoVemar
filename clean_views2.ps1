$viewDir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\Views"

Get-ChildItem $viewDir -Filter "*.xaml" | ForEach-Object {
    $c = Get-Content $_.FullName -Raw -Encoding UTF8

    $marker = '<Border Grid.Row="3"'
    $markerIdx = $c.IndexOf($marker)
    if ($markerIdx -lt 0) { return }

    # Find the matching </Border> by counting proper opening/closing Border tags
    $i = $markerIdx
    $depth = 0
    $endIdx = -1

    while ($i -lt $c.Length - 1) {
        # Check for opening Border (must be <Border followed by space, >, or newline, NOT a dot)
        if ($i + 7 -le $c.Length) {
            $sub7 = $c.Substring($i, 7)
            $nextChar = if ($i + 7 -lt $c.Length) { $c[$i + 7] } else { ' ' }
            if ($sub7 -eq '<Border' -and ($nextChar -eq ' ' -or $nextChar -eq '>' -or $nextChar -eq "`r" -or $nextChar -eq "`n")) {
                $depth++
                $i += 7
                continue
            }
        }

        # Check for closing </Border>
        if ($i + 9 -le $c.Length -and $c.Substring($i, 9) -eq '</Border>') {
            $depth--
            if ($depth -eq 0) {
                $endIdx = $i + 9
                break
            }
            $i += 9
            continue
        }

        $i++
    }

    if ($endIdx -ge 0) {
        # Remove optional trailing whitespace/newlines before the cut point
        $before = $c.Substring(0, $markerIdx).TrimEnd()
        $after = $c.Substring($endIdx)
        $c = $before + "`r`n" + $after
        Set-Content $_.FullName -Value $c -Encoding UTF8
        Write-Host "Cleaned form panel from: $($_.Name)"
    } else {
        Write-Host "Could not find closing Border in: $($_.Name)"
    }
}
Write-Host "Done."
