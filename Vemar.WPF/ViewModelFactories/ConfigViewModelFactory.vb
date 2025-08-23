Public Class ConfigViewModelFactory : Implements IViewModelFactory(Of ConfigViewModel)
    Private ReadOnly _configurationService As IConfigurationService
    Private ReadOnly _dbDiscoveryService As IDbDiscoveryService

    Public Function CreateViewModel() As ConfigViewModel Implements IViewModelFactory(Of ConfigViewModel).CreateViewModel
        Return New ConfigViewModel(_configurationService, _dbDiscoveryService)
    End Function
End Class
