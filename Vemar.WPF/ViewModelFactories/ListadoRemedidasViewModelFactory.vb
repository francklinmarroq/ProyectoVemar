Imports Vemar.Domain

Public Class ListadoRemedidasViewModelFactory : Implements IViewModelFactory(Of ListadoRemedidasViewModel)
    Private ReadOnly _dataService As IDataService(Of Remedida)
    Public Sub New(dataService As IDataService(Of Remedida))
        _dataService = dataService
    End Sub
    Public Function CreateViewModel() As ListadoRemedidasViewModel Implements IViewModelFactory(Of ListadoRemedidasViewModel).CreateViewModel
        Return New ListadoRemedidasViewModel(_dataService)
    End Function

End Class
