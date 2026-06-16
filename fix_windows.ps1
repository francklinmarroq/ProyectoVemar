$winDir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\Windows"

$windows = @(
    @{ Name="AgregarZonificacionWindow";    VM="ZonificacionesViewModel" },
    @{ Name="AgregarTipoTramiteWindow";     VM="TiposTramiteViewModel" },
    @{ Name="AgregarTipoMovimientoWindow";  VM="TiposMovimientoViewModel" },
    @{ Name="AgregarCategoriaProyectoWindow"; VM="CategoriaProyectosViewModel" },
    @{ Name="AgregarColaboradorWindow";     VM="ColaboradoresViewModel" },
    @{ Name="AgregarContratistaWindow";     VM="ContratistasViewModel" },
    @{ Name="AgregarAvanceWindow";          VM="AvancesViewModel" },
    @{ Name="AgregarCobroRemedidaWindow";   VM="CobroRemedidasViewModel" },
    @{ Name="AgregarGastoRemedidaWindow";   VM="GastoRemedidasViewModel" },
    @{ Name="AgregarMovimientoWindow";      VM="MovimientosViewModel" },
    @{ Name="AgregarPagoContratoWindow";    VM="PagoContratosViewModel" },
    @{ Name="AgregarContratoWindow";        VM="ContratosViewModel" },
    @{ Name="AgregarUsuarioWindow";         VM="UsuariosViewModel" },
    @{ Name="AgregarClienteWindow";         VM="ClientesViewModel" },
    @{ Name="AgregarRemedidaWindow";        VM="RemedidasViewModel" },
    @{ Name="AgregarProyectoWindow";        VM="ProyectosViewModel" },
    @{ Name="AgregarTramiteWindow";         VM="TramitesViewModel" },
    @{ Name="AgregarAsignacionWindow";      VM="AsignacionesViewModel" }
)

foreach ($w in $windows) {
    $n = $w.Name
    $vm = $w.VM

    # Fix XAML: add Loaded event and BtnCancelar Click
    $xamlPath = "$winDir\$n.xaml"
    $xaml = Get-Content $xamlPath -Raw -Encoding UTF8

    # Add Loaded="Window_Loaded" to Window element
    $xaml = $xaml -replace '(Background="#F8FAFC">)', 'Background="#F8FAFC" Loaded="Window_Loaded">'

    # Add Click="BtnCancelar_Click" to BtnCancelar button
    $xaml = $xaml -replace '(Name="BtnCancelar")', 'Name="BtnCancelar" Click="BtnCancelar_Click"'

    Set-Content $xamlPath -Value $xaml -Encoding UTF8

    # Rewrite code-behind without Handles
    $vb = @"
Public Class $n
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim vm = TryCast(DataContext, $vm)
        If vm IsNot Nothing Then
            AddHandler vm.GuardadoExitoso, Sub(s, ev) Me.Close()
        End If
    End Sub
    Private Sub BtnCancelar_Click(sender As Object, e As RoutedEventArgs)
        Me.Close()
    End Sub
End Class
"@
    Set-Content "$winDir\$n.xaml.vb" -Value $vb -Encoding UTF8
    Write-Host "Fixed: $n"
}
Write-Host "Done."
