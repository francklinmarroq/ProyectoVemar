Imports System.ComponentModel
Imports Vemar.Domain

Public Class DashboardViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged

    Private ReadOnly _clienteService As IDataService(Of Cliente)
    Private ReadOnly _remedidaService As IDataService(Of Remedida)
    Private ReadOnly _proyectoService As IDataService(Of Proyecto)
    Private ReadOnly _colaboradorService As IDataService(Of Colaborador)
    Private ReadOnly _contratistaService As IDataService(Of Contratista)
    Private ReadOnly _tramiteService As IDataService(Of Tramite)

    Private _totalClientes As Integer
    Private _totalRemedidas As Integer
    Private _totalProyectos As Integer
    Private _totalColaboradores As Integer
    Private _totalContratistas As Integer
    Private _totalTramites As Integer
    Private _fechaHoy As String

    Public Property TotalClientes As Integer
        Get
            Return _totalClientes
        End Get
        Set(value As Integer)
            _totalClientes = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TotalClientes)))
        End Set
    End Property

    Public Property TotalRemedidas As Integer
        Get
            Return _totalRemedidas
        End Get
        Set(value As Integer)
            _totalRemedidas = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TotalRemedidas)))
        End Set
    End Property

    Public Property TotalProyectos As Integer
        Get
            Return _totalProyectos
        End Get
        Set(value As Integer)
            _totalProyectos = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TotalProyectos)))
        End Set
    End Property

    Public Property TotalColaboradores As Integer
        Get
            Return _totalColaboradores
        End Get
        Set(value As Integer)
            _totalColaboradores = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TotalColaboradores)))
        End Set
    End Property

    Public Property TotalContratistas As Integer
        Get
            Return _totalContratistas
        End Get
        Set(value As Integer)
            _totalContratistas = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TotalContratistas)))
        End Set
    End Property

    Public Property TotalTramites As Integer
        Get
            Return _totalTramites
        End Get
        Set(value As Integer)
            _totalTramites = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TotalTramites)))
        End Set
    End Property

    Public Property FechaHoy As String
        Get
            Return _fechaHoy
        End Get
        Set(value As String)
            _fechaHoy = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(FechaHoy)))
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New(
        clienteService As IDataService(Of Cliente),
        remedidaService As IDataService(Of Remedida),
        proyectoService As IDataService(Of Proyecto),
        colaboradorService As IDataService(Of Colaborador),
        contratistaService As IDataService(Of Contratista),
        tramiteService As IDataService(Of Tramite))

        _clienteService = clienteService
        _remedidaService = remedidaService
        _proyectoService = proyectoService
        _colaboradorService = colaboradorService
        _contratistaService = contratistaService
        _tramiteService = tramiteService

        FechaHoy = Date.Today.ToString("dddd, dd 'de' MMMM 'de' yyyy", New Globalization.CultureInfo("es-HN"))
        CargarEstadisticas()
    End Sub

    Public Async Sub CargarEstadisticas()
        Try
            Dim clientes = Await _clienteService.GetAll()
            TotalClientes = clientes.Count()

            Dim remedidas = Await _remedidaService.GetAll()
            TotalRemedidas = remedidas.Count()

            Dim proyectos = Await _proyectoService.GetAll()
            TotalProyectos = proyectos.Count()

            Dim colaboradores = Await _colaboradorService.GetAll()
            TotalColaboradores = colaboradores.Count()

            Dim contratistas = Await _contratistaService.GetAll()
            TotalContratistas = contratistas.Count()

            Dim tramites = Await _tramiteService.GetAll()
            TotalTramites = tramites.Count()
        Catch ex As Exception
            ' Silently handle - dashboard counts are non-critical
        End Try
    End Sub
End Class

