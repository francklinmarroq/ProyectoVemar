Imports Vemar.Domain
Imports Vemar.EF.Services

Public Class DetalleRemedidaViewModelFactory : Implements IViewModelFactory(Of DetalleRemedidaViewModel)
    Private ReadOnly _clienteDataService As ClienteDataService
    Private ReadOnly _remedidaDataService As IDataService(Of Remedida)


    Public Sub New(dataService As IDataService(Of Cliente), remedidaDataService As IDataService(Of Remedida))
        _clienteDataService = dataService
        _remedidaDataService = remedidaDataService
    End Sub

    Public Function CreateViewModel() As DetalleRemedidaViewModel Implements IViewModelFactory(Of DetalleRemedidaViewModel).CreateViewModel
        Return New DetalleRemedidaViewModel(_clienteDataService, _remedidaDataService)
    End Function

End Class
