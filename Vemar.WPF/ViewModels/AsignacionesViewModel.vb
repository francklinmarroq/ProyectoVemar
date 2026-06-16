Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Data
Imports Vemar.Domain
Imports Vemar.WPF.Reports

Public Class AsignacionesViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of Asignacion)
    Private ReadOnly _colaboradorService As IDataService(Of Colaborador)
    Private ReadOnly _proyectoService As IDataService(Of Proyecto)
    Private _itemsSource As New ObservableCollection(Of Asignacion)
    Private _itemsView As ICollectionView
    Private _colaboradores As New ObservableCollection(Of Colaborador)
    Private _proyectos As New ObservableCollection(Of Proyecto)
    Private _guardarCommand As RelayCommand
    Private _asignacionEditando As Asignacion = Nothing

    Private _busqueda As String = ""
    Private _colaboradorSel As Colaborador
    Private _proyectoSel As Proyecto
    Private _fechaAsignacion As DateTime = DateTime.Today
    Private _fechaFinalizacion As DateTime? = Nothing
    Private _observaciones As String = ""
    Private _tituloFormulario As String = "Nueva Asignación"

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
        Set(v As String)
            _busqueda = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Busqueda)))
            _itemsView?.Refresh()
        End Set
    End Property

    Public Property TituloFormulario As String
        Get
            Return _tituloFormulario
        End Get
        Set(v As String)
            _tituloFormulario = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TituloFormulario)))
        End Set
    End Property

    Public Property Colaboradores As ObservableCollection(Of Colaborador)
        Get
            Return _colaboradores
        End Get
        Set(v As ObservableCollection(Of Colaborador))
            _colaboradores = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Colaboradores)))
        End Set
    End Property

    Public Property Proyectos As ObservableCollection(Of Proyecto)
        Get
            Return _proyectos
        End Get
        Set(v As ObservableCollection(Of Proyecto))
            _proyectos = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Proyectos)))
        End Set
    End Property

    Public Property ColaboradorSeleccionado As Colaborador
        Get
            Return _colaboradorSel
        End Get
        Set(v As Colaborador)
            _colaboradorSel = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ColaboradorSeleccionado)))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public Property ProyectoSeleccionado As Proyecto
        Get
            Return _proyectoSel
        End Get
        Set(v As Proyecto)
            _proyectoSel = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ProyectoSeleccionado)))
        End Set
    End Property

    Public Property FechaAsignacion As DateTime
        Get
            Return _fechaAsignacion
        End Get
        Set(v As DateTime)
            _fechaAsignacion = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(FechaAsignacion)))
        End Set
    End Property

    Public Property FechaFinalizacion As DateTime?
        Get
            Return _fechaFinalizacion
        End Get
        Set(v As DateTime?)
            _fechaFinalizacion = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(FechaFinalizacion)))
        End Set
    End Property

    Public Property Observaciones As String
        Get
            Return _observaciones
        End Get
        Set(v As String)
            _observaciones = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Observaciones)))
        End Set
    End Property

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property ModificarCommand As ICommand
    Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property ExportarExcelCommand As ICommand
    Public ReadOnly Property ExportarPdfCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of Asignacion),
                   colaboradorService As IDataService(Of Colaborador),
                   proyectoService As IDataService(Of Proyecto))
        _service = service
        _colaboradorService = colaboradorService
        _proyectoService = proyectoService

        AgregarCommand = New RelayCommand(Sub(o)
                                             _asignacionEditando = Nothing
                                             TituloFormulario = "Nueva Asignación"
                                             ColaboradorSeleccionado = Nothing
                                             ProyectoSeleccionado = Nothing
                                             Observaciones = ""
                                             FechaAsignacion = DateTime.Today
                                             FechaFinalizacion = Nothing
                                             Dim win As New AgregarAsignacionWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow

                                             win.ShowDialog()
                                         End Sub)

        ModificarCommand = New RelayCommand(Sub(o)
                                               Dim a = TryCast(o, Asignacion)
                                               If a Is Nothing Then Return
                                               _asignacionEditando = a
                                               TituloFormulario = "Modificar Asignación"
                                               ColaboradorSeleccionado = Colaboradores.FirstOrDefault(Function(c) c.Id = a.Colaborador?.Id)
                                               ProyectoSeleccionado = Proyectos.FirstOrDefault(Function(p) p.Id = a.Proyecto?.Id)
                                               Observaciones = a.Observaciones
                                               FechaAsignacion = a.FechaAsignacion
                                               FechaFinalizacion = a.FechaFinalizacion
                                               Dim win As New AgregarAsignacionWindow()
                                               win.DataContext = Me
                                               win.Owner = Application.Current.MainWindow

                                               win.ShowDialog()
                                           End Sub)

        EliminarCommand = New RelayCommand(AddressOf Eliminar)

        Dim _reportAsig As New Vemar.WPF.Reports.AsignacionesReport()
        ExportarExcelCommand = New RelayCommand(Sub(o)
                                                   _reportAsig.GenerateExcelAsync(_itemsSource.ToList(), "Asignaciones")
                                               End Sub)
        ExportarPdfCommand = New RelayCommand(Sub(o)
                                                  _reportAsig.GeneratePdfAsync(_itemsSource.ToList(), "Asignaciones")
                                              End Sub)

        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) ColaboradorSeleccionado IsNot Nothing)

        _itemsView = CollectionViewSource.GetDefaultView(_itemsSource)
        _itemsView.Filter = Function(obj)
                                Dim a = TryCast(obj, Asignacion)
                                If a Is Nothing Then Return False
                                If String.IsNullOrWhiteSpace(_busqueda) Then Return True
                                Dim q = _busqueda.ToLower()
                                Return (a.Colaborador?.Nombre?.ToLower().Contains(q) OrElse
                                        a.Proyecto?.Nombre?.ToLower().Contains(q) OrElse
                                        a.Proyecto?.Cliente?.Nombre?.ToLower().Contains(q))
                            End Function
        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Dim lista = Await _service.GetAll()
            _itemsSource.Clear()
            For Each a In lista
                _itemsSource.Add(a)
            Next
            _itemsView.Refresh()
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ItemsView)))
            Colaboradores = New ObservableCollection(Of Colaborador)(Await _colaboradorService.GetAll())
            Proyectos = New ObservableCollection(Of Proyecto)(Await _proyectoService.GetAll())
        Catch ex As Exception
            MessageBox.Show("Error al cargar asignaciones: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim a = TryCast(obj, Asignacion)
        If a Is Nothing Then Return
        Dim colaboradorNombre = If(a.Colaborador?.Nombre, "esta asignación")
        Dim resultado = MessageBox.Show($"¿Está seguro que desea eliminar la asignación de ""{colaboradorNombre}""?",
                                        "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning)
        If resultado = MessageBoxResult.Yes Then
            Try
                Await _service.Delete(a.Id)
                CargarItems()
            Catch ex As Exception
                MessageBox.Show("Error al eliminar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
    End Sub

    Public Async Sub Guardar(obj As Object)
        Try
            Dim item As New Asignacion With {
                .Colaborador = ColaboradorSeleccionado,
                .Proyecto = ProyectoSeleccionado,
                .FechaAsignacion = FechaAsignacion,
                .FechaFinalizacion = FechaFinalizacion,
                .Observaciones = Observaciones
            }
            If _asignacionEditando IsNot Nothing Then
                Await _service.Update(_asignacionEditando.Id, item)
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
