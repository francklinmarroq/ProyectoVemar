Public Class MontoSolicitudWindow

    Public Property MontoIngresado As Decimal = 0
    Public Property Confirmado As Boolean = False

    Private _saldoPendiente As Decimal

    Public Sub New(saldoPendiente As Decimal)
        InitializeComponent()
        _saldoPendiente = saldoPendiente
        TxtSaldoPendiente.Text = $"L {saldoPendiente:N2}"
        TxtMonto.Text = saldoPendiente.ToString("N2")

        AddHandler BtnCancelar.Click, Sub(s, e) Me.Close()
        AddHandler BtnAceptar.Click, Sub(s, e) Confirmar()
        AddHandler TxtMonto.TextChanged, Sub(s, e) ActualizarResultante()

        AddHandler Me.Loaded, Sub(s, e)
                                  TxtMonto.SelectAll()
                                  TxtMonto.Focus()
                              End Sub
    End Sub

    Private Sub TxtMonto_KeyDown(sender As Object, e As Input.KeyEventArgs)
        If e.Key = Input.Key.Enter Then Confirmar()
        If e.Key = Input.Key.Escape Then Me.Close()
    End Sub

    Private Sub ActualizarResultante()
        Dim monto As Decimal
        If Decimal.TryParse(TxtMonto.Text.Replace(",", ""), Globalization.NumberStyles.Any,
                            Globalization.CultureInfo.InvariantCulture, monto) AndAlso monto > 0 Then
            Dim restante = _saldoPendiente - monto
            TxtSaldoResultante.Text = $"L {restante:N2}"
            TxtSaldoResultante.Foreground = New Media.SolidColorBrush(
                If(restante >= 0, Media.Color.FromRgb(&H16, &HA3, &H4A), Media.Color.FromRgb(&HDC, &H26, &H26)))
            PanelResultante.Visibility = Visibility.Visible
        Else
            PanelResultante.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub Confirmar()
        Dim monto As Decimal
        If Not Decimal.TryParse(TxtMonto.Text.Replace(",", ""), Globalization.NumberStyles.Any,
                                Globalization.CultureInfo.InvariantCulture, monto) OrElse monto <= 0 Then
            MessageBox.Show("Ingrese un monto válido mayor a 0.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning)
            Return
        End If
        MontoIngresado = monto
        Confirmado = True
        Me.Close()
    End Sub

End Class
