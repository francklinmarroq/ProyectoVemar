Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class CajaChicaViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged

    Private ReadOnly _service As IDataService(Of CajaChica)
    Private ReadOnly _remedidaService As IDataService(Of Remedida)
    Private ReadOnly _proyectoService As IDataService(Of Proyecto)

    Private _allItems As New List(Of CajaChica)
    Private _items As New ObservableCollection(Of CajaChica)
    Private _remedidas As New ObservableCollection(Of Remedida)
    Private _proyectos As New ObservableCollection(Of Proyecto)
    Private _guardarCommand As RelayCommand
    Private _eliminarCommand As RelayCommand

    ' ── Formulario ───────────────────────────────────────────────────────────
    Private _fecha As Date = Date.Today
    Private _concepto As String = ""
    Private _monto As String = ""
    Private _tipoOperacion As String = "Entrada"
    Private _numeroFactura As String = ""
    Private _vinculoTipo As String = "Ninguno"     ' "Ninguno" | "Remedida" | "Proyecto"
    Private _remedidaSel As Remedida
    Private _proyectoSel As Proyecto
    Private _itemSel As CajaChica

    ' ── Filtros ───────────────────────────────────────────────────────────────
    Private _filtroDesde As Date? = Nothing
    Private _filtroHasta As Date? = Nothing
    Private _filtroTipo As String = "Todos"        ' "Todos" | "Entrada" | "Salida"
    Private _filtroTexto As String = ""

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Private Sub OnProp(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

    ' ── Items y catálogos ────────────────────────────────────────────────────
    Public Property Items As ObservableCollection(Of CajaChica)
        Get
            Return _items
        End Get
        Set(v As ObservableCollection(Of CajaChica))
            _items = v
            OnProp(NameOf(Items))
            OnProp(NameOf(TotalEntradas))
            OnProp(NameOf(TotalSalidas))
            OnProp(NameOf(Saldo))
            OnProp(NameOf(SaldoColor))
            OnProp(NameOf(SaldoLabel))
        End Set
    End Property

    Public Property Remedidas As ObservableCollection(Of Remedida)
        Get
            Return _remedidas
        End Get
        Set(v As ObservableCollection(Of Remedida))
            _remedidas = v
            OnProp(NameOf(Remedidas))
        End Set
    End Property

    Public Property Proyectos As ObservableCollection(Of Proyecto)
        Get
            Return _proyectos
        End Get
        Set(v As ObservableCollection(Of Proyecto))
            _proyectos = v
            OnProp(NameOf(Proyectos))
        End Set
    End Property

    ' ── Totales ──────────────────────────────────────────────────────────────
    Public ReadOnly Property TotalEntradas As Decimal
        Get
            Return _items.Where(Function(i) i.TipoOperacion = "Entrada").Sum(Function(i) i.Monto)
        End Get
    End Property

    Public ReadOnly Property TotalSalidas As Decimal
        Get
            Return _items.Where(Function(i) i.TipoOperacion = "Salida").Sum(Function(i) i.Monto)
        End Get
    End Property

    Public ReadOnly Property Saldo As Decimal
        Get
            Return TotalEntradas - TotalSalidas
        End Get
    End Property

    Public ReadOnly Property SaldoColor As String
        Get
            Return If(Saldo >= 0, "#16A34A", "#DC2626")
        End Get
    End Property

    Public ReadOnly Property SaldoLabel As String
        Get
            Return If(Saldo >= 0, "SUPERÁVIT", "DÉFICIT")
        End Get
    End Property

    ' ── Formulario ───────────────────────────────────────────────────────────
    Public Property Fecha As Date
        Get
            Return _fecha
        End Get
        Set(v As Date)
            _fecha = v
            OnProp(NameOf(Fecha))
        End Set
    End Property

    Public Property Concepto As String
        Get
            Return _concepto
        End Get
        Set(v As String)
            _concepto = v
            OnProp(NameOf(Concepto))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public Property Monto As String
        Get
            Return _monto
        End Get
        Set(v As String)
            _monto = v
            OnProp(NameOf(Monto))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public Property TipoOperacion As String
        Get
            Return _tipoOperacion
        End Get
        Set(v As String)
            _tipoOperacion = v
            OnProp(NameOf(TipoOperacion))
        End Set
    End Property

    Public Property NumeroFactura As String
        Get
            Return _numeroFactura
        End Get
        Set(v As String)
            _numeroFactura = v
            OnProp(NameOf(NumeroFactura))
        End Set
    End Property

    Public Property VinculoTipo As String
        Get
            Return _vinculoTipo
        End Get
        Set(v As String)
            _vinculoTipo = v
            OnProp(NameOf(VinculoTipo))
            OnProp(NameOf(ShowRemedidas))
            OnProp(NameOf(ShowProyectos))
        End Set
    End Property

    Public ReadOnly Property ShowRemedidas As Boolean
        Get
            Return _vinculoTipo = "Remedida"
        End Get
    End Property

    Public ReadOnly Property ShowProyectos As Boolean
        Get
            Return _vinculoTipo = "Proyecto"
        End Get
    End Property

    Public Property RemedidaSeleccionada As Remedida
        Get
            Return _remedidaSel
        End Get
        Set(v As Remedida)
            _remedidaSel = v
            OnProp(NameOf(RemedidaSeleccionada))
        End Set
    End Property

    Public Property ProyectoSeleccionado As Proyecto
        Get
            Return _proyectoSel
        End Get
        Set(v As Proyecto)
            _proyectoSel = v
            OnProp(NameOf(ProyectoSeleccionado))
        End Set
    End Property

    Public Property ItemSeleccionado As CajaChica
        Get
            Return _itemSel
        End Get
        Set(v As CajaChica)
            _itemSel = v
            OnProp(NameOf(ItemSeleccionado))
            _eliminarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    ' ── Filtros ───────────────────────────────────────────────────────────────
    Public Property FiltroDesde As Date?
        Get
            Return _filtroDesde
        End Get
        Set(v As Date?)
            _filtroDesde = v
            OnProp(NameOf(FiltroDesde))
            AplicarFiltro()
        End Set
    End Property

    Public Property FiltroHasta As Date?
        Get
            Return _filtroHasta
        End Get
        Set(v As Date?)
            _filtroHasta = v
            OnProp(NameOf(FiltroHasta))
            AplicarFiltro()
        End Set
    End Property

    Public Property FiltroTipo As String
        Get
            Return _filtroTipo
        End Get
        Set(v As String)
            _filtroTipo = v
            OnProp(NameOf(FiltroTipo))
            AplicarFiltro()
        End Set
    End Property

    Public Property FiltroTexto As String
        Get
            Return _filtroTexto
        End Get
        Set(v As String)
            _filtroTexto = v
            OnProp(NameOf(FiltroTexto))
            AplicarFiltro()
        End Set
    End Property

    ' ── Comandos ─────────────────────────────────────────────────────────────
    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property
    Public ReadOnly Property EliminarCommand As ICommand
        Get
            Return _eliminarCommand
        End Get
    End Property
    Public ReadOnly Property ReporteExcelCommand As ICommand
    Public ReadOnly Property ReportePdfCommand As ICommand
    Public ReadOnly Property LimpiarFiltrosCommand As ICommand

    ' ── Constructor ───────────────────────────────────────────────────────────
    Public Sub New(service As IDataService(Of CajaChica),
                   remedidaService As IDataService(Of Remedida),
                   proyectoService As IDataService(Of Proyecto))
        _service = service
        _remedidaService = remedidaService
        _proyectoService = proyectoService

        AgregarCommand = New RelayCommand(Sub(o) AbrirVentanaAgregar())

        _guardarCommand = New RelayCommand(AddressOf Guardar,
            Function(o) Not String.IsNullOrWhiteSpace(Concepto) AndAlso
                        Decimal.TryParse(Monto.Replace(",", "."), New Decimal))

        _eliminarCommand = New RelayCommand(AddressOf Eliminar,
            Function(o) ItemSeleccionado IsNot Nothing)

        Dim rpt As New Vemar.WPF.Reports.CajaChicaReport()
        ReporteExcelCommand = New RelayCommand(Async Sub(o) Await GenerarReporte("excel"))
        ReportePdfCommand = New RelayCommand(Async Sub(o) Await GenerarReporte("pdf"))

        LimpiarFiltrosCommand = New RelayCommand(Sub(o)
                                                     FiltroDesde = Nothing
                                                     FiltroHasta = Nothing
                                                     FiltroTipo = "Todos"
                                                     FiltroTexto = ""
                                                 End Sub)
        CargarItems()
    End Sub

    ' ── Abrir ventana de agregar ──────────────────────────────────────────────
    Private Sub AbrirVentanaAgregar()
        Fecha = Date.Today
        Concepto = ""
        Monto = ""
        NumeroFactura = ""
        TipoOperacion = "Entrada"
        VinculoTipo = "Ninguno"
        RemedidaSeleccionada = Nothing
        ProyectoSeleccionado = Nothing

        Dim win As New AgregarMovimientoCajaWindow()
        win.DataContext = Me
        win.Owner = Application.Current.MainWindow
        win.ShowDialog()
    End Sub

    ' ── Cargar ────────────────────────────────────────────────────────────────
    Public Async Sub CargarItems()
        Try
            _allItems = New List(Of CajaChica)(Await _service.GetAll())
            Remedidas = New ObservableCollection(Of Remedida)(Await _remedidaService.GetAll())
            Proyectos = New ObservableCollection(Of Proyecto)(Await _proyectoService.GetAll())
            AplicarFiltro()
        Catch ex As Exception
            MessageBox.Show("Error al cargar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ' ── Filtrado ──────────────────────────────────────────────────────────────
    Private Sub AplicarFiltro()
        Dim query = _allItems.AsEnumerable()

        If _filtroDesde.HasValue Then
            query = query.Where(Function(i) i.Fecha >= _filtroDesde.Value)
        End If
        If _filtroHasta.HasValue Then
            query = query.Where(Function(i) i.Fecha <= _filtroHasta.Value)
        End If
        If _filtroTipo <> "Todos" Then
            query = query.Where(Function(i) i.TipoOperacion = _filtroTipo)
        End If
        If Not String.IsNullOrWhiteSpace(_filtroTexto) Then
            Dim txt = _filtroTexto.ToLower()
            query = query.Where(Function(i) i.Concepto IsNot Nothing AndAlso i.Concepto.ToLower().Contains(txt))
        End If

        Items = New ObservableCollection(Of CajaChica)(query.OrderByDescending(Function(i) i.Fecha))
    End Sub

    ' ── Guardar ───────────────────────────────────────────────────────────────
    Public Async Sub Guardar(obj As Object)
        Try
            Dim montoVal As Decimal = 0
            Dim montoStr = Monto.Replace(",", ".")
            If Not Decimal.TryParse(montoStr, Globalization.NumberStyles.Any,
                                    Globalization.CultureInfo.InvariantCulture, montoVal) OrElse montoVal <= 0 Then
                MessageBox.Show("Ingrese un monto válido mayor a 0.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning)
                Return
            End If

            Dim entidad As New CajaChica With {
                .Fecha = Fecha,
                .Concepto = Concepto,
                .Monto = montoVal,
                .TipoOperacion = TipoOperacion,
                .NumeroFactura = If(String.IsNullOrWhiteSpace(NumeroFactura), Nothing, NumeroFactura.Trim()),
                .Remedida = If(VinculoTipo = "Remedida", RemedidaSeleccionada, Nothing),
                .Proyecto = If(VinculoTipo = "Proyecto", ProyectoSeleccionado, Nothing)
            }

            Await _service.Add(entidad)
            RaiseEvent GuardadoExitoso(Me, EventArgs.Empty)
            CargarItems()
        Catch ex As Exception
            MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    ' ── Eliminar ─────────────────────────────────────────────────────────────
    Public Async Sub Eliminar(obj As Object)
        If ItemSeleccionado Is Nothing Then Return
        Dim res = MessageBox.Show($"¿Eliminar el movimiento ""{ItemSeleccionado.Concepto}""?",
                                   "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question)
        If res = MessageBoxResult.Yes Then
            Try
                Await _service.Delete(ItemSeleccionado.Id)
                CargarItems()
            Catch ex As Exception
                MessageBox.Show("Error al eliminar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
    End Sub

    ' ── Reportes ─────────────────────────────────────────────────────────────
    Private Async Function GenerarReporte(tipo As String) As Task
        Dim datos = _items.ToList()
        If datos.Count = 0 Then
            MessageBox.Show("No hay registros para reportar.", "Sin datos", MessageBoxButton.OK, MessageBoxImage.Information)
            Return
        End If

        Dim desde = If(_filtroDesde.HasValue, _filtroDesde.Value.ToString("dd/MM/yyyy"), "")
        Dim hasta = If(_filtroHasta.HasValue, _filtroHasta.Value.ToString("dd/MM/yyyy"), "")

        Dim rpt As New Vemar.WPF.Reports.CajaChicaReport()
        If tipo = "excel" Then
            Await rpt.GenerateExcelAsync(datos, desde, hasta, _allItems)
        Else
            Await rpt.GeneratePdfAsync(datos, desde, hasta, _allItems)
        End If
    End Function

End Class
