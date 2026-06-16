$winDir = "C:\Users\Marroquin Quezada\Documents\ProyectoVemar\Vemar.WPF\Windows"

$windows = @(
    @{ Name="AgregarZonificacionWindow";       VM="ZonificacionesViewModel" },
    @{ Name="AgregarTipoTramiteWindow";        VM="TiposTramiteViewModel" },
    @{ Name="AgregarTipoMovimientoWindow";     VM="TiposMovimientoViewModel" },
    @{ Name="AgregarCategoriaProyectoWindow";  VM="CategoriaProyectosViewModel" },
    @{ Name="AgregarColaboradorWindow";        VM="ColaboradoresViewModel" },
    @{ Name="AgregarContratistaWindow";        VM="ContratistasViewModel" },
    @{ Name="AgregarAvanceWindow";             VM="AvancesViewModel" },
    @{ Name="AgregarCobroRemedidaWindow";      VM="CobroRemedidasViewModel" },
    @{ Name="AgregarGastoRemedidaWindow";      VM="GastoRemedidasViewModel" },
    @{ Name="AgregarMovimientoWindow";         VM="MovimientosViewModel" },
    @{ Name="AgregarPagoContratoWindow";       VM="PagoContratosViewModel" },
    @{ Name="AgregarContratoWindow";           VM="ContratosViewModel" },
    @{ Name="AgregarUsuarioWindow";            VM="UsuariosViewModel" },
    @{ Name="AgregarClienteWindow";            VM="ClientesViewModel" },
    @{ Name="AgregarRemedidaWindow";           VM="RemedidasViewModel" },
    @{ Name="AgregarProyectoWindow";           VM="ProyectosViewModel" },
    @{ Name="AgregarTramiteWindow";            VM="TramitesViewModel" },
    @{ Name="AgregarAsignacionWindow";         VM="AsignacionesViewModel" }
)

foreach ($w in $windows) {
    $n = $w.Name
    $vm = $w.VM

    # Fix XAML: remove Loaded and Click event attributes (keep Name)
    $xamlPath = "$winDir\$n.xaml"
    $xaml = Get-Content $xamlPath -Raw -Encoding UTF8
    $xaml = $xaml -replace ' Loaded="Window_Loaded"', ''
    $xaml = $xaml -replace ' Click="BtnCancelar_Click"', ''
    Set-Content $xamlPath -Value $xaml -Encoding UTF8

    # Rewrite code-behind: use AddHandler in New()
    $vb = @"
Public Class $n
    Public Sub New()
        InitializeComponent()
        AddHandler Me.Loaded, Sub(s As Object, e As RoutedEventArgs)
                                  Dim vm = TryCast(DataContext, $vm)
                                  If vm IsNot Nothing Then
                                      AddHandler vm.GuardadoExitoso, Sub(ss, ev) Me.Close()
                                  End If
                              End Sub
        AddHandler BtnCancelar.Click, Sub(s As Object, e As RoutedEventArgs)
                                          Me.Close()
                                      End Sub
    End Sub
End Class
"@
    Set-Content "$winDir\$n.xaml.vb" -Value $vb -Encoding UTF8
    Write-Host "Fixed: $n"
}
Write-Host "Done."
