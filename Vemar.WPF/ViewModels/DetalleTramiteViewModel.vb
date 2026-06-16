Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class DetalleTramiteViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged

    Private ReadOnly _tramiteService As IDataService(Of Tramite)
    Private ReadOnly _tipoTramiteService As IDataService(Of TipoTramite)
    Private ReadOnly _estadoTramiteService As IDataService(Of EstadoTramite)
    Private ReadOnly _proyectoService As IDataService(Of Proyecto)
    Private _guardarCommand As RelayCommand

    Private _tiposTramite As ObservableCollection(Of TipoTramite)
    Private _estadosTramite As ObservableCollection(Of EstadoTramite)
    Private _proyectos As ObservableCollection(Of Proyecto)
    Private _tipoTramiteSeleccionado As TipoTramite
    Private _estadoTramiteSeleccionado As EstadoTramite
    Private _proyectoSeleccionado As Proyecto
    Private _descripcion As String = String.Empty

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property TiposTramite As ObservableCollection(Of TipoTramite)
        Get
            Return _tiposTramite
        End Get
        Set(value As ObservableCollection(Of TipoTramite))
            _tiposTramite = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TiposTramite)))
        End Set
    End Property

    Public Property EstadosTramite As ObservableCollection(Of EstadoTramite)
        Get
            Return _estadosTramite
        End Get
        Set(value As ObservableCollection(Of EstadoTramite))
            _estadosTramite = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(EstadosTramite)))
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

    Public Property TipoTramiteSeleccionado As TipoTramite
        Get
            Return _tipoTramiteSeleccionado
        End Get
        Set(value As TipoTramite)
            _tipoTramiteSeleccionado = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TipoTramiteSeleccionado)))
            _guardarCommand.RaiseCanExecuteChanged()
        End Set
    End Property

    Public Property EstadoTramiteSeleccionado As EstadoTramite
        Get
            Return _estadoTramiteSeleccionado
        End Get
        Set(value As EstadoTramite)
            _estadoTramiteSeleccionado = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(EstadoTramiteSeleccionado)))
        End Set
    End Property

    Public Property ProyectoSeleccionado As Proyecto
        Get
            Return _proyectoSeleccionado
        End Get
        Set(value As Proyecto)
            _proyectoSeleccionado = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ProyectoSeleccionado)))
        End Set
    End Property

    Public Property Descripcion As String
        Get
            Return _descripcion
        End Get
        Set(value As String)
            If _descripcion <> value Then
                _descripcion = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Descripcion)))
                _guardarCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property

    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(tramiteService As IDataService(Of Tramite),
                   tipoTramiteService As IDataService(Of TipoTramite),
                   estadoTramiteService As IDataService(Of EstadoTramite),
                   proyectoService As IDataService(Of Proyecto))
        _tramiteService = tramiteService
        _tipoTramiteService = tipoTramiteService
        _estadoTramiteService = estadoTramiteService
        _proyectoService = proyectoService
        _guardarCommand = New RelayCommand(AddressOf Guardar, AddressOf CanGuardar)
        CargarDatos()
    End Sub

    Public Function CanGuardar() As Boolean
        Return Not String.IsNullOrWhiteSpace(Descripcion)
    End Function

    Public Async Sub CargarDatos()
        Try
            Dim listaTipos = Await _tipoTramiteService.GetAll()
            TiposTramite = New ObservableCollection(Of TipoTramite)(listaTipos)

            Dim listaEstados = Await _estadoTramiteService.GetAll()
            EstadosTramite = New ObservableCollection(Of EstadoTramite)(listaEstados)

            Dim listaProyectos = Await _proyectoService.GetAll()
            Proyectos = New ObservableCollection(Of Proyecto)(listaProyectos)
        Catch ex As Exception
            MessageBox.Show("Error al cargar datos: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Guardar()
        Try
            Dim tramite As New Tramite With {
                .Descripcion = Descripcion,
                .TipoTramite = TipoTramiteSeleccionado,
                .EstadoTramite = EstadoTramiteSeleccionado,
                .Proyecto = ProyectoSeleccionado
            }
            Await _tramiteService.Add(tramite)
            MessageBox.Show("Trámite guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information)
        Catch ex As Exception
            MessageBox.Show($"Error al guardar el trámite: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class

