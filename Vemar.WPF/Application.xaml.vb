Imports Microsoft.Extensions.DependencyInjection
Imports Vemar.Domain
Imports Vemar.EF

Class Application

    Protected Overrides Async Sub OnStartup(e As StartupEventArgs)
        MyBase.OnStartup(e)

        ' ── 1. Intentar conexión silenciosa con ajustes guardados ─────────
        If ConnectionSettings.HasSavedSettings() Then
            Dim saved = ConnectionSettings.Current
            ConnectionConfig.ConnectionString = saved.ConnectionString

            Dim serverOk = Await ConnectionSettings.TestServerAsync(saved.ConnectionString)
            Dim dbOk = Await ConnectionSettings.DatabaseExistsAsync(saved.ConnectionString)

            If serverOk AndAlso dbOk Then
                Await LaunchMainWindowAsync()
                Return
            End If
        End If

        ' ── 2. Mostrar splash ─────────────────────────────────────────────
        Dim splash As New SplashWindow()
        Dim result = splash.ShowDialog()

        If result <> True Then
            Shutdown()
            Return
        End If

        ' ── 3. Lanzar app principal ───────────────────────────────────────
        Await LaunchMainWindowAsync()
    End Sub

    Private Async Function LaunchMainWindowAsync() As Task
        DIContenedor.ConfigureServices()

        ' ── Aplicar migraciones pendientes (nuevas versiones del sistema) ──
        Try
            Await Task.Run(Async Function()
                               Await VemarDbContextFactory.ApplyMigrationsAsync()
                           End Function)
        Catch ex As Exception
            MessageBox.Show("Advertencia: no se pudieron aplicar las migraciones pendientes." &
                            Environment.NewLine & ex.Message,
                            "Migración", MessageBoxButton.OK, MessageBoxImage.Warning)
        End Try

        ' ── Verificar si existe algún usuario registrado ──────────────────
        Dim userService = DIContenedor.Services.GetRequiredService(Of IDataService(Of Usuario))()
        Dim users As IEnumerable(Of Usuario)

        Try
            users = Await userService.GetAll()
        Catch
            ' Si falla la consulta (p.ej. tabla no existe aún) se omite el login
            users = Enumerable.Empty(Of Usuario)()
        End Try

        If users.Any() Then
            ' ── Hay usuarios → mostrar ventana de login ───────────────────
            Dim loginVm As New LoginViewModel(userService)
            Dim login As New LoginWindow(loginVm)
            Dim loginResult = login.ShowDialog()

            If loginResult <> True Then
                Shutdown()
                Return
            End If
        End If

        ' ── No hay usuarios (o login exitoso) → abrir ventana principal ───
        Dim mainWindow As MainWindow = DIContenedor.Services.GetRequiredService(Of MainWindow)()
        mainWindow.Show()
    End Function

End Class
