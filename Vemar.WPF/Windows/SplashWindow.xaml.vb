Public Class SplashWindow

    Private ReadOnly _vm As SplashViewModel

    Public Sub New()
        InitializeComponent()
        _vm = New SplashViewModel()
        DataContext = _vm
        AddHandler _vm.ReadyToLaunch, AddressOf OnReadyToLaunch
    End Sub

    ''' <summary>El ViewModel indica que la conexión está lista → lanzar app principal.</summary>
    Private Sub OnReadyToLaunch(sender As Object, e As EventArgs)
        DialogResult = True
        Close()
    End Sub

    ''' <summary>Permite arrastrar la ventana sin bordes haciendo clic en el panel izquierdo.</summary>
    Private Sub Window_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.ChangedButton = MouseButton.Left Then
            DragMove()
        End If
    End Sub

    ''' <summary>Botón X: salir de la aplicación.</summary>
    Private Sub BtnCerrar_Click(sender As Object, e As RoutedEventArgs)
        Application.Current.Shutdown()
    End Sub

    Private Sub RadioWindows_Checked(sender As Object, e As RoutedEventArgs)
        If _vm IsNot Nothing Then _vm.UseWindowsAuth = True
    End Sub

    Private Sub RadioSql_Checked(sender As Object, e As RoutedEventArgs)
        If _vm IsNot Nothing Then _vm.UseWindowsAuth = False
    End Sub

    ''' <summary>El PasswordBox no admite binding directo; lo sincronizamos aquí.</summary>
    Private Sub PwdBox_PasswordChanged(sender As Object, e As RoutedEventArgs)
        Dim pwdBox = TryCast(sender, PasswordBox)
        If pwdBox IsNot Nothing AndAlso _vm IsNot Nothing Then
            _vm.Password = pwdBox.Password
        End If
    End Sub

End Class
