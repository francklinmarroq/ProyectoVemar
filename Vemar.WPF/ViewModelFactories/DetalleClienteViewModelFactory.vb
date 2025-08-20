Imports Vemar.Domain

Public Class DetalleClienteViewModelFactory : Implements IViewModelFactory(Of DetalleClienteViewModel)
    Private ReadOnly _dataService As IDataService(Of Cliente)

    Public Sub New(dataService As IDataService(Of Cliente))
        _dataService = dataService
    End Sub

    Public Function CreateViewModel() As DetalleClienteViewModel Implements IViewModelFactory(Of DetalleClienteViewModel).CreateViewModel
        Return New DetalleClienteViewModel(_dataService)
    End Function
End Class
