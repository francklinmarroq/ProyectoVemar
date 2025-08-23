Imports Microsoft.Extensions.DependencyInjection

Class Application
    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        MyBase.OnStartup(e)
        ' Configurar DI
        DIContenedor.ConfigureServices()


        ' Crear y mostrar la ventana principal
        ' El main viewmodel se inyecta automáticamente en el constructor de MainWindow
        Dim mainWindow As MainWindow = DIContenedor.Services.GetRequiredService(Of MainWindow)()
        mainWindow.Show()
    End Sub

End Class
