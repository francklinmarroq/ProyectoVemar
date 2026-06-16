Public Class AgregarZonificacionWindow
    Public Sub New()
        InitializeComponent()
        AddHandler Me.Loaded, Sub(s As Object, e As RoutedEventArgs)
                                  Dim vm = TryCast(DataContext, ZonificacionesViewModel)
                                  If vm IsNot Nothing Then
                                      AddHandler vm.GuardadoExitoso, Sub(ss, ev) Me.Close()
                                  End If
                              End Sub
        AddHandler BtnCancelar.Click, Sub(s As Object, e As RoutedEventArgs)
                                          Me.Close()
                                      End Sub
    End Sub
End Class
