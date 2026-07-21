Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Data
Imports Vemar.Domain
Imports Vemar.WPF.Reports

Public Class TramitesViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged

    Private ReadOnly _service As IDataService(Of Tramite)
    Private ReadOnly _tipoService As IDataService(Of TipoTramite)
    Private ReadOnly _estadoService As IDataService(Of EstadoTramite)
    Private ReadOnly _proyectoService As IDataService(Of Proyecto)
    Private _itemsSource As New ObservableCollection(Of Tramite)
    Private _itemsView As ICollectionView
    Private _tipos As New ObservableCollection(Of TipoTramite)
    Private _estados As New ObservableCollection(Of EstadoTramite)
    Private _proyectos As New ObservableCollection(Of Proyecto)
    Private _guardarCommand As RelayCommand
    Private _tramiteEditando As Tramite = Nothing

    Private _busqueda As String = ""
    Private _descripcion As String = ""
    Private _tipoSeleccionado As TipoTramite
    Private _estadoSeleccionado As EstadoTramite
    Private _proyectoSeleccionado As Proyecto
    Private _fecha As DateTime? = DateTime.Today
    Private _tituloFormulario As String = "Nuevo Trámite"

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

    Public Property Tipos As ObservableCollection(Of TipoTramite)
        Get
            Return _tipos
        End Get
        Set(value As ObservableCollection(Of TipoTramite))
            _tipos = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Tipos)))
        End Set
    End Property

    Public Property Estados As ObservableCollection(Of EstadoTramite)
        Get
            Return _estados
        End Get
        Set(value As ObservableCollection(Of EstadoTramite))
            _estados = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Estados)))
        End Set
    End Property

    Public Property Proyectos As ObservableCollection(Of Proyecto)
        Get
            Return _proyectos
        End Get
        Set(value As ObservableCollection(Of Proyecto))
            _proyectos = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Proyectos)))
        End Set
    End Property

    Public Property Descripcion As String
        Get
            Return _descripcion
        End Get
        Set(value As String)
            _descripcion = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Descripcion)))
        End Set
    End Property

    Public Property TipoSeleccionado As TipoTramite
        Get
            Return _tipoSeleccionado
        End Get
        Set(value As TipoTramite)
            _tipoSeleccionado = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TipoSeleccionado)))
        End Set
    End Property

    Public Property EstadoSeleccionado As EstadoTramite
        Get
            Return _estadoSeleccionado
        End Get
        Set(value As EstadoTramite)
            _estadoSeleccionado = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(EstadoSeleccionado)))
        End Set
    End Property

    Public Property ProyectoSeleccionado As Proyecto
        Get
            Return _proyectoSeleccionado
        End Get
        Set(value As Proyecto)
            _proyectoSeleccionado = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ProyectoSeleccionado)))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public Property Fecha As DateTime?
        Get
            Return _fecha
        End Get
        Set(value As DateTime?)
            _fecha = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Fecha)))
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

    Public Sub New(service As IDataService(Of Tramite),
                   tipoService As IDataService(Of TipoTramite),
                   estadoService As IDataService(Of EstadoTramite),
                   proyectoService As IDataService(Of Proyecto))
        _service = service
        _tipoService = tipoService
        _estadoService = estadoService
        _proyectoService = proyectoService

        AgregarCommand = New RelayCommand(Sub(o)
                                             _tramiteEditando = Nothing
                                             TituloFormulario = "Nuevo Trámite"
                                             Descripcion = ""
                                             TipoSeleccionado = Nothing
                                             EstadoSeleccionado = Nothing
                                             ProyectoSeleccionado = Nothing
                                             Fecha = DateTime.Today
                                             Dim win As New AgregarTramiteWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow

                                             win.ShowDialog()
                                         End Sub)

        ModificarCommand = New RelayCommand(Sub(o)
                                               Dim t = TryCast(o, Tramite)
                                               If t Is Nothing Then Return
                                               _tramiteEditando = t
                                               TituloFormulario = "Modificar Trámite"
                                               Descripcion = t.Descripcion
                                               TipoSeleccionado = Tipos.FirstOrDefault(Function(x) x.Id = t.TipoTramite?.Id)
                                               EstadoSeleccionado = Estados.FirstOrDefault(Function(x) x.Id = t.EstadoTramite?.Id)
                                               ProyectoSeleccionado = Proyectos.FirstOrDefault(Function(x) x.Id = t.Proyecto?.Id)
                                               Fecha = t.Fecha
                                               Dim win As New AgregarTramiteWindow()
                                               win.DataContext = Me
                                               win.Owner = Application.Current.MainWindow

                                               win.ShowDialog()
                                           End Sub)

        EliminarCommand = New RelayCommand(AddressOf Eliminar)

        Dim _reportTram As New Vemar.WPF.Reports.TramitesReport()
        ExportarExcelCommand = New RelayCommand(Sub(o)
                                                   _reportTram.GenerateExcelAsync(_itemsSource.ToList(), "Tramites")
                                               End Sub)
        ExportarPdfCommand = New RelayCommand(Sub(o)
                                                  _reportTram.GeneratePdfAsync(_itemsSource.ToList(), "Tramites")
                                              End Sub)

        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) ProyectoSeleccionado IsNot Nothing)

        _itemsView = CollectionViewSource.GetDefaultView(_itemsSource)
        _itemsView.Filter = Function(obj)
                                Dim t = TryCast(obj, Tramite)
                                If t Is Nothing Then Return False
                                If String.IsNullOrWhiteSpace(_busqueda) Then Return True
                                Dim q = _busqueda.ToLower().Trim()
                                Dim proyNombre As String = If(t.Proyecto IsNot Nothing AndAlso t.Proyecto.Nombre IsNot Nothing, t.Proyecto.Nombre.ToLower(), "")
                                Dim tipoNombre As String = If(t.TipoTramite IsNot Nothing AndAlso t.TipoTramite.Nombre IsNot Nothing, t.TipoTramite.Nombre.ToLower(), "")
                                Dim estadoNombre As String = If(t.EstadoTramite IsNot Nothing AndAlso t.EstadoTramite.Estado IsNot Nothing, t.EstadoTramite.Estado.ToLower(), "")
                                Dim desc As String = If(t.Descripcion IsNot Nothing, t.Descripcion.ToLower(), "")
                                Return proyNombre.Contains(q) OrElse tipoNombre.Contains(q) OrElse estadoNombre.Contains(q) OrElse desc.Contains(q)
                            End Function
        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Dim lista = Await _service.GetAll()
            _itemsSource.Clear()
            For Each t In lista
                _itemsSource.Add(t)
            Next
            _itemsView.Refresh()
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ItemsView)))
            Tipos = New ObservableCollection(Of TipoTramite)(Await _tipoService.GetAll())
            Estados = New ObservableCollection(Of EstadoTramite)(Await _estadoService.GetAll())
            Proyectos = New ObservableCollection(Of Proyecto)(Await _proyectoService.GetAll())
        Catch ex As Exception
            MessageBox.Show("Error al cargar trámites: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim t = TryCast(obj, Tramite)
        If t Is Nothing Then Return
        Dim resultado = MessageBox.Show($"¿Está seguro que desea eliminar el trámite ""{t.Descripcion}""?",
                                        "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning)
        If resultado = MessageBoxResult.Yes Then
            Try
                Await _service.Delete(t.Id)
                CargarItems()
            Catch ex As Exception
                MessageBox.Show("Error al eliminar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
    End Sub

    Public Async Sub Guardar(obj As Object)
        Try
            Dim item As New Tramite With {
                .Descripcion = Descripcion,
                .TipoTramite = TipoSeleccionado,
                .EstadoTramite = EstadoSeleccionado,
                .Proyecto = ProyectoSeleccionado,
                .Fecha = Fecha
            }
            If _tramiteEditando IsNot Nothing Then
                Await _service.Update(_tramiteEditando.Id, item)
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
