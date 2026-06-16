Public Class AgregarMovimientoCajaWindow

    Public Sub New()
        InitializeComponent()

        AddHandler Me.Loaded,
            Sub(s As Object, e As RoutedEventArgs)
                Dim vm = TryCast(DataContext, CajaChicaViewModel)
                If vm IsNot Nothing Then
                    AddHandler vm.GuardadoExitoso, Sub(ss, ev) Me.Close()
                End If
            End Sub

        AddHandler BtnCancelar.Click,
            Sub(s As Object, e As RoutedEventArgs)
                Me.Close()
            End Sub
    End Sub

    Private Sub RadioEntrada_Checked(sender As Object, e As RoutedEventArgs)
        Dim vm = TryCast(DataContext, CajaChicaViewModel)
        If vm IsNot Nothing Then vm.TipoOperacion = "Entrada"
    End Sub

    Private Sub RadioSalida_Checked(sender As Object, e As RoutedEventArgs)
        Dim vm = TryCast(DataContext, CajaChicaViewModel)
        If vm IsNot Nothing Then vm.TipoOperacion = "Salida"
    End Sub

    Private Sub RadioVinculoNinguno_Checked(sender As Object, e As RoutedEventArgs)
        Dim vm = TryCast(DataContext, CajaChicaViewModel)
        If vm IsNot Nothing Then vm.VinculoTipo = "Ninguno"
    End Sub

    Private Sub RadioVinculoRemedida_Checked(sender As Object, e As RoutedEventArgs)
        Dim vm = TryCast(DataContext, CajaChicaViewModel)
        If vm IsNot Nothing Then vm.VinculoTipo = "Remedida"
    End Sub

    Private Sub RadioVinculoProyecto_Checked(sender As Object, e As RoutedEventArgs)
        Dim vm = TryCast(DataContext, CajaChicaViewModel)
        If vm IsNot Nothing Then vm.VinculoTipo = "Proyecto"
    End Sub

End Class
