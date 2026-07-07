Public Class AgregarMovimientoRemedidaWindow
    Private Sub BtnCancelar_Click(sender As Object, e As RoutedEventArgs) Handles BtnCancelar.Click
        Close()
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim vm = TryCast(DataContext, MovimientosRemedidaViewModel)
        If vm IsNot Nothing Then
            AddHandler vm.GuardadoExitoso, Sub(s, ev) Close()
        End If
    End Sub
End Class
