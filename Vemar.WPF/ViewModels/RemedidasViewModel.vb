Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Data
Imports Vemar.Domain
Imports Vemar.WPF.Reports

Public Class RemedidasViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged

    Private ReadOnly _service As IDataService(Of Remedida)
    Private ReadOnly _cobroService As IDataService(Of CobroRemedida)
    Private ReadOnly _gastoService As IDataService(Of GastoRemedida)
    Private ReadOnly _movimientoService As IDataService(Of Movimiento)
    Private ReadOnly _tipoMovimientoService As IDataService(Of TipoMovimiento)
    Private _itemsSource As New ObservableCollection(Of Remedida)
    Private _itemsView As ICollectionView
    Private _busqueda As String = ""
    Private _guardarCommand As RelayCommand
    Private _remedidaEditando As Remedida = Nothing

    Private _propietario As String = ""
    Private _representante As String = ""
    Private _rtn As String = ""
    Private _ubicacion As String = ""
    Private _claveSure As String = ""
    Private _matricula As String = ""
    Private _cam As String = ""
    Private _objeto As String = ""
    Private _fecha As DateTime = DateTime.Today
    Private _precio As String = ""
    Private _expedienteEntregado As Boolean
    Private _tituloFormulario As String = "Nueva Remedida"

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public ReadOnly Property ItemsView As ICollectionView
        Get
            Return _itemsView
        End Get
    End Property

    Public Property Busqueda As String
        Get
            Return _busqueda
        End Get
        Set(value As String)
            _busqueda = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Busqueda)))
            _itemsView?.Refresh()
        End Set
    End Property

    Public Property TituloFormulario As String
        Get
            Return _tituloFormulario
        End Get
        Set(value As String)
            _tituloFormulario = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TituloFormulario)))
        End Set
    End Property

    Public Property Propietario As String
        Get
            Return _propietario
        End Get
        Set(value As String)
            _propietario = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Propietario)))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public Property Representante As String
        Get
            Return _representante
        End Get
        Set(value As String)
            _representante = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Representante)))
        End Set
    End Property

    Public Property Rtn As String
        Get
            Return _rtn
        End Get
        Set(value As String)
            _rtn = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Rtn)))
        End Set
    End Property

    Public Property Ubicacion As String
        Get
            Return _ubicacion
        End Get
        Set(value As String)
            _ubicacion = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Ubicacion)))
        End Set
    End Property

    Public Property ClaveSure As String
        Get
            Return _claveSure
        End Get
        Set(value As String)
            _claveSure = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ClaveSure)))
        End Set
    End Property

    Public Property Matricula As String
        Get
            Return _matricula
        End Get
        Set(value As String)
            _matricula = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Matricula)))
        End Set
    End Property

    Public Property Fecha As DateTime
        Get
            Return _fecha
        End Get
        Set(value As DateTime)
            _fecha = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Fecha)))
        End Set
    End Property

    Public Property Precio As String
        Get
            Return _precio
        End Get
        Set(value As String)
            _precio = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Precio)))
        End Set
    End Property

    Public Property Cam As String
        Get
            Return _cam
        End Get
        Set(value As String)
            _cam = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Cam)))
        End Set
    End Property

    Public Property Objeto As String
        Get
            Return _objeto
        End Get
        Set(value As String)
            _objeto = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Objeto)))
        End Set
    End Property

    Public Property ExpedienteEntregado As Boolean
        Get
            Return _expedienteEntregado
        End Get
        Set(value As Boolean)
            _expedienteEntregado = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ExpedienteEntregado)))
        End Set
    End Property

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property ModificarCommand As ICommand
    Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property VerCobrosCommand As ICommand
    Public ReadOnly Property VerGastosCommand As ICommand
    Public ReadOnly Property VerMovimientosCommand As ICommand
    Public ReadOnly Property ExportarExcelCommand As ICommand
    Public ReadOnly Property ExportarPdfCommand As ICommand
    Public ReadOnly Property ReporteFinancieroPdfCommand As ICommand
    Public ReadOnly Property ReporteFinancieroExcelCommand As ICommand
    Public ReadOnly Property BoletaPdfCommand As ICommand
    Public ReadOnly Property MovimientosPdfCommand As ICommand
    Public ReadOnly Property MovimientosExcelCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of Remedida),
                   cobroService As IDataService(Of CobroRemedida),
                   gastoService As IDataService(Of GastoRemedida),
                   movimientoService As IDataService(Of Movimiento),
                   tipoMovimientoService As IDataService(Of TipoMovimiento))
        _service = service
        _cobroService = cobroService
        _gastoService = gastoService
        _movimientoService = movimientoService
        _tipoMovimientoService = tipoMovimientoService

        AgregarCommand = New RelayCommand(Sub(o)
                                             _remedidaEditando = Nothing
                                             TituloFormulario = "Nueva Remedida"
                                             LimpiarFormulario()
                                             Dim win As New AgregarRemedidaWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow

                                             win.ShowDialog()
                                         End Sub)

        ModificarCommand = New RelayCommand(Sub(o)
                                               Dim r = TryCast(o, Remedida)
                                               If r Is Nothing Then Return
                                               _remedidaEditando = r
                                               TituloFormulario = "Modificar Remedida"
                                               Propietario = r.Propietario
                                               Representante = r.Representante
                                               Ubicacion = r.Ubicacion
                                               ClaveSure = r.ClaveSure
                                               Matricula = r.Matricula
                                               Cam = r.Cam
                                               Objeto = r.Objeto
                                               Fecha = r.Fecha
                                               Precio = r.Precio.ToString()
                                               ExpedienteEntregado = r.ExpedienteEntregado
                                               Dim win As New AgregarRemedidaWindow()
                                               win.DataContext = Me
                                               win.Owner = Application.Current.MainWindow

                                               win.ShowDialog()
                                           End Sub)

        EliminarCommand = New RelayCommand(AddressOf Eliminar)

        Dim _reportRem As New Vemar.WPF.Reports.RemedidasReport()
        ExportarExcelCommand = New RelayCommand(Sub(o)
                                                   _reportRem.GenerateExcelAsync(_itemsSource.ToList(), "Remedidas")
                                               End Sub)
        ExportarPdfCommand = New RelayCommand(Sub(o)
                                                  _reportRem.GeneratePdfAsync(_itemsSource.ToList(), "Remedidas")
                                              End Sub)

        VerCobrosCommand = New RelayCommand(Sub(o)
                                               Dim r = TryCast(o, Remedida)
                                               If r Is Nothing Then Return
                                               Dim vm As New CobrosRemedidaViewModel(_cobroService, r)
                                               Dim win As New CobrosRemedidaWindow()
                                               win.DataContext = vm
                                               AddHandler vm.GuardadoExitoso, Sub(s, e)
                                                                                   Dim agregarWin = Application.Current.Windows.OfType(Of AgregarCobroRemedidaWindow)().FirstOrDefault()
                                                                                   agregarWin?.Close()
                                                                               End Sub
                                               win.Owner = Application.Current.MainWindow

                                               win.ShowDialog()
                                           End Sub)
        VerGastosCommand = New RelayCommand(Sub(o)
                                               Dim r = TryCast(o, Remedida)
                                               If r Is Nothing Then Return
                                               Dim vm As New GastosRemedidaViewModel(_gastoService, r)
                                               Dim win As New GastosRemedidaWindow()
                                               win.DataContext = vm
                                               AddHandler vm.GuardadoExitoso, Sub(s, e)
                                                                                   Dim agregarWin = Application.Current.Windows.OfType(Of AgregarGastoRemedidaRWindow)().FirstOrDefault()
                                                                                   agregarWin?.Close()
                                                                               End Sub
                                               win.Owner = Application.Current.MainWindow

                                               win.ShowDialog()
                                           End Sub)

        VerMovimientosCommand = New RelayCommand(Sub(o)
                                                   Dim r = TryCast(o, Remedida)
                                                   If r Is Nothing Then Return
                                                   Dim vm As New MovimientosRemedidaViewModel(_movimientoService, _tipoMovimientoService, r)
                                                   Dim win As New MovimientosRemedidaWindow()
                                                   win.DataContext = vm
                                                   win.Owner = Application.Current.MainWindow
                                                   win.ShowDialog()
                                               End Sub)

        ReporteFinancieroPdfCommand = New RelayCommand(Async Sub(o)
                                                          Dim r = TryCast(o, Remedida)
                                                          If r Is Nothing Then Return
                                                          Try
                                                              Dim todosGastos = Await _gastoService.GetAll()
                                                              Dim gastos = todosGastos.Where(Function(g) g.Remedida?.Id = r.Id).ToList()
                                                              Dim todosCobros = Await _cobroService.GetAll()
                                                              Dim cobros = todosCobros.Where(Function(c) c.Remedida?.Id = r.Id).ToList()
                                                              Dim rpt As New Vemar.WPF.Reports.ReporteFinancieroRemedidaReport()
                                                              Await rpt.GeneratePdfAsync(r, gastos, cobros)
                                                          Catch ex As Exception
                                                              MessageBox.Show("Error al generar reporte PDF: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                          End Try
                                                      End Sub)

        ReporteFinancieroExcelCommand = New RelayCommand(Async Sub(o)
                                                             Dim r = TryCast(o, Remedida)
                                                             If r Is Nothing Then Return
                                                             Try
                                                                 Dim todosGastos = Await _gastoService.GetAll()
                                                                 Dim gastos = todosGastos.Where(Function(g) g.Remedida?.Id = r.Id).ToList()
                                                                 Dim todosCobros = Await _cobroService.GetAll()
                                                                 Dim cobros = todosCobros.Where(Function(c) c.Remedida?.Id = r.Id).ToList()
                                                                 Dim rpt As New Vemar.WPF.Reports.ReporteFinancieroRemedidaReport()
                                                                 Await rpt.GenerateExcelAsync(r, gastos, cobros)
                                                             Catch ex As Exception
                                                                 MessageBox.Show("Error al generar reporte Excel: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                             End Try
                                                         End Sub)

        BoletaPdfCommand = New RelayCommand(Async Sub(o)
                                                Dim r = TryCast(o, Remedida)
                                                If r Is Nothing Then Return
                                                Try
                                                    Dim rpt As New Vemar.WPF.Reports.BoletaRemedidaReport()
                                                    Await rpt.GeneratePdfAsync(r)
                                                Catch ex As Exception
                                                    MessageBox.Show("Error al generar boleta: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                End Try
                                            End Sub)

        ' ── Movimientos por remedida ──────────────────────────────────────
        MovimientosPdfCommand = New RelayCommand(Async Sub(o)
                                                     Dim r = TryCast(o, Remedida)
                                                     If r Is Nothing Then Return
                                                     Try
                                                         Dim todos = Await _movimientoService.GetAll()
                                                         Dim filtrado = todos.Where(Function(m) m.Remedida?.Id = r.Id).ToList()
                                                         Dim rpt As New Vemar.WPF.Reports.MovimientosRemedidaReport(r.Representante)
                                                         Await rpt.GeneratePdfAsync(filtrado, $"Movimientos_{r.Representante}")
                                                     Catch ex As Exception
                                                         MessageBox.Show("Error al generar reporte: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                     End Try
                                                 End Sub)
        MovimientosExcelCommand = New RelayCommand(Async Sub(o)
                                                       Dim r = TryCast(o, Remedida)
                                                       If r Is Nothing Then Return
                                                       Try
                                                           Dim todos = Await _movimientoService.GetAll()
                                                           Dim filtrado = todos.Where(Function(m) m.Remedida?.Id = r.Id).ToList()
                                                           Dim rpt As New Vemar.WPF.Reports.MovimientosRemedidaReport(r.Representante)
                                                           Await rpt.GenerateExcelAsync(filtrado, $"Movimientos_{r.Representante}")
                                                       Catch ex As Exception
                                                           MessageBox.Show("Error al generar reporte: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                       End Try
                                                   End Sub)

        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) Not String.IsNullOrWhiteSpace(Propietario))

        _itemsView = CollectionViewSource.GetDefaultView(_itemsSource)
        _itemsView.Filter = Function(obj)
                                Dim r = TryCast(obj, Remedida)
                                If r Is Nothing Then Return False
                                If String.IsNullOrWhiteSpace(_busqueda) Then Return True
                                Dim q = _busqueda.ToLower()
                                Return (If(r.Propietario?.ToLower().Contains(q), False) OrElse
                                        If(r.Representante?.ToLower().Contains(q), False) OrElse
                                        If(r.ClaveSure?.ToLower().Contains(q), False) OrElse
                                        If(r.Matricula?.ToLower().Contains(q), False) OrElse
                                        If(r.Cam?.ToLower().Contains(q), False))
                            End Function
        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Dim lista = Await _service.GetAll()
            _itemsSource.Clear()
            For Each r In lista
                _itemsSource.Add(r)
            Next
            _itemsView.Refresh()
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ItemsView)))
        Catch ex As Exception
            MessageBox.Show("Error al cargar remedidas: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub LimpiarFormulario()
        Propietario = "" : Representante = "" : Rtn = "" : Ubicacion = "" : ClaveSure = "" : Matricula = ""
        Cam = "" : Objeto = "" : Precio = ""
        Fecha = DateTime.Today : ExpedienteEntregado = False
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim r = TryCast(obj, Remedida)
        If r Is Nothing Then Return
        Dim resultado = MessageBox.Show($"¿Está seguro que desea eliminar la remedida de ""{r.Representante}""?",
                                        "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning)
        If resultado = MessageBoxResult.Yes Then
            Try
                Await _service.Delete(r.Id)
                CargarItems()
            Catch ex As Exception
                MessageBox.Show("Error al eliminar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
    End Sub

    Public Async Sub Guardar(obj As Object)
        Try
            Dim precioDecimal As Decimal = 0
            Decimal.TryParse(Precio, precioDecimal)
            Dim item As New Remedida With {
                .Propietario = Propietario, .Representante = Representante, .Ubicacion = Ubicacion,
                .ClaveSure = ClaveSure, .Matricula = Matricula,
                .Cam = Cam, .Objeto = Objeto,
                .Fecha = Fecha, .Precio = precioDecimal,
                .ExpedienteEntregado = ExpedienteEntregado
            }
            If _remedidaEditando IsNot Nothing Then
                Await _service.Update(_remedidaEditando.Id, item)
            Else
                Await _service.Add(item)
            End If
            RaiseEvent GuardadoExitoso(Me, EventArgs.Empty)
            CargarItems()
        Catch ex As Exception
            MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class
