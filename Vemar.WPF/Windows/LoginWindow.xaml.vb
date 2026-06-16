Public Class LoginWindow

    Private ReadOnly _vm As LoginViewModel

    Public Sub New(vm As LoginViewModel)
        InitializeComponent()
        _vm = vm
        DataContext = _vm
        AddHandler _vm.LoginSuccess, AddressOf OnLoginSuccess
        TxtUsername.Focus()
    End Sub

    Private Sub OnLoginSuccess(sender As Object, e As EventArgs)
        DialogResult = True
        Close()
    End Sub

    Private Sub Window_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.ChangedButton = MouseButton.Left Then DragMove()
    End Sub

    Private Sub BtnCerrar_Click(sender As Object, e As RoutedEventArgs)
        DialogResult = False
        Close()
    End Sub

    Private Sub PwdBox_PasswordChanged(sender As Object, e As RoutedEventArgs)
        If _vm IsNot Nothing Then
            _vm.Password = PwdBox.Password
        End If
    End Sub

End Class
