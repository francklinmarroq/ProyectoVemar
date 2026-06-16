Imports Microsoft.Extensions.DependencyInjection

Public Class ViewModelAbstractFactory : Implements IViewModelAbstractFactory
    Private ReadOnly _sp As IServiceProvider

    Public Sub New(sp As IServiceProvider)
        _sp = sp
    End Sub

    Public Function CreateViewModel(vmType As ViewModelType) As ViewModelBase Implements IViewModelAbstractFactory.CreateViewModel
        Select Case vmType
            Case ViewModelType.ClientesViewModel
                Return _sp.GetRequiredService(Of ClientesViewModel)()
            Case ViewModelType.RemedidasViewModel
                Return _sp.GetRequiredService(Of RemedidasViewModel)()
            Case ViewModelType.ProyectosViewModel
                Return _sp.GetRequiredService(Of ProyectosViewModel)()
            Case ViewModelType.ColaboradoresViewModel
                Return _sp.GetRequiredService(Of ColaboradoresViewModel)()
            Case ViewModelType.ContratistasViewModel
                Return _sp.GetRequiredService(Of ContratistasViewModel)()
            Case ViewModelType.TramitesViewModel
                Return _sp.GetRequiredService(Of TramitesViewModel)()
            Case ViewModelType.AsignacionesViewModel
                Return _sp.GetRequiredService(Of AsignacionesViewModel)()
            Case ViewModelType.AvancesViewModel
                Return _sp.GetRequiredService(Of AvancesViewModel)()
            Case ViewModelType.ContratosViewModel
                Return _sp.GetRequiredService(Of ContratosViewModel)()
            Case ViewModelType.CobroRemedidasViewModel
                Return _sp.GetRequiredService(Of CobroRemedidasViewModel)()
            Case ViewModelType.GastoRemedidasViewModel
                Return _sp.GetRequiredService(Of GastoRemedidasViewModel)()
            Case ViewModelType.MovimientosViewModel
                Return _sp.GetRequiredService(Of MovimientosViewModel)()
            Case ViewModelType.PagoContratosViewModel
                Return _sp.GetRequiredService(Of PagoContratosViewModel)()
            Case ViewModelType.ZonificacionesViewModel
                Return _sp.GetRequiredService(Of ZonificacionesViewModel)()
            Case ViewModelType.CategoriaProyectosViewModel
                Return _sp.GetRequiredService(Of CategoriaProyectosViewModel)()
            Case ViewModelType.TiposTramiteViewModel
                Return _sp.GetRequiredService(Of TiposTramiteViewModel)()
            Case ViewModelType.EstadosTramiteViewModel
                Return _sp.GetRequiredService(Of EstadosTramiteViewModel)()
            Case ViewModelType.TiposMovimientoViewModel
                Return _sp.GetRequiredService(Of TiposMovimientoViewModel)()
            Case ViewModelType.UsuariosViewModel
                Return _sp.GetRequiredService(Of UsuariosViewModel)()
            Case ViewModelType.CajaChicaViewModel
                Return _sp.GetRequiredService(Of CajaChicaViewModel)()
            Case Else
                Throw New ArgumentException($"ViewModel type '{vmType}' not supported.")
        End Select
    End Function
End Class
