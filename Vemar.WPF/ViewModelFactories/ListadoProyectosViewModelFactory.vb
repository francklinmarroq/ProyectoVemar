Imports Vemar.Domain

Public Class ListadoProyectosViewModelFactory : Implements IViewModelFactory(Of ListadoProyectosViewModel)
    Private ReadOnly _dataService As IDataService(Of Proyecto)
    Public Sub New(dataService As IDataService(Of Proyecto))
        _dataService = dataService
    End Sub
    Public Function CreateViewModel() As ListadoProyectosViewModel Implements IViewModelFactory(Of ListadoProyectosViewModel).CreateViewModel
        Return New ListadoProyectosViewModel(_dataService)
    End Function
End Class
