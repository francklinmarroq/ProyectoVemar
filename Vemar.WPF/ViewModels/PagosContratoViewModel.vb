Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class PagosContratoViewModel : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of PagoContrato)
    Private ReadOnly _contratoFijo As Contrato
    Private _items As New ObservableCollection(Of PagoContrato)
    Private _guardarCommand As RelayCommand
    Private _valor As String = ""
    Private _descripcion As String = ""
    Private _formaPago As String = ""
    Private _fecha As DateTime = DateTime.Today

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public ReadOnly Property TituloContrato As String
        Get
            Return $"Pagos del Contrato — {_contratoFijo.Contratista?.Nombre} / {_contratoFijo.Proyecto?.Nombre} (L {_contratoFijo.Valor:N2})"
        End Get
    End Property

    Public Property Items As ObservableCollection(Of PagoContrato)
        Get
            Return _items
        End Get
        Set(v As ObservableCollection(Of PagoContrato))
            _items = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Items)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TotalPagado)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Pendiente)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PorcentajePagado)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PendienteColor)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PendienteBgColor)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PendienteEtiqueta)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BarraAncho)))
        End Set
    End Property

    Public ReadOnly Property ValorContrato As Decimal
        Get
            Return _contratoFijo.Valor
        End Get
    End Property

    Public ReadOnly Property TotalPagado As Decimal
        Get
            Return _items.Sum(Function(p) p.Valor)
        End Get
    End Property

    Public ReadOnly Property Pendiente As Decimal
        Get
            Return _contratoFijo.Valor - TotalPagado
        End Get
    End Property

    Public ReadOnly Property PorcentajePagado As Double
        Get
            If _contratoFijo.Valor = 0 Then Return 0
            Return Math.Min(100, Math.Round(CDbl(TotalPagado) / CDbl(_contratoFijo.Valor) * 100, 1))
        End Get
    End Property

    Public ReadOnly Property PendienteColor As String
        Get
            Return If(Pendiente <= 0, "#16A34A", "#DC2626")
        End Get
    End Property

    Public ReadOnly Property PendienteBgColor As String
        Get
            Return If(Pendiente <= 0, "#DCFCE7", "#FEE2E2")
        End Get
    End Property

    Public ReadOnly Property PendienteEtiqueta As String
        Get
            Return If(Pendiente <= 0, "Saldado", "Por pagar")
        End Get
    End Property

    Public ReadOnly Property BarraAncho As Double
        Get
            Return PorcentajePagado
        End Get
    End Property

    Public Property Valor As String
        Get
            Return _valor
        End Get
        Set(v As String)
            _valor = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Valor)))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public Property Descripcion As String
        Get
            Return _descripcion
        End Get
        Set(v As String)
            _descripcion = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Descripcion)))
        End Set
    End Property

    Public Property FormaPago As String
        Get
            Return _formaPago
        End Get
        Set(v As String)
            _formaPago = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(FormaPago)))
        End Set
    End Property

    Public Property Fecha As DateTime
        Get
            Return _fecha
        End Get
        Set(v As DateTime)
            _fecha = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Fecha)))
        End Set
    End Property

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property ReciboPdfCommand As ICommand
    Public ReadOnly Property SolicitudPagoCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of PagoContrato), contratoFijo As Contrato)
        _service = service
        _contratoFijo = contratoFijo

        SolicitudPagoCommand = New RelayCommand(Async Sub(o)
                                                   Try
                                                       Dim rpt As New Vemar.WPF.Reports.SolicitudPagoReport()
                                                       Await rpt.GeneratePdfAsync(_contratoFijo, Items.OrderBy(Function(x) x.Id).ToList())
                                                   Catch ex As Exception
                                                       MessageBox.Show("Error al generar solicitud: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                   End Try
                                               End Sub)

        AgregarCommand = New RelayCommand(Sub(o)
                                             Valor = ""
                                             Descripcion = ""
                                             FormaPago = ""
                                             Fecha = DateTime.Today
                                             Dim win As New AgregarPagoContratoWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow

                                             win.ShowDialog()
                                         End Sub)

        EliminarCommand = New RelayCommand(AddressOf Eliminar)

        ReciboPdfCommand = New RelayCommand(Async Sub(o)
                                                Dim p = TryCast(o, PagoContrato)
                                                If p Is Nothing Then Return
                                                Try
                                                    ' Calcular saldos basados en el orden de los pagos
                                                    Dim pagosOrdenados = Items.OrderBy(Function(x) x.Id).ToList()
                                                    Dim idx = pagosOrdenados.FindIndex(Function(x) x.Id = p.Id)
                                                    Dim totalAntes As Decimal = pagosOrdenados.Take(Math.Max(0, idx)).Sum(Function(x) x.Valor)
                                                    Dim saldoAnterior As Decimal = _contratoFijo.Valor - totalAntes
                                                    Dim saldoActual As Decimal = saldoAnterior - p.Valor

                                                    Dim rpt As New Vemar.WPF.Reports.ReciboPagoContratoReport()
                                                    Await rpt.GeneratePdfAsync(p, _contratoFijo, saldoAnterior, saldoActual)
                                                Catch ex As Exception
                                                    MessageBox.Show("Error al generar recibo: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                End Try
                                            End Sub)

        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) Not String.IsNullOrWhiteSpace(Valor))

        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Dim todos = Await _service.GetAll()
            Dim filtrados = todos.Where(Function(p) p.Contrato?.Id = _contratoFijo.Id)
            Items = New ObservableCollection(Of PagoContrato)(filtrados)
        Catch ex As Exception
            MessageBox.Show("Error al cargar pagos: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim p = TryCast(obj, PagoContrato)
        If p Is Nothing Then Return
        Dim resultado = MessageBox.Show($"¿Eliminar el pago de {p.Valor:C}?",
                                        "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning)
        If resultado = MessageBoxResult.Yes Then
            Try
                Await _service.Delete(p.Id)
                CargarItems()
            Catch ex As Exception
                MessageBox.Show("Error al eliminar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
    End Sub

    Public Async Sub Guardar(obj As Object)
        Try
            Dim v As Decimal = 0
            Decimal.TryParse(Valor, v)
            Await _service.Add(New PagoContrato With {
                .Valor = v,
                .Descripcion = Descripcion,
                .FormaPago = FormaPago,
                .Fecha = Fecha,
                .Contrato = _contratoFijo
            })
            RaiseEvent GuardadoExitoso(Me, EventArgs.Empty)
            CargarItems()
        Catch ex As Exception
            MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class
