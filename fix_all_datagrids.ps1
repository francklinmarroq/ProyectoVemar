$viewDir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\Views"

Get-ChildItem $viewDir -Filter "*.xaml" | ForEach-Object {
    $path = $_.FullName
    $name = $_.Name

    $content = Get-Content $path -Raw -Encoding UTF8

    # Solo procesar si tiene DataGrid
    if ($content -notmatch '<DataGrid') { return }

    # Reemplazar propiedades viejas del DataGrid
    $content = $content -replace 'GridLinesVisibility="Horizontal"', 'GridLinesVisibility="None"'
    $content = $content -replace 'RowBackground="White" AlternatingRowBackground="[^"]*"', ''
    $content = $content -replace 'HorizontalGridLinesBrush="[^"]*"', ''

    # Agregar estilos al DataGrid si no los tiene
    if ($content -notmatch 'RowStyle="\{StaticResource DgRowStyle') {
        # Buscar la línea del DataGrid y agregar los estilos
        $content = $content -replace '(<DataGrid[^>]*SelectionMode="Single")(?!.*RowStyle)', '$1 RowStyle="{StaticResource DgRowStyle}" CellStyle="{StaticResource DgCellStyle}" ColumnHeaderStyle="{StaticResource DgHeaderStyle}"'
    }

    Set-Content $path -Value $content -Encoding UTF8
    Write-Host "Fixed DataGrid in: $name"
}

Write-Host "All DataGrids configured correctly."
