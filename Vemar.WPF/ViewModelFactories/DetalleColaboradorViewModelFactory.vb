Imports Vemar.Domain

Public Class DetalleColaboradorViewModelFactory : Implements IViewModelFactory(Of DetalleColaboradorViewModel)
    Private ReadOnly _dataService As IDataService(Of Colaborador)
    Public Sub New(dataService As IDataService(Of Colaborador))
        _dataService = dataService
    End Sub
    Public Function CreateViewModel() As DetalleColaboradorViewModel Implements IViewModelFactory(Of DetalleColaboradorViewModel).CreateViewModel
        Return New DetalleColaboradorViewModel(_dataService)
    End Function
End Class
