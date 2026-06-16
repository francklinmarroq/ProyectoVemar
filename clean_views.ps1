$viewDir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\Views"

$viewFiles = @(
    "ClientesView.xaml","RemedidasView.xaml","ProyectosView.xaml","ColaboradoresView.xaml",
    "ContratistasView.xaml","TramitesView.xaml","AsignacionesView.xaml","AvancesView.xaml",
    "ContratosView.xaml","CobroRemedidasView.xaml","GastoRemedidasView.xaml","MovimientosView.xaml",
    "PagoContratosView.xaml","ZonificacionesView.xaml","CategoriaProyectosView.xaml",
    "TiposTramiteView.xaml","TiposMovimientoView.xaml","UsuariosView.xaml"
)

foreach ($f in $viewFiles) {
    $path = "$viewDir\$f"
    if (-not (Test-Path $path)) { Write-Host "SKIP (not found): $f"; continue }

    $c = Get-Content $path -Raw -Encoding UTF8

    # 1. Remove 4th RowDefinition (the Auto one for form panel)
    $c = $c -replace '<RowDefinition Height="Auto"/><RowDefinition Height="12"/><RowDefinition Height="\*"/><RowDefinition Height="Auto"/>',
                     '<RowDefinition Height="Auto"/><RowDefinition Height="12"/><RowDefinition Height="*"/>'

    # 2. Remove BoolToVis and InvBoolToVis converter resources
    $c = $c -replace '\s*<BooleanToVisibilityConverter x:Key="BoolToVis"/>', ''
    $c = $c -replace '\s*<conv:InverseBooleanToVisibilityConverter x:Key="InvBoolToVis"/>', ''

    # 3. Remove Visibility binding from Agregar button (InvBoolToVis)
    $c = $c -replace ' Visibility="\{Binding MostrandoFormulario, Converter=\{StaticResource InvBoolToVis\}\}"', ''

    # 4. Remove the form panel Border (Grid.Row="3") - multiline
    # Strategy: find the Border starting with Grid.Row="3" and remove until closing </Border>
    # Use a simpler approach: split on the marker and remove from there to last </Border> before </Grid>
    $marker = '<Border Grid.Row="3"'
    $markerIdx = $c.IndexOf($marker)
    if ($markerIdx -ge 0) {
        # Find the last </Border> after the marker (which closes the form panel)
        # Count opening/closing Border tags to find the matching one
        $depth = 0
        $i = $markerIdx
        $endIdx = -1
        while ($i -lt $c.Length - 7) {
            if ($c.Substring($i, [Math]::Min(7, $c.Length - $i)) -eq '<Border') {
                $depth++
                $i += 7
            } elseif ($c.Substring($i, [Math]::Min(9, $c.Length - $i)) -eq '</Border>') {
                $depth--
                if ($depth -eq 0) {
                    $endIdx = $i + 9
                    break
                }
                $i += 9
            } else {
                $i++
            }
        }
        if ($endIdx -ge 0) {
            $c = $c.Substring(0, $markerIdx) + $c.Substring($endIdx)
        }
    }

    # 5. Remove xmlns:conv if no longer referenced
    if ($c -notmatch 'conv:') {
        $c = $c -replace '\s*xmlns:conv="[^"]*"', ''
    }

    Set-Content $path -Value $c -Encoding UTF8
    Write-Host "Cleaned: $f"
}
Write-Host "Done."
