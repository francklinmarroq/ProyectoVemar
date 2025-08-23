Imports Microsoft.Extensions.DependencyInjection
Imports Vemar.Domain
Imports Vemar.EF
Imports Vemar.EF.Services

Module DIContenedor
    Private _serviceProvider As IServiceProvider
    Public ReadOnly Property Services As IServiceProvider
        Get
            If _serviceProvider Is Nothing Then
                Throw New InvalidOperationException("Service provider is not initialized. Please call Initialize first.")
            End If
            Return _serviceProvider
        End Get
    End Property

    Public Sub ConfigureServices()
        Dim services As IServiceCollection = New ServiceCollection()

        services.AddSingleton(Of IWindowService, WindowService)()
        services.AddSingleton(Of IWindowAbstractFactory, WindowAbstractFactory)()
        services.AddSingleton(Of IWindowFactory(Of DetalleClienteWindow), DetalleClienteWindowFactory)
        services.AddSingleton(Of IWindowFactory(Of ListadoClientesWindow), ListadoClientesWindowFactory)()
        services.AddSingleton(Of IWindowFactory(Of DetalleRemedidaWindow), DetalleRemedidaWindowFactory)()
        services.AddSingleton(Of IWindowFactory(Of ListadoRemedidasWindow), ListadoRemedidasWindowFactory)()

        services.AddSingleton(Of IViewModelAbstractFactory, ViewModelAbstractFactory)()
        services.AddSingleton(Of IViewModelFactory(Of DetalleClienteViewModel), DetalleClienteViewModelFactory)()
        services.AddSingleton(Of IViewModelFactory(Of ListadoClientesViewModel), ListadoClientesViewModelFactory)()
        services.AddSingleton(Of IViewModelFactory(Of DetalleRemedidaViewModel), DetalleRemedidaViewModelFactory)()
        services.AddSingleton(Of IViewModelFactory(Of ListadoRemedidasViewModel), ListadoRemedidasViewModelFactory)()
        services.AddSingleton(Of IViewModelFactory(Of ConfigViewModel), ConfigViewModelFactory)()

        services.AddSingleton(Of VemarDbContextFactory)()

        services.AddSingleton(Of IDataService(Of Cliente), ClienteDataService)()
        services.AddSingleton(Of IDataService(Of Remedida), GenericDataService(Of Remedida))()

        services.AddSingleton(Of IDbDiscoveryService, DbDiscoveryService)()
        services.AddSingleton(Of IConfigurationService, ConfigurationService)()

        services.AddSingleton(Of MainViewModel)()
        services.AddSingleton(Of MainWindow)(Function(s) New MainWindow(s.GetRequiredService(Of MainViewModel)()))

        _serviceProvider = services.BuildServiceProvider()
    End Sub


End Module
