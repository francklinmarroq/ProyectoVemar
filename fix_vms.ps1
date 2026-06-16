$nl = "`r`n"

Get-ChildItem "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\ViewModels" -Filter '*ViewModel.vb' | ForEach-Object {
    $c = Get-Content $_.FullName -Raw -Encoding UTF8

    # Fix: PropertyChangedn -> proper newline before GuardadoExitoso
    $c = $c -replace 'PropertyChangedn\s+Public Event GuardadoExitoso', ("PropertyChanged" + $nl + "    Public Event GuardadoExitoso")

    # Fix: Window()n   win.DataContext = Men   win.ShowDialog() -> proper newlines
    $c = [regex]::Replace($c, '(New Agregar\w+Window\(\))n(\s+win\.DataContext = Me)n(\s+win\.ShowDialog\(\))', {
        param($m)
        $m.Groups[1].Value + $nl + $m.Groups[2].Value + $nl + $m.Groups[3].Value
    })

    # Fix: GuardadoExitoso(Me, EventArgs.Empty)n   CargarItems -> proper newline
    $c = $c -replace '(RaiseEvent GuardadoExitoso\(Me, EventArgs\.Empty\))n(\s+CargarItems\(\))', ('$1' + $nl + '$2')

    Set-Content $_.FullName -Value $c -Encoding UTF8
    Write-Host "Fixed: $($_.Name)"
}
