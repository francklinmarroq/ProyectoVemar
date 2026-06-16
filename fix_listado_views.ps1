$dir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\Views"

$views = @(
    @{ File="ListadoClientesView.xaml"; Binding="Clientes"; Columns=@(
        @{H="ID";B="Id";W="60"},@{H="NOMBRE";B="Nombre";W="*"},@{H="RTN";B="Rtn";W="130"},@{H="TELÉFONO";B="Telefono";W="120"}
    )},
    @{ File="ListadoRemedidasView.xaml"; Binding="Remedidas"; Columns=@(
        @{H="ID";B="Id";W="60"},@{H="REPRESENTANTE";B="Representante";W="*"},@{H="UBICACIÓN";B="Ubicacion";W="180"},@{H="PRECIO";B="Precio";W="120"}
    )},
    @{ File="ListadoProyectosView.xaml"; Binding="Proyectos"; Columns=@(
        @{H="ID";B="Id";W="60"},@{H="NOMBRE";B="Nombre";W="*"},@{H="UBICACIÓN";B="Ubicacion";W="180"}
    )},
    @{ File="ListadoColaboradoresView.xaml"; Binding="Colaboradores"; Columns=@(
        @{H="ID";B="Id";W="60"},@{H="NOMBRE";B="Nombre";W="*"},@{H="CARGO";B="Cargo";W="150"}
    )},
    @{ File="ListadoContratistasView.xaml"; Binding="Contratistas"; Columns=@(
        @{H="ID";B="Id";W="60"},@{H="NOMBRE";B="Nombre";W="*"},@{H="TELÉFONO";B="Telefono";W="130"}
    )},
    @{ File="ListadoTramitesView.xaml"; Binding="Tramites"; Columns=@(
        @{H="ID";B="Id";W="60"},@{H="DESCRIPCIÓN";B="Descripcion";W="*"}
    )}
)

foreach ($v in $views) {
    $path = "$dir\$($v.File)"
    $lines = [System.Collections.Generic.List[string]](Get-Content $path -Encoding UTF8)

    # Find the last line before </Grid> and </UserControl>
    # Remove the trailing </Grid> and </UserControl>
    while ($lines.Count -gt 0 -and ($lines[$lines.Count-1].Trim() -eq '' -or $lines[$lines.Count-1].Trim() -eq '</UserControl>')) {
        $lines.RemoveAt($lines.Count - 1)
    }
    # Remove the trailing </Grid> (the misplaced one)
    if ($lines.Count -gt 0 -and $lines[$lines.Count-1].Trim() -eq '</Grid>') {
        $lines.RemoveAt($lines.Count - 1)
    }

    # Add proper closing: Header Grid close, DataGrid, root Grid close, UserControl close
    $lines.Add('        </Grid>')

    # DataGrid border
    $lines.Add('        <Border Grid.Row="2" CornerRadius="12" Background="White" ClipToBounds="True">')
    $lines.Add('            <Border.Effect><DropShadowEffect BlurRadius="16" ShadowDepth="0" Opacity="0.06" Color="#000"/></Border.Effect>')
    $lines.Add("            <DataGrid ItemsSource=""{Binding $($v.Binding)}"" AutoGenerateColumns=""False"" IsReadOnly=""True"" CanUserAddRows=""False""")
    $lines.Add('                      GridLinesVisibility="Horizontal" HeadersVisibility="Column" BorderThickness="0"')
    $lines.Add('                      RowBackground="White" AlternatingRowBackground="#FAFBFC" HorizontalGridLinesBrush="#F1F5F9" SelectionMode="Single">')
    $lines.Add('                <DataGrid.Columns>')
    foreach ($col in $v.Columns) {
        $lines.Add("                    <DataGridTextColumn Header=""$($col.H)"" Binding=""{Binding $($col.B)}"" Width=""$($col.W)""/>")
    }
    $lines.Add('                </DataGrid.Columns>')
    $lines.Add('            </DataGrid>')
    $lines.Add('        </Border>')
    $lines.Add('    </Grid>')
    $lines.Add('</UserControl>')

    Set-Content $path -Value $lines -Encoding UTF8
    Write-Host "Fixed: $($v.File)"
}
Write-Host "Done."
