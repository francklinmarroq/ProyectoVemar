$viewDir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\Views"

# Estilos mejorados para DataGrid
$datagridStyles = @'
        <Style x:Key="DataGridHeaderStyle" TargetType="DataGridColumnHeader">
            <Setter Property="Background" Value="#3B82F6"/>
            <Setter Property="Foreground" Value="White"/>
            <Setter Property="FontSize" Value="12"/>
            <Setter Property="FontWeight" Value="SemiBold"/>
            <Setter Property="Padding" Value="12,10"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="HorizontalContentAlignment" Value="Left"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="DataGridColumnHeader">
                        <Border Background="{TemplateBinding Background}"
                                BorderBrush="#2563EB" BorderThickness="0,0,1,0"
                                Padding="{TemplateBinding Padding}">
                            <ContentPresenter HorizontalAlignment="{TemplateBinding HorizontalContentAlignment}"
                                            VerticalAlignment="Center"/>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
        <Style x:Key="DataGridCellStyle" TargetType="DataGridCell">
            <Setter Property="Padding" Value="12,8"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Foreground" Value="#1F2937"/>
            <Setter Property="FontSize" Value="13"/>
            <Setter Property="Background" Value="Transparent"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="DataGridCell">
                        <Border Background="{TemplateBinding Background}"
                                BorderBrush="{TemplateBinding BorderBrush}"
                                BorderThickness="{TemplateBinding BorderThickness}"
                                Padding="{TemplateBinding Padding}">
                            <ContentPresenter/>
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
            <Style.Triggers>
                <Trigger Property="IsSelected" Value="True">
                    <Setter Property="Background" Value="#EFF6FF"/>
                    <Setter Property="Foreground" Value="#1E40AF"/>
                    <Setter Property="FontWeight" Value="SemiBold"/>
                </Trigger>
            </Style.Triggers>
        </Style>
        <Style x:Key="DataGridRowStyle" TargetType="DataGridRow">
            <Setter Property="Background" Value="White"/>
            <Setter Property="BorderThickness" Value="0"/>
            <Setter Property="Margin" Value="0"/>
            <Setter Property="Padding" Value="0"/>
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="DataGridRow">
                        <Border Background="{TemplateBinding Background}"
                                BorderBrush="#F3F4F6" BorderThickness="0,0,0,1"
                                Padding="{TemplateBinding Padding}">
                            <SelectiveScrollingGrid>
                                <SelectiveScrollingGrid.ColumnDefinitions>
                                    <ColumnDefinition Width="Auto"/>
                                    <ColumnDefinition Width="*"/>
                                </SelectiveScrollingGrid.ColumnDefinitions>
                                <DataGridCellsPresenter ItemsPanel="{TemplateBinding ItemsPanel}"
                                                       Grid.Column="1"/>
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
    $c = Get-Content $path -Raw -Encoding UTF8

    # Solo procesar si tiene DataGrid
    if ($c -notmatch '<DataGrid') { return }

    # Agregar estilos al UserControl.Resources si no existen
    if ($c -match '<UserControl\.Resources>') {
        # Reemplazar el cierre de Resources con los estilos + cierre
        $c = $c -replace '(</UserControl\.Resources>)', ($datagridStyles + "`r`n    </UserControl.Resources>")
    } else {
        # Si no existe Resources, crear uno
        $c = $c -replace '(Background="[^"]*">)', ('$1' + "`r`n    <UserControl.Resources>" + $datagridStyles + "`r`n    </UserControl.Resources>")
    }

    # Aplicar estilos al DataGrid
    $c = $c -replace '(<DataGrid[^>]*)', ('$1 RowStyle="{StaticResource DataGridRowStyle}" CellStyle="{StaticResource DataGridCellStyle}"')
    $c = $c -replace '(<DataGrid\.Columns>)', ('<DataGridColumnHeader.Style>' + "`r`n                <StaticResource ResourceKey="DataGridHeaderStyle"/>`r`n            </DataGridColumnHeader.Style>`r`n            $1')

    # Si no hay ColumnHeader.Style, agregarlo antes de Columns
    if ($c -notmatch 'DataGridColumnHeader\.Style') {
        $c = $c -replace '(<DataGrid[^>]*RowStyle[^>]*CellStyle[^>]*>)', ('$1' + "`r`n                <DataGrid.ColumnHeaderStyle>`r`n                    <StaticResource ResourceKey="DataGridHeaderStyle"/>`r`n                </DataGrid.ColumnHeaderStyle>")
    }

    # Actualizar estilos de DataGrid
    $c = $c -replace 'GridLinesVisibility="Horizontal"', 'GridLinesVisibility="None"'
    $c = $c -replace 'RowBackground="White" AlternatingRowBackground="#FAFBFC"', ''
    $c = $c -replace 'HorizontalGridLinesBrush="#F1F5F9"', ''

    Set-Content $path -Value $c -Encoding UTF8
    Write-Host "Mejorado: $($_.Name)"
}

Write-Host "Estilos DataGrid aplicados."
