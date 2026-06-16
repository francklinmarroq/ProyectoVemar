Imports Vemar.Domain

Public Class ListadoColaboradoresViewModelFactory : Implements IViewModelFactory(Of ListadoColaboradoresViewModel)
    Private ReadOnly _dataService As IDataService(Of Colaborador)
    Public Sub New(dataService As IDataService(Of Colaborador))
        _dataService = dataService
    End Sub
    Public Function CreateViewModel() As ListadoColaboradoresViewModel Implements IViewModelFactory(Of ListadoColaboradoresViewModel).CreateViewModel
        Return New ListadoColaboradoresViewModel(_dataService)
    End Function
End Class
