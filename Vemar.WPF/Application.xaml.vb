Imports Microsoft.Extensions.DependencyInjection
Imports Vemar.EF

Class Application
    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        Dim serviceProvider As IServiceProvider = CreateServiceProvider()



        Dim mainWindow As MainWindow = serviceProvider.GetRequiredService(Of MainWindow)()
        mainWindow.Show()

        MyBase.OnStartup(e)
    End Sub

    Public Function CreateServiceProvider() As IServiceProvider
        Dim services As IServiceCollection = New ServiceCollection()

        services.AddSingleton(Of VemarDbContextFactory)
        services.AddSingleton(Of INavigator, Navigator)()
        services.AddSingleton(Of IWindowService, WindowService)
        services.AddSingleton(Of IVemarViewModelAbstractFactory, VemarViewModelAbstractFactory)()
        services.AddSingleton(Of IVemarViewModelFactory(Of InicioViewModel), InicioViewModelFactory)()
        services.AddSingleton(Of IVemarViewModelFactory(Of RemedidasViewModel), RemedidasViewModelFactory)()
        services.AddSingleton(Of IVemarViewModelFactory(Of AgregarClienteViewModel), AgregarClienteViewModelFactory)()
        services.AddSingleton(Of IVemarWindowAbstractFactory, VemarWindowAbstractFactory)()
        services.AddSingleton(Of IWindowFactory(Of AgregarClienteWindow), AgregarClienteWindowFactory)()

        services.AddScoped(Of MainViewModel)()
        services.AddScoped(Of MainWindow)(Function(s) New MainWindow(s.GetRequiredService(Of MainViewModel)()))


        Return services.BuildServiceProvider()
    End Function



End Class
