Imports Vemar.Domain

Public Class DetalleTramiteViewModelFactory : Implements IViewModelFactory(Of DetalleTramiteViewModel)
    Private ReadOnly _tramiteService As IDataService(Of Tramite)
    Private ReadOnly _tipoTramiteService As IDataService(Of TipoTramite)
    Private ReadOnly _estadoTramiteService As IDataService(Of EstadoTramite)
    Private ReadOnly _proyectoService As IDataService(Of Proyecto)

    Public Sub New(tramiteService As IDataService(Of Tramite),
                   tipoTramiteService As IDataService(Of TipoTramite),
                   estadoTramiteService As IDataService(Of EstadoTramite),
                   proyectoService As IDataService(Of Proyecto))
        _tramiteService = tramiteService
        _tipoTramiteService = tipoTramiteService
        _estadoTramiteService = estadoTramiteService
        _proyectoService = proyectoService
    End Sub

    Public Function CreateViewModel() As DetalleTramiteViewModel Implements IViewModelFactory(Of DetalleTramiteViewModel).CreateViewModel
        Return New DetalleTramiteViewModel(_tramiteService, _tipoTramiteService, _estadoTramiteService, _proyectoService)
    End Function
End Class
