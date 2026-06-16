Public Class AgregarUsuarioWindow

    Public Sub New()
        InitializeComponent()

        AddHandler Me.Loaded,
            Sub(s As Object, e As RoutedEventArgs)
                Dim vm = TryCast(DataContext, UsuariosViewModel)
                If vm IsNot Nothing Then
                    AddHandler vm.GuardadoExitoso, Sub(ss, ev) Me.Close()
                End If
            End Sub

        AddHandler BtnCancelar.Click,
            Sub(s As Object, e As RoutedEventArgs)
                Me.Close()
            End Sub

        ' Sincronizar PasswordBox → ViewModel (PasswordBox no soporta binding directo)
        AddHandler PwdBox.PasswordChanged,
            Sub(s As Object, e As RoutedEventArgs)
                Dim vm = TryCast(DataContext, UsuariosViewModel)
                If vm IsNot Nothing Then
                    vm.Contrasena = PwdBox.Password
                End If
            End Sub
    End Sub

End Class
