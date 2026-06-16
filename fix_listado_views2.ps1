$dir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\Views"

function Fix-Listado($file, $binding, $cols) {
    $path = "$dir\$file"
    $lines = [System.Collections.Generic.List[string]](Get-Content $path -Encoding UTF8)

    # Remove trailing empty lines, </UserControl>, and last </Grid>
    while ($lines.Count -gt 0 -and $lines[$lines.Count-1].Trim() -eq '') { $lines.RemoveAt($lines.Count-1) }
    if ($lines[$lines.Count-1].Trim() -eq '</UserControl>') { $lines.RemoveAt($lines.Count-1) }
    while ($lines.Count -gt 0 -and $lines[$lines.Count-1].Trim() -eq '') { $lines.RemoveAt($lines.Count-1) }
    if ($lines[$lines.Count-1].Trim() -eq '</Grid>') { $lines.RemoveAt($lines.Count-1) }

    $lines.Add('        </Grid>')
    $lines.Add('        <Border Grid.Row="2" CornerRadius="12" Background="White" ClipToBounds="True">')
    $lines.Add('            <Border.Effect><DropShadowEffect BlurRadius="16" ShadowDepth="0" Opacity="0.06" Color="#000"/></Border.Effect>')
    $lines.Add('            <DataGrid ItemsSource="{Binding ' + $binding + '}" AutoGenerateColumns="False" IsReadOnly="True" CanUserAddRows="False"')
    $lines.Add('                      GridLinesVisibility="Horizontal" HeadersVisibility="Column" BorderThickness="0"')
    $lines.Add('                      RowBackground="White" AlternatingRowBackground="#FAFBFC" HorizontalGridLinesBrush="#F1F5F9" SelectionMode="Single">')
    $lines.Add('                <DataGrid.Columns>')
    foreach ($col in $cols) { $lines.Add($col) }
    $lines.Add('                </DataGrid.Columns>')
    $lines.Add('            </DataGrid>')
    $lines.Add('        </Border>')
    $lines.Add('    </Grid>')
    $lines.Add('</UserControl>')

    Set-Content $path -Value $lines -Encoding UTF8
    Write-Host "Fixed: $file"
}

Fix-Listado "ListadoClientesView.xaml" "Clientes" @(
    '                    <DataGridTextColumn Header="ID" Binding="{Binding Id}" Width="60"/>',
    '                    <DataGridTextColumn Header="NOMBRE" Binding="{Binding Nombre}" Width="*"/>',
    '                    <DataGridTextColumn Header="RTN" Binding="{Binding Rtn}" Width="130"/>',
    '                    <DataGridTextColumn Header="TELEFONO" Binding="{Binding Telefono}" Width="120"/>'
)

Fix-Listado "ListadoRemedidasView.xaml" "Remedidas" @(
    '                    <DataGridTextColumn Header="ID" Binding="{Binding Id}" Width="60"/>',
    '                    <DataGridTextColumn Header="REPRESENTANTE" Binding="{Binding Representante}" Width="*"/>',
    '                    <DataGridTextColumn Header="UBICACION" Binding="{Binding Ubicacion}" Width="180"/>',
    '                    <DataGridTextColumn Header="PRECIO" Binding="{Binding Precio}" Width="120"/>'
)

Fix-Listado "ListadoProyectosView.xaml" "Proyectos" @(
    '                    <DataGridTextColumn Header="ID" Binding="{Binding Id}" Width="60"/>',
    '                    <DataGridTextColumn Header="NOMBRE" Binding="{Binding Nombre}" Width="*"/>',
    '                    <DataGridTextColumn Header="UBICACION" Binding="{Binding Ubicacion}" Width="180"/>'
)

Fix-Listado "ListadoColaboradoresView.xaml" "Colaboradores" @(
    '                    <DataGridTextColumn Header="ID" Binding="{Binding Id}" Width="60"/>',
    '                    <DataGridTextColumn Header="NOMBRE" Binding="{Binding Nombre}" Width="*"/>',
    '                    <DataGridTextColumn Header="CARGO" Binding="{Binding Cargo}" Width="150"/>'
)

Fix-Listado "ListadoContratistasView.xaml" "Contratistas" @(
    '                    <DataGridTextColumn Header="ID" Binding="{Binding Id}" Width="60"/>',
    '                    <DataGridTextColumn Header="NOMBRE" Binding="{Binding Nombre}" Width="*"/>',
    '                    <DataGridTextColumn Header="TELEFONO" Binding="{Binding Telefono}" Width="130"/>'
)

Fix-Listado "ListadoTramitesView.xaml" "Tramites" @(
    '                    <DataGridTextColumn Header="ID" Binding="{Binding Id}" Width="60"/>',
    '                    <DataGridTextColumn Header="DESCRIPCION" Binding="{Binding Descripcion}" Width="*"/>'
)

Write-Host "All done."
