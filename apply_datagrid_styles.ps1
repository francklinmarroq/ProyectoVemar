$viewDir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\Views"

$datagridStyles = @'
        <!-- DataGrid Styles -->
        <Style x:Key="DgHeaderStyle" TargetType="DataGridColumnHeader">
            <Setter Property="Background" Value="#3B82F6"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="FontSize" Value="12"/>
            <Setter Property="FontWeight" Value="SemiBold"/>
            <Setter Property="Padding" Value="12,10"/>
            <Setter Property="BorderThickness" Value="0,0,1,0"/>
            <Setter Property="BorderBrush" Value="#2563EB"/>
            <Setter Property="HorizontalContentAlignment" Value="Left"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="DataGridColumnHeader">
                        <Border Background="{TemplateBinding Background}" BorderBrush="{TemplateBinding BorderBrush}" BorderThickness="{TemplateBinding BorderThickness}" Padding="{TemplateBinding Padding}">
                            <ContentPresenter VerticalAlignment="Center"/>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
        <Style x:Key="DgCellStyle" TargetType="DataGridCell">
            <Setter Property="Padding" Value="12,8"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Foreground" Value="#1F2937"/>
            <Setter Property="FontSize" Value="13"/>
            <Setter Property="Background" Value="Transparent"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="DataGridCell">
                        <Border Background="{TemplateBinding Background}" Padding="{TemplateBinding Padding}">
                            <ContentPresenter VerticalAlignment="Center"/>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
            <Style.Triggers>
                <Trigger Property="IsSelected" Value="True">
                    <Setter Property="Background" Value="#EFF6FF"/>
                    <Setter Property="Foreground" Value="#1E40AF"/>
                </Trigger>
            </Style.Triggers>
        </Style>
        <Style x:Key="DgRowStyle" TargetType="DataGridRow">
            <Setter Property="Background" Value="White"/>
            <Setter Property="Margin" Value="0"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="DataGridRow">
                        <Border Background="{TemplateBinding Background}" BorderBrush="#F0F4F8" BorderThickness="0,0,0,1">
                            <SelectiveScrollingGrid>
                                <SelectiveScrollingGrid.ColumnDefinitions>
                                    <ColumnDefinition Width="Auto"/>
                                    <ColumnDefinition Width="*"/>
                                </SelectiveScrollingGrid.ColumnDefinitions>
                                <DataGridCellsPresenter ItemsPanel="{TemplateBinding ItemsPanel}" Grid.Column="1"/>
                            </SelectiveScrollingGrid>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
            <Style.Triggers>
                <Trigger Property="IsMouseOver" Value="True">
                    <Setter Property="Background" Value="#F0F9FF"/>
                </Trigger>
                <Trigger Property="IsSelected" Value="True">
                    <Setter Property="Background" Value="#DBEAFE"/>
                </Trigger>
            </Style.Triggers>
        </Style>
'@

Get-ChildItem $viewDir -Filter "*.xaml" | ForEach-Object {
    $path = $_.FullName
    $name = $_.Name

    # Skip ClientesView (already done)
    if ($name -eq "ClientesView.xaml") { Write-Host "SKIP (already updated): $name"; return }

    $lines = [System.Collections.Generic.List[string]](Get-Content $path -Encoding UTF8)

    # Buscar dónde termina UserControl.Resources
    $resourcesEndIdx = -1
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '</UserControl\.Resources>') {
            $resourcesEndIdx = $i
            break
        }
    }

    # Si no existe Resources, buscar dónde insertar (después del Background="...")
    if ($resourcesEndIdx -eq -1) {
        # Crear Resources section
        $insertIdx = -1
        for ($i = 0; $i -lt $lines.Count; $i++) {
            if ($lines[$i] -match 'Background="' -and $lines[$i] -match '>') {
                $insertIdx = $i + 1
                break
            }
        }
        if ($insertIdx -gt 0) {
            $lines.Insert($insertIdx, "    <UserControl.Resources>")
            $datagridStyles.Split("`n") | ForEach-Object { $lines.Insert($insertIdx + 1, $_); $insertIdx++ }
            $lines.Insert($insertIdx + 1, "    </UserControl.Resources>")
            $resourcesEndIdx = $insertIdx + 1
        }
    } else {
        # Insertar estilos antes de </UserControl.Resources>
        $datagridStyles.Split("`n") | ForEach-Object {
            $lines.Insert($resourcesEndIdx, $_)
            $resourcesEndIdx++
        }
    }

    # Actualizar DataGrid: agregar estilos
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '<DataGrid') {
            # Reemplazar propiedades viejas
            $lines[$i] = $lines[$i] -replace 'GridLinesVisibility="Horizontal"', 'GridLinesVisibility="None"'
            $lines[$i] = $lines[$i] -replace 'RowBackground="White" AlternatingRowBackground="[^"]*"', ''
            $lines[$i] = $lines[$i] -replace 'HorizontalGridLinesBrush="[^"]*"', ''

            # Agregar estilos
            if ($lines[$i] -notmatch 'RowStyle=') {
                $lines[$i] = $lines[$i] -replace '(SelectionMode="Single")', 'SelectionMode="Single" RowStyle="{StaticResource DgRowStyle}" CellStyle="{StaticResource DgCellStyle}" ColumnHeaderStyle="{StaticResource DgHeaderStyle}"'
            }
            break
        }
    }

    Set-Content $path -Value $lines -Encoding UTF8
    Write-Host "Updated: $name"
}

Write-Host "All DataGrid styles applied."
