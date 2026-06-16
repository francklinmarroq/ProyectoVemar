$viewDir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\Views"

Get-ChildItem $viewDir -Filter "*.xaml" | ForEach-Object {
    $lines = Get-Content $_.FullName -Encoding UTF8
    $result = [System.Collections.Generic.List[string]]::new()
    $skipOrphaned = $false
    $foundDataGridBorder = $false

    # Strategy: after the Grid with RowDefinitions closes, remove any orphaned </StackPanel> and </Border>
    # We detect when we hit the first </Grid> (root grid close candidate) and then filter orphaned tags

    # Simpler: just filter out any line that is ONLY </StackPanel> or </Border> that appears
    # AFTER the DataGrid section (Grid.Row="2") and BEFORE the final </Grid>

    # Find the last correct </Grid> line index
    $lastGridLine = -1
    for ($j = $lines.Count - 1; $j -ge 0; $j--) {
        if ($lines[$j].Trim() -eq '</Grid>') {
            $lastGridLine = $j
            break
        }
    }

    # Find the second-to-last </Grid> line (if any orphaned structure inserted one)
    $secondLastGridLine = -1
    for ($j = $lastGridLine - 1; $j -ge 0; $j--) {
        if ($lines[$j].Trim() -eq '</Grid>') {
            $secondLastGridLine = $j
            break
        }
    }

    if ($secondLastGridLine -ge 0) {
        # There are two </Grid> tags near the end: the first one was incorrectly placed
        # Remove lines from $secondLastGridLine+1 to $lastGridLine-1 (the orphaned ones)
        # AND fix line $secondLastGridLine to become the correct closing position
        $cleanLines = [System.Collections.Generic.List[string]]::new()
        for ($j = 0; $j -lt $secondLastGridLine; $j++) {
            $cleanLines.Add($lines[$j])
        }
        # Add the correct root Grid close
        $cleanLines.Add($lines[$lastGridLine])
        # Add </UserControl>
        for ($j = $lastGridLine + 1; $j -lt $lines.Count; $j++) {
            if ($lines[$j].Trim() -ne '') {
                $cleanLines.Add($lines[$j])
            }
        }
        Set-Content $_.FullName -Value $cleanLines -Encoding UTF8
        Write-Host "Fixed orphaned tags in: $($_.Name)"
    } else {
        Write-Host "No fix needed: $($_.Name)"
    }
}
Write-Host "Done."
