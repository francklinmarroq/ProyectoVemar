Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Data
Imports Vemar.Domain
Imports Vemar.WPF.Reports

Public Class AvancesViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of Avance)
    Private ReadOnly _proyectoService As IDataService(Of Proyecto)
    Private _itemsSource As New ObservableCollection(Of Avance)
    Private _itemsView As ICollectionView
    Private _proyectos As New ObservableCollection(Of Proyecto)
    Private _guardarCommand As RelayCommand
    Private _avanceEditando As Avance = Nothing

    Private _busqueda As String = ""
    Private _descripcion As String = ""
    Private _proyectoSel As Proyecto
    Private _fecha As DateTime? = Nothing
    Private _tituloFormulario As String = "Nuevo Avance"

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

    Public Property Proyectos As ObservableCollection(Of Proyecto)
        Get
            Return _proyectos
        End Get
        Set(v As ObservableCollection(Of Proyecto))
            _proyectos = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Proyectos)))
        End Set
    End Property

    Public Property Descripcion As String
        Get
            Return _descripcion
        End Get
        Set(v As String)
            _descripcion = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Descripcion)))
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

    Public Property Fecha As DateTime?
        Get
            Return _fecha
        End Get
        Set(v As DateTime?)
            _fecha = v
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

    Public Sub New(service As IDataService(Of Avance), proyectoService As IDataService(Of Proyecto))
        _service = service
        _proyectoService = proyectoService

        AgregarCommand = New RelayCommand(Sub(o)
                                             _avanceEditando = Nothing
                                             TituloFormulario = "Nuevo Avance"
                                             Descripcion = ""
                                             ProyectoSeleccionado = Nothing
                                             Fecha = Nothing
                                             Dim win As New AgregarAvanceWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow

                                             win.ShowDialog()
                                         End Sub)

        ModificarCommand = New RelayCommand(Sub(o)
                                               Dim a = TryCast(o, Avance)
                                               If a Is Nothing Then Return
                                               _avanceEditando = a
                                               TituloFormulario = "Modificar Avance"
                                               Descripcion = a.Descripcion
                                               ProyectoSeleccionado = Proyectos.FirstOrDefault(Function(p) p.Id = a.Proyecto?.Id)
                                               Fecha = a.Fecha
                                               Dim win As New AgregarAvanceWindow()
                                               win.DataContext = Me
                                               win.Owner = Application.Current.MainWindow

                                               win.ShowDialog()
                                           End Sub)

        EliminarCommand = New RelayCommand(AddressOf Eliminar)

        Dim _reportAv As New Vemar.WPF.Reports.AvancesReport()
        ExportarExcelCommand = New RelayCommand(Sub(o)
                                                   _reportAv.GenerateExcelAsync(_itemsSource.ToList(), "Avances")
                                               End Sub)
        ExportarPdfCommand = New RelayCommand(Sub(o)
                                                  _reportAv.GeneratePdfAsync(_itemsSource.ToList(), "Avances")
                                              End Sub)

        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) Not String.IsNullOrWhiteSpace(Descripcion))

        _itemsView = CollectionViewSource.GetDefaultView(_itemsSource)
        _itemsView.Filter = Function(obj)
                                Dim a = TryCast(obj, Avance)
                                If a Is Nothing Then Return False
                                If String.IsNullOrWhiteSpace(_busqueda) Then Return True
                                Return a.Proyecto?.Nombre?.ToLower().Contains(_busqueda.ToLower())
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
            Proyectos = New ObservableCollection(Of Proyecto)(Await _proyectoService.GetAll())
        Catch ex As Exception
            MessageBox.Show("Error al cargar avances: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim a = TryCast(obj, Avance)
        If a Is Nothing Then Return
        Dim resultado = MessageBox.Show($"¿Está seguro que desea eliminar el avance ""{a.Descripcion}""?",
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
            Dim item As New Avance With {
                .Descripcion = Descripcion,
                .Proyecto = ProyectoSeleccionado,
                .Fecha = Fecha
            }
            If _avanceEditando IsNot Nothing Then
                Await _service.Update(_avanceEditando.Id, item)
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
