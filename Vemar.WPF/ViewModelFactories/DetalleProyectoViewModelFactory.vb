Imports Vemar.Domain

Public Class DetalleProyectoViewModelFactory : Implements IViewModelFactory(Of DetalleProyectoViewModel)
    Private ReadOnly _proyectoService As IDataService(Of Proyecto)
    Private ReadOnly _clienteService As IDataService(Of Cliente)
    Private ReadOnly _zonificacionService As IDataService(Of Zonificacion)
    Private ReadOnly _categoriaService As IDataService(Of CategoriaProyecto)

    Public Sub New(proyectoService As IDataService(Of Proyecto),
                   clienteService As IDataService(Of Cliente),
                   zonificacionService As IDataService(Of Zonificacion),
                   categoriaService As IDataService(Of CategoriaProyecto))
        _proyectoService = proyectoService
        _clienteService = clienteService
        _zonificacionService = zonificacionService
        _categoriaService = categoriaService
    End Sub

    Public Function CreateViewModel() As DetalleProyectoViewModel Implements IViewModelFactory(Of DetalleProyectoViewModel).CreateViewModel
        Return New DetalleProyectoViewModel(_proyectoService, _clienteService, _zonificacionService, _categoriaService)
    End Function
End Class
