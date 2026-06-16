$viewDir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\Views"

function Fix-View($fileName, $dataGridContent) {
    $path = "$viewDir\$fileName"
    $c = Get-Content $path -Raw -Encoding UTF8

    # Remove orphaned RowDefinition Height="Auto" at the end of RowDefinitions (4th row)
    $c = $c -replace '(<RowDefinition Height="Auto"/>)?(<RowDefinition Height="12"/>)(<RowDefinition Height="\*"/>)<RowDefinition Height="Auto"/>',
                     '$2$3'
    # Also handle 4-line format
    $c = $c -replace '(<RowDefinition Height="Auto"/>\r?\n\s*<RowDefinition Height="12"/>\r?\n\s*<RowDefinition Height="\*"/>\r?\n\s*<RowDefinition Height="Auto"/>)',
                     '<RowDefinition Height="Auto"/>
            <RowDefinition Height="12"/>
            <RowDefinition Height="*"/>'

    # The file ends with inner </Grid> and </UserControl> - need to add DataGrid + root Grid close
    # Pattern: ...    </Grid>\r\n</UserControl> at the very end
    $ending = "    </Grid>`r`n</UserControl>"
    $endingIdx = $c.LastIndexOf("    </Grid>")

    if ($endingIdx -lt 0) { Write-Host "SKIP: $fileName (no ending Grid found)"; return }

    # Everything up to and including the inner Grid close
    $before = $c.Substring(0, $endingIdx + 11) # 11 = length of "    </Grid>"

    # Build the new ending
    $newEnd = "`r`n        </Grid>`r`n        $dataGridContent`r`n    </Grid>`r`n</UserControl>"

    $result = $before + $newEnd
    Set-Content $path -Value $result -Encoding UTF8
    Write-Host "Fixed: $fileName"
}

# RemedidasView
Fix-View "RemedidasView.xaml" @'
<Border Grid.Row="2" CornerRadius="12" Background="White" ClipToBounds="True">
            <Border.Effect><DropShadowEffect BlurRadius="16" ShadowDepth="0" Opacity="0.06" Color="#000"/></Border.Effect>
            <DataGrid ItemsSource="{Binding Items}" AutoGenerateColumns="False" IsReadOnly="True" CanUserAddRows="False"
                      GridLinesVisibility="Horizontal" HeadersVisibility="Column" BorderThickness="0"
                      RowBackground="White" AlternatingRowBackground="#FAFBFC" HorizontalGridLinesBrush="#F1F5F9" SelectionMode="Single">
                <DataGrid.Columns>
                    <DataGridTextColumn Header="ID" Binding="{Binding Id}" Width="60"/>
                    <DataGridTextColumn Header="REPRESENTANTE" Binding="{Binding Representante}" Width="*"/>
                    <DataGridTextColumn Header="UBICACIÓN" Binding="{Binding Ubicacion}" Width="180"/>
                    <DataGridTextColumn Header="MATRÍCULA" Binding="{Binding Matricula}" Width="120"/>
                    <DataGridTextColumn Header="PRECIO" Binding="{Binding Precio}" Width="120"/>
                    <DataGridTextColumn Header="FECHA" Binding="{Binding Fecha, StringFormat='{}{0:dd/MM/yyyy}'}" Width="100"/>
                </DataGrid.Columns>
            </DataGrid>
        </Border>
'@

# ColaboradoresView
Fix-View "ColaboradoresView.xaml" @'
<Border Grid.Row="2" CornerRadius="12" Background="White" ClipToBounds="True">
            <Border.Effect><DropShadowEffect BlurRadius="16" ShadowDepth="0" Opacity="0.06" Color="#000"/></Border.Effect>
            <DataGrid ItemsSource="{Binding Items}" AutoGenerateColumns="False" IsReadOnly="True" CanUserAddRows="False"
                      GridLinesVisibility="Horizontal" HeadersVisibility="Column" BorderThickness="0"
                      RowBackground="White" AlternatingRowBackground="#FAFBFC" HorizontalGridLinesBrush="#F1F5F9" SelectionMode="Single">
                <DataGrid.Columns>
                    <DataGridTextColumn Header="ID" Binding="{Binding Id}" Width="60"/>
                    <DataGridTextColumn Header="NOMBRE" Binding="{Binding Nombre}" Width="*"/>
                    <DataGridTextColumn Header="TELÉFONO" Binding="{Binding Telefono}" Width="130"/>
                    <DataGridTextColumn Header="CARGO" Binding="{Binding Cargo}" Width="150"/>
                    <DataGridTextColumn Header="EMAIL" Binding="{Binding Email}" Width="180"/>
                </DataGrid.Columns>
            </DataGrid>
        </Border>
