Imports Vemar.Domain

Public Class ListadoClientesViewModelFactory : Implements IViewModelFactory(Of ListadoClientesViewModel)
    Private ReadOnly _dataService As IDataService(Of Cliente)

    Public Sub New(dataService As IDataService(Of Cliente))
        _dataService = dataService
    End Sub

    Public Function CreateViewModel() As ListadoClientesViewModel Implements IViewModelFactory(Of ListadoClientesViewModel).CreateViewModel
        Return New ListadoClientesViewModel(_dataService)
    End Function
End Class
