Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        Dim window As New MainWindow()
        window.DataContext = New MainViewModel
        window.Show()
        MyBase.OnStartup(e)
    End Sub



End Class
