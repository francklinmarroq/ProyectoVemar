Imports Vemar.Domain

Public Class DetalleContratistaViewModelFactory : Implements IViewModelFactory(Of DetalleContratistaViewModel)
    Private ReadOnly _dataService As IDataService(Of Contratista)
    Public Sub New(dataService As IDataService(Of Contratista))
        _dataService = dataService
    End Sub
    Public Function CreateViewModel() As DetalleContratistaViewModel Implements IViewModelFactory(Of DetalleContratistaViewModel).CreateViewModel
        Return New DetalleContratistaViewModel(_dataService)
    End Function
End Class
