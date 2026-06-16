Imports Vemar.Domain

Public Class ListadoContratistasViewModelFactory : Implements IViewModelFactory(Of ListadoContratistasViewModel)
    Private ReadOnly _dataService As IDataService(Of Contratista)
    Public Sub New(dataService As IDataService(Of Contratista))
        _dataService = dataService
    End Sub
    Public Function CreateViewModel() As ListadoContratistasViewModel Implements IViewModelFactory(Of ListadoContratistasViewModel).CreateViewModel
        Return New ListadoContratistasViewModel(_dataService)
    End Function
End Class
