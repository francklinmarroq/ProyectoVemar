$files = @("ColaboradoresView.xaml","GastoRemedidasView.xaml","ProyectosView.xaml","TramitesView.xaml")
$dir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\Views"

foreach ($f in $files) {
    $lines = [System.Collections.Generic.List[string]](Get-Content "$dir\$f" -Encoding UTF8)
    $fixed = $false

    # Find the pattern: line with "    </Grid>" immediately followed by "        </Grid>"
    for ($i = 0; $i -lt $lines.Count - 1; $i++) {
        if ($lines[$i] -eq '    </Grid>' -and $lines[$i+1] -eq '        </Grid>') {
            # Replace both with just "        </Grid>" (header Grid close)
            $lines[$i] = '        </Grid>'
            $lines.RemoveAt($i + 1)
            $fixed = $true
            Write-Host "Fixed pattern at line $i in: $f"
            break
        }
    }

    if (-not $fixed) {
        Write-Host "Pattern NOT found in: $f (checking manually)"
        $lines | Select-Object -Skip ([Math]::Max(0, $lines.Count - 10)) | ForEach-Object { Write-Host "  '$_'" }
    }

    Set-Content "$dir\$f" -Value $lines -Encoding UTF8
}
Write-Host "Done."
