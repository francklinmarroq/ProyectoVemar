Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class DetalleProyectoViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged

    Private ReadOnly _proyectoService As IDataService(Of Proyecto)
    Private ReadOnly _clienteService As IDataService(Of Cliente)
    Private ReadOnly _zonificacionService As IDataService(Of Zonificacion)
    Private ReadOnly _categoriaService As IDataService(Of CategoriaProyecto)
    Private _guardarCommand As RelayCommand

    Private _clientes As ObservableCollection(Of Cliente)
    Private _zonificaciones As ObservableCollection(Of Zonificacion)
    Private _categorias As ObservableCollection(Of CategoriaProyecto)
    Private _clienteSeleccionado As Cliente
    Private _zonificacionSeleccionada As Zonificacion
    Private _categoriaSeleccionada As CategoriaProyecto
    Private _nombre As String = String.Empty
    Private _ubicacion As String = String.Empty
    Private _matricula As String = String.Empty
    Private _claveSure As String = String.Empty
    Private _area As String = String.Empty

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

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

    Public Property ClienteSeleccionado As Cliente
        Get
            Return _clienteSeleccionado
        End Get
        Set(value As Cliente)
            _clienteSeleccionado = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ClienteSeleccionado)))
            _guardarCommand.RaiseCanExecuteChanged()
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

    Public Property Nombre As String
        Get
            Return _nombre
        End Get
        Set(value As String)
            If _nombre <> value Then
                _nombre = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Nombre)))
                _guardarCommand.RaiseCanExecuteChanged()
            End If
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

    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(proyectoService As IDataService(Of Proyecto),
                   clienteService As IDataService(Of Cliente),
                   zonificacionService As IDataService(Of Zonificacion),
                   categoriaService As IDataService(Of CategoriaProyecto))
        _proyectoService = proyectoService
        _clienteService = clienteService
        _zonificacionService = zonificacionService
        _categoriaService = categoriaService
        _guardarCommand = New RelayCommand(AddressOf Guardar, AddressOf CanGuardar)
        CargarDatos()
    End Sub

    Public Function CanGuardar() As Boolean
        Return Not String.IsNullOrWhiteSpace(Nombre) AndAlso ClienteSeleccionado IsNot Nothing
    End Function

    Public Async Sub CargarDatos()
        Try
            Dim listaClientes = Await _clienteService.GetAll()
            Clientes = New ObservableCollection(Of Cliente)(listaClientes)

            Dim listaZonificaciones = Await _zonificacionService.GetAll()
            Zonificaciones = New ObservableCollection(Of Zonificacion)(listaZonificaciones)

            Dim listaCategorias = Await _categoriaService.GetAll()
            Categorias = New ObservableCollection(Of CategoriaProyecto)(listaCategorias)
        Catch ex As Exception
            MessageBox.Show("Error al cargar datos: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Guardar()
        Try
            Dim areaDecimal As Decimal = 0
            Decimal.TryParse(Area, areaDecimal)
            Dim proyecto As New Proyecto With {
                .Nombre = Nombre,
                .Ubicacion = Ubicacion,
                .Matricula = Matricula,
                .ClaveSure = ClaveSure,
                .Area = areaDecimal,
                .Cliente = ClienteSeleccionado,
                .Zonificacion = ZonificacionSeleccionada,
                .CategoriaProyecto = CategoriaSeleccionada
            }
            Await _proyectoService.Add(proyecto)
            MessageBox.Show("Proyecto guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information)
        Catch ex As Exception
            MessageBox.Show($"Error al guardar el proyecto: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class

