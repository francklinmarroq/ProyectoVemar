Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Data
Imports Vemar.Domain
Imports Vemar.WPF.Reports

Public Class ProyectosViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged

    Private ReadOnly _service As IDataService(Of Proyecto)
    Private ReadOnly _clienteService As IDataService(Of Cliente)
    Private ReadOnly _zonificacionService As IDataService(Of Zonificacion)
    Private ReadOnly _categoriaService As IDataService(Of CategoriaProyecto)
    Private ReadOnly _contratoService As IDataService(Of Contrato)
    Private ReadOnly _contratistaService As IDataService(Of Contratista)
    Private ReadOnly _pagoService As IDataService(Of PagoContrato)
    Private ReadOnly _gastoService As IDataService(Of GastoProyecto)
    Private ReadOnly _tramiteService As IDataService(Of Tramite)
    Private ReadOnly _asignacionService As IDataService(Of Asignacion)
    Private ReadOnly _avanceService As IDataService(Of Avance)
    Private _itemsSource As New ObservableCollection(Of Proyecto)
    Private _itemsView As ICollectionView
    Private _clientes As New ObservableCollection(Of Cliente)
    Private _zonificaciones As New ObservableCollection(Of Zonificacion)
    Private _categorias As New ObservableCollection(Of CategoriaProyecto)
    Private _guardarCommand As RelayCommand
    Private _proyectoEditando As Proyecto = Nothing

    Private _busqueda As String = ""
    Private _nombre As String = ""
    Private _ubicacion As String = ""
    Private _matricula As String = ""
    Private _claveSure As String = ""
    Private _area As String = ""
    Private _clienteSeleccionado As Cliente
    Private _zonificacionSeleccionada As Zonificacion
    Private _categoriaSeleccionada As CategoriaProyecto
    Private _tituloFormulario As String = "Nuevo Proyecto"

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

    Public Property Clientes As ObservableCollection(Of Cliente)
        Get
            Return _clientes
        End Get
        Set(value As ObservableCollection(Of Cliente))
            _clientes = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Clientes)))
        End Set
    End Property

    Public Property Zonificaciones As ObservableCollection(Of Zonificacion)
        Get
            Return _zonificaciones
        End Get
        Set(value As ObservableCollection(Of Zonificacion))
            _zonificaciones = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Zonificaciones)))
        End Set
    End Property

    Public Property Categorias As ObservableCollection(Of CategoriaProyecto)
        Get
            Return _categorias
        End Get
        Set(value As ObservableCollection(Of CategoriaProyecto))
            _categorias = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Categorias)))
        End Set
    End Property

    Public Property Nombre As String
        Get
            Return _nombre
        End Get
        Set(value As String)
            _nombre = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Nombre)))
            _guardarCommand?.RaiseCanExecuteChanged()
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

    Public Property Matricula As String
        Get
            Return _matricula
        End Get
        Set(value As String)
            _matricula = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Matricula)))
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

    Public Property Area As String
        Get
            Return _area
        End Get
        Set(value As String)
            _area = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Area)))
        End Set
    End Property

    Public Property ClienteSeleccionado As Cliente
        Get
            Return _clienteSeleccionado
        End Get
        Set(value As Cliente)
            _clienteSeleccionado = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ClienteSeleccionado)))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public Property ZonificacionSeleccionada As Zonificacion
        Get
            Return _zonificacionSeleccionada
        End Get
        Set(value As Zonificacion)
            _zonificacionSeleccionada = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ZonificacionSeleccionada)))
        End Set
    End Property

    Public Property CategoriaSeleccionada As CategoriaProyecto
        Get
            Return _categoriaSeleccionada
        End Get
        Set(value As CategoriaProyecto)
            _categoriaSeleccionada = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CategoriaSeleccionada)))
        End Set
    End Property

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property ModificarCommand As ICommand
    Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property VerContratosCommand As ICommand
    Public ReadOnly Property VerGastosCommand As ICommand
    Public ReadOnly Property ExportarExcelCommand As ICommand
    Public ReadOnly Property ExportarPdfCommand As ICommand
    Public ReadOnly Property ReporteFinancieroPdfCommand As ICommand
    Public ReadOnly Property ReporteFinancieroExcelCommand As ICommand
    Public ReadOnly Property BoletaPdfCommand As ICommand
    Public ReadOnly Property TramitesPdfCommand As ICommand
    Public ReadOnly Property TramitesExcelCommand As ICommand
    Public ReadOnly Property AsignacionesPdfCommand As ICommand
    Public ReadOnly Property AsignacionesExcelCommand As ICommand
    Public ReadOnly Property AvancesPdfCommand As ICommand
    Public ReadOnly Property AvancesExcelCommand As ICommand
    Public ReadOnly Property ContratosPdfCommand As ICommand
    Public ReadOnly Property ContratosExcelCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of Proyecto),
                   clienteService As IDataService(Of Cliente),
                   zonificacionService As IDataService(Of Zonificacion),
                   categoriaService As IDataService(Of CategoriaProyecto),
                   contratoService As IDataService(Of Contrato),
                   contratistaService As IDataService(Of Contratista),
                   pagoService As IDataService(Of PagoContrato),
                   gastoService As IDataService(Of GastoProyecto),
                   tramiteService As IDataService(Of Tramite),
                   asignacionService As IDataService(Of Asignacion),
                   avanceService As IDataService(Of Avance))
        _service = service
        _clienteService = clienteService
        _zonificacionService = zonificacionService
        _categoriaService = categoriaService
        _contratoService = contratoService
        _contratistaService = contratistaService
        _pagoService = pagoService
        _gastoService = gastoService
        _tramiteService = tramiteService
        _asignacionService = asignacionService
        _avanceService = avanceService

        AgregarCommand = New RelayCommand(Sub(o)
                                             _proyectoEditando = Nothing
                                             TituloFormulario = "Nuevo Proyecto"
                                             LimpiarFormulario()
                                             Dim win As New AgregarProyectoWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow

                                             win.ShowDialog()
                                         End Sub)

        ModificarCommand = New RelayCommand(Sub(o)
                                               Dim p = TryCast(o, Proyecto)
                                               If p Is Nothing Then Return
                                               _proyectoEditando = p
                                               TituloFormulario = "Modificar Proyecto"
                                               Nombre = p.Nombre
                                               Ubicacion = p.Ubicacion
                                               Matricula = p.Matricula
                                               ClaveSure = p.ClaveSure
                                               Area = p.Area.ToString()
                                               ClienteSeleccionado = Clientes.FirstOrDefault(Function(c) c.Id = p.Cliente?.Id)
                                               ZonificacionSeleccionada = Zonificaciones.FirstOrDefault(Function(z) z.Id = p.Zonificacion?.Id)
                                               CategoriaSeleccionada = Categorias.FirstOrDefault(Function(c) c.Id = p.CategoriaProyecto?.Id)
                                               Dim win As New AgregarProyectoWindow()
                                               win.DataContext = Me
                                               win.Owner = Application.Current.MainWindow

                                               win.ShowDialog()
                                           End Sub)

        EliminarCommand = New RelayCommand(AddressOf Eliminar)

        VerContratosCommand = New RelayCommand(Sub(o)
                                                   Dim p = TryCast(o, Proyecto)
                                                   If p Is Nothing Then Return
                                                   Dim vm As New ContratosProyectoViewModel(_contratoService, _contratistaService, _pagoService, p)
                                                   Dim win As New ContratosProyectoWindow()
                                                   win.DataContext = vm
                                                   AddHandler vm.GuardadoExitoso, Sub(s, e)
                                                                                       Dim agregarWin = Application.Current.Windows.OfType(Of AgregarContratoProyectoWindow)().FirstOrDefault()
                                                                                       agregarWin?.Close()
                                                                                   End Sub
                                                   win.Owner = Application.Current.MainWindow

                                                   win.ShowDialog()
                                               End Sub)

        VerGastosCommand = New RelayCommand(Sub(o)
                                               Dim p = TryCast(o, Proyecto)
                                               If p Is Nothing Then Return
                                               Dim vm As New GastosProyectoViewModel(_gastoService, p)
                                               Dim win As New GastosProyectoWindow()
                                               win.DataContext = vm
                                               AddHandler vm.GuardadoExitoso, Sub(s, e)
                                                                                   Dim agregarWin = Application.Current.Windows.OfType(Of AgregarGastoProyectoWindow)().FirstOrDefault()
                                                                                   agregarWin?.Close()
                                                                               End Sub
                                               win.Owner = Application.Current.MainWindow

                                               win.ShowDialog()
                                           End Sub)

        ReporteFinancieroPdfCommand = New RelayCommand(Async Sub(o)
                                                          Dim p = TryCast(o, Proyecto)
                                                          If p Is Nothing Then Return
                                                          Try
                                                              Dim todosGastos = Await _gastoService.GetAll()
                                                              Dim gastos = todosGastos.Where(Function(g) g.Proyecto?.Id = p.Id).ToList()
                                                              Dim todosPagos = Await _pagoService.GetAll()
                                                              Dim pagos = todosPagos.Where(Function(pg) pg.Contrato?.Proyecto?.Id = p.Id).ToList()
                                                              Dim rpt As New Vemar.WPF.Reports.ReporteFinancieroProyectoReport()
                                                              Await rpt.GeneratePdfAsync(p, gastos, pagos)
                                                          Catch ex As Exception
                                                              MessageBox.Show("Error al generar reporte PDF: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                          End Try
                                                      End Sub)

        ReporteFinancieroExcelCommand = New RelayCommand(Async Sub(o)
                                                             Dim p = TryCast(o, Proyecto)
                                                             If p Is Nothing Then Return
                                                             Try
                                                                 Dim todosGastos = Await _gastoService.GetAll()
                                                                 Dim gastos = todosGastos.Where(Function(g) g.Proyecto?.Id = p.Id).ToList()
                                                                 Dim todosPagos = Await _pagoService.GetAll()
                                                                 Dim pagos = todosPagos.Where(Function(pg) pg.Contrato?.Proyecto?.Id = p.Id).ToList()
                                                                 Dim rpt As New Vemar.WPF.Reports.ReporteFinancieroProyectoReport()
                                                                 Await rpt.GenerateExcelAsync(p, gastos, pagos)
                                                             Catch ex As Exception
                                                                 MessageBox.Show("Error al generar reporte Excel: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                             End Try
                                                         End Sub)

        BoletaPdfCommand = New RelayCommand(Async Sub(o)
                                                Dim p = TryCast(o, Proyecto)
                                                If p Is Nothing Then Return
                                                Try
                                                    Dim rpt As New Vemar.WPF.Reports.BoletaProyectoReport()
                                                    Await rpt.GeneratePdfAsync(p)
                                                Catch ex As Exception
                                                    MessageBox.Show("Error al generar boleta: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                End Try
                                            End Sub)

        ' ── Trámites por proyecto ─────────────────────────────────────────
        TramitesPdfCommand = New RelayCommand(Async Sub(o)
                                                  Dim p = TryCast(o, Proyecto)
                                                  If p Is Nothing Then Return
                                                  Try
                                                      Dim todos = Await _tramiteService.GetAll()
                                                      Dim filtrado = todos.Where(Function(t) t.Proyecto?.Id = p.Id).ToList()
                                                      Dim rpt As New Vemar.WPF.Reports.TramitesProyectoReport(p.Nombre)
                                                      Await rpt.GeneratePdfAsync(filtrado, $"Tramites_{p.Nombre}")
                                                  Catch ex As Exception
                                                      MessageBox.Show("Error al generar reporte: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                  End Try
                                              End Sub)
        TramitesExcelCommand = New RelayCommand(Async Sub(o)
                                                    Dim p = TryCast(o, Proyecto)
                                                    If p Is Nothing Then Return
                                                    Try
                                                        Dim todos = Await _tramiteService.GetAll()
                                                        Dim filtrado = todos.Where(Function(t) t.Proyecto?.Id = p.Id).ToList()
                                                        Dim rpt As New Vemar.WPF.Reports.TramitesProyectoReport(p.Nombre)
                                                        Await rpt.GenerateExcelAsync(filtrado, $"Tramites_{p.Nombre}")
                                                    Catch ex As Exception
                                                        MessageBox.Show("Error al generar reporte: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                    End Try
                                                End Sub)

        ' ── Asignaciones por proyecto ─────────────────────────────────────
        AsignacionesPdfCommand = New RelayCommand(Async Sub(o)
                                                      Dim p = TryCast(o, Proyecto)
                                                      If p Is Nothing Then Return
                                                      Try
                                                          Dim todos = Await _asignacionService.GetAll()
                                                          Dim filtrado = todos.Where(Function(a) a.Proyecto?.Id = p.Id).ToList()
                                                          Dim rpt As New Vemar.WPF.Reports.AsignacionesProyectoReport(p.Nombre)
                                                          Await rpt.GeneratePdfAsync(filtrado, $"Asignaciones_{p.Nombre}")
                                                      Catch ex As Exception
                                                          MessageBox.Show("Error al generar reporte: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                      End Try
                                                  End Sub)
        AsignacionesExcelCommand = New RelayCommand(Async Sub(o)
                                                        Dim p = TryCast(o, Proyecto)
                                                        If p Is Nothing Then Return
                                                        Try
                                                            Dim todos = Await _asignacionService.GetAll()
                                                            Dim filtrado = todos.Where(Function(a) a.Proyecto?.Id = p.Id).ToList()
                                                            Dim rpt As New Vemar.WPF.Reports.AsignacionesProyectoReport(p.Nombre)
                                                            Await rpt.GenerateExcelAsync(filtrado, $"Asignaciones_{p.Nombre}")
                                                        Catch ex As Exception
                                                            MessageBox.Show("Error al generar reporte: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                        End Try
                                                    End Sub)

        ' ── Avances por proyecto ──────────────────────────────────────────
        AvancesPdfCommand = New RelayCommand(Async Sub(o)
                                                 Dim p = TryCast(o, Proyecto)
                                                 If p Is Nothing Then Return
                                                 Try
                                                     Dim todos = Await _avanceService.GetAll()
                                                     Dim filtrado = todos.Where(Function(a) a.Proyecto?.Id = p.Id).ToList()
                                                     Dim rpt As New Vemar.WPF.Reports.AvancesProyectoReport(p.Nombre)
                                                     Await rpt.GeneratePdfAsync(filtrado, $"Avances_{p.Nombre}")
                                                 Catch ex As Exception
                                                     MessageBox.Show("Error al generar reporte: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                 End Try
                                             End Sub)
        AvancesExcelCommand = New RelayCommand(Async Sub(o)
                                                   Dim p = TryCast(o, Proyecto)
                                                   If p Is Nothing Then Return
                                                   Try
                                                       Dim todos = Await _avanceService.GetAll()
                                                       Dim filtrado = todos.Where(Function(a) a.Proyecto?.Id = p.Id).ToList()
                                                       Dim rpt As New Vemar.WPF.Reports.AvancesProyectoReport(p.Nombre)
                                                       Await rpt.GenerateExcelAsync(filtrado, $"Avances_{p.Nombre}")
                                                   Catch ex As Exception
                                                       MessageBox.Show("Error al generar reporte: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                   End Try
                                               End Sub)

        ' ── Contratos por proyecto ────────────────────────────────────────
        ContratosPdfCommand = New RelayCommand(Async Sub(o)
                                                   Dim p = TryCast(o, Proyecto)
                                                   If p Is Nothing Then Return
                                                   Try
                                                       Dim todos = Await _contratoService.GetAll()
                                                       Dim filtrado = todos.Where(Function(c) c.Proyecto?.Id = p.Id).ToList()
                                                       Dim rpt As New Vemar.WPF.Reports.ContratosProyectoReport(p.Nombre)
                                                       Await rpt.GeneratePdfAsync(filtrado, $"Contratos_{p.Nombre}")
                                                   Catch ex As Exception
                                                       MessageBox.Show("Error al generar reporte: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                   End Try
                                               End Sub)
        ContratosExcelCommand = New RelayCommand(Async Sub(o)
                                                     Dim p = TryCast(o, Proyecto)
                                                     If p Is Nothing Then Return
                                                     Try
                                                         Dim todos = Await _contratoService.GetAll()
                                                         Dim filtrado = todos.Where(Function(c) c.Proyecto?.Id = p.Id).ToList()
                                                         Dim rpt As New Vemar.WPF.Reports.ContratosProyectoReport(p.Nombre)
                                                         Await rpt.GenerateExcelAsync(filtrado, $"Contratos_{p.Nombre}")
                                                     Catch ex As Exception
                                                         MessageBox.Show("Error al generar reporte: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                                     End Try
                                                 End Sub)

        Dim _report As New Vemar.WPF.Reports.ProyectosReport()
        ExportarExcelCommand = New RelayCommand(Sub(o)
                                                   _report.GenerateExcelAsync(_itemsSource.ToList(), "Proyectos")
                                               End Sub)
        ExportarPdfCommand = New RelayCommand(Sub(o)
                                                  _report.GeneratePdfAsync(_itemsSource.ToList(), "Proyectos")
                                              End Sub)

        _guardarCommand = New RelayCommand(AddressOf Guardar,
            Function(o) Not String.IsNullOrWhiteSpace(Nombre) AndAlso ClienteSeleccionado IsNot Nothing)

        _itemsView = CollectionViewSource.GetDefaultView(_itemsSource)
        _itemsView.Filter = Function(obj)
                                Dim p = TryCast(obj, Proyecto)
                                If p Is Nothing Then Return False
                                If String.IsNullOrWhiteSpace(_busqueda) Then Return True
                                Dim q = _busqueda.ToLower()
                                Return (p.Cliente?.Nombre?.ToLower().Contains(q) OrElse
                                        p.ClaveSure?.ToLower().Contains(q) OrElse
                                        p.Matricula?.ToLower().Contains(q))
                            End Function
        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Dim lista = Await _service.GetAll()
            _itemsSource.Clear()
            For Each p In lista
                _itemsSource.Add(p)
            Next
            _itemsView.Refresh()
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ItemsView)))
            Clientes = New ObservableCollection(Of Cliente)(Await _clienteService.GetAll())
            Zonificaciones = New ObservableCollection(Of Zonificacion)(Await _zonificacionService.GetAll())
            Categorias = New ObservableCollection(Of CategoriaProyecto)(Await _categoriaService.GetAll())
        Catch ex As Exception
            MessageBox.Show("Error al cargar proyectos: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub LimpiarFormulario()
        Nombre = "" : Ubicacion = "" : Matricula = "" : ClaveSure = "" : Area = ""
        ClienteSeleccionado = Nothing : ZonificacionSeleccionada = Nothing : CategoriaSeleccionada = Nothing
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim p = TryCast(obj, Proyecto)
        If p Is Nothing Then Return
        Dim resultado = MessageBox.Show($"¿Está seguro que desea eliminar el proyecto ""{p.Nombre}""?",
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
            Dim areaDecimal As Decimal = 0
            Decimal.TryParse(Area, areaDecimal)
            Dim item As New Proyecto With {
                .Nombre = Nombre, .Ubicacion = Ubicacion, .Matricula = Matricula,
                .ClaveSure = ClaveSure, .Area = areaDecimal,
                .Cliente = ClienteSeleccionado, .Zonificacion = ZonificacionSeleccionada,
                .CategoriaProyecto = CategoriaSeleccionada
            }
            If _proyectoEditando IsNot Nothing Then
                Await _service.Update(_proyectoEditando.Id, item)
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
