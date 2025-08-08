Imports Microsoft.Extensions.DependencyInjection
Imports Vemar.EF

Class Application
    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        Dim serviceProvider As IServiceProvider = CreateServiceProvider()



        Dim window As New MainWindow()
        window.DataContext = serviceProvider.GetRequiredService(Of MainViewModel)()
        window.Show()
        MyBase.OnStartup(e)
    End Sub

    Public Function CreateServiceProvider() As IServiceProvider
        Dim services As IServiceCollection = New ServiceCollection()
        services.AddSingleton(Of VemarDbContextFactory)
        services.AddSingleton(Of INavigator, Navigator)()
        services.AddScoped(Of MainViewModel)()
        Return services.BuildServiceProvider()
    End Function



End Class
