Imports Vemar.Domain

Public Class ListadoTramitesViewModelFactory : Implements IViewModelFactory(Of ListadoTramitesViewModel)
    Private ReadOnly _dataService As IDataService(Of Tramite)
    Public Sub New(dataService As IDataService(Of Tramite))
        _dataService = dataService
    End Sub
    Public Function CreateViewModel() As ListadoTramitesViewModel Implements IViewModelFactory(Of ListadoTramitesViewModel).CreateViewModel
        Return New ListadoTramitesViewModel(_dataService)
    End Function
End Class