'@

# GastoRemedidasView
Fix-View "GastoRemedidasView.xaml" @'
<Border Grid.Row="2" CornerRadius="12" Background="White" ClipToBounds="True">
            <Border.Effect><DropShadowEffect BlurRadius="16" ShadowDepth="0" Opacity="0.06" Color="#000"/></Border.Effect>
            <DataGrid ItemsSource="{Binding Items}" AutoGenerateColumns="False" IsReadOnly="True" CanUserAddRows="False"
                      GridLinesVisibility="Horizontal" HeadersVisibility="Column" BorderThickness="0"
                      RowBackground="White" AlternatingRowBackground="#FAFBFC" HorizontalGridLinesBrush="#F1F5F9" SelectionMode="Single">
                <DataGrid.Columns>
                    <DataGridTextColumn Header="ID" Binding="{Binding Id}" Width="60"/>
                    <DataGridTextColumn Header="DESCRIPCIÓN" Binding="{Binding Descripcion}" Width="*"/>
                    <DataGridTextColumn Header="MONTO" Binding="{Binding Monto}" Width="130"/>
                </DataGrid.Columns>
            </DataGrid>
        </Border>
'@

# ProyectosView
Fix-View "ProyectosView.xaml" @'
<Border Grid.Row="2" CornerRadius="12" Background="White" ClipToBounds="True">
            <Border.Effect><DropShadowEffect BlurRadius="16" ShadowDepth="0" Opacity="0.06" Color="#000"/></Border.Effect>
            <DataGrid ItemsSource="{Binding Items}" AutoGenerateColumns="False" IsReadOnly="True" CanUserAddRows="False"
                      GridLinesVisibility="Horizontal" HeadersVisibility="Column" BorderThickness="0"
                      RowBackground="White" AlternatingRowBackground="#FAFBFC" HorizontalGridLinesBrush="#F1F5F9" SelectionMode="Single">
                <DataGrid.Columns>
                    <DataGridTextColumn Header="ID" Binding="{Binding Id}" Width="60"/>
                    <DataGridTextColumn Header="NOMBRE" Binding="{Binding Nombre}" Width="*"/>
                    <DataGridTextColumn Header="UBICACIÓN" Binding="{Binding Ubicacion}" Width="180"/>
                    <DataGridTextColumn Header="MATRÍCULA" Binding="{Binding Matricula}" Width="120"/>
                    <DataGridTextColumn Header="ÁREA" Binding="{Binding Area}" Width="100"/>
                </DataGrid.Columns>
            </DataGrid>
        </Border>
'@

# TramitesView
Fix-View "TramitesView.xaml" @'
<Border Grid.Row="2" CornerRadius="12" Background="White" ClipToBounds="True">
            <Border.Effect><DropShadowEffect BlurRadius="16" ShadowDepth="0" Opacity="0.06" Color="#000"/></Border.Effect>
            <DataGrid ItemsSource="{Binding Items}" AutoGenerateColumns="False" IsReadOnly="True" CanUserAddRows="False"
                      GridLinesVisibility="Horizontal" HeadersVisibility="Column" BorderThickness="0"
                      RowBackground="White" AlternatingRowBackground="#FAFBFC" HorizontalGridLinesBrush="#F1F5F9" SelectionMode="Single">
                <DataGrid.Columns>
                    <DataGridTextColumn Header="ID" Binding="{Binding Id}" Width="60"/>
                    <DataGridTextColumn Header="DESCRIPCIÓN" Binding="{Binding Descripcion}" Width="*"/>
                </DataGrid.Columns>
            </DataGrid>
        </Border>
'@

Write-Host "All combined views fixed."

# Now fix Listado* views - they need </Grid> added before </UserControl>
$listadoViews = @("ListadoClientesView.xaml","ListadoColaboradoresView.xaml","ListadoContratistasView.xaml","ListadoProyectosView.xaml","ListadoRemedidasView.xaml","ListadoTramitesView.xaml")

foreach ($lv in $listadoViews) {
    $path = "$viewDir\$lv"
    if (-not (Test-Path $path)) { continue }
    $c = Get-Content $path -Raw -Encoding UTF8

    # Check if the file ends with </Grid> + </UserControl> (correct)
    # or is missing the root </Grid>
    if ($c -match '    </Grid>\r?\n</UserControl>') {
        Write-Host "OK (no fix needed): $lv"
    } else {
        # Add root Grid close before </UserControl>
        $c = $c -replace '(\s*</UserControl>)', "`r`n    </Grid>`r`n</UserControl>"
        Set-Content $path -Value $c -Encoding UTF8
        Write-Host "Fixed Listado: $lv"
    }
}
Write-Host "Done."
