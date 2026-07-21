Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Media
Imports Vemar.Domain

Public Class DashboardBarItem
    Public Property Label As String
    Public Property Count As Integer
    Public Property BarraPixels As Double
    Public Property BarBrush As SolidColorBrush
    Public Property BgBrush As SolidColorBrush
    Public Property TextBrush As SolidColorBrush
End Class

Public Class ActividadItem
    Public Property Icono As String
    Public Property Titulo As String
    Public Property Subtitulo As String
    Public Property FechaTexto As String
    Public Property IconBrush As SolidColorBrush
    Public Property IconBgBrush As SolidColorBrush
End Class

Public Class DashboardViewModel
    Inherits ViewModelBase
    Implements INotifyPropertyChanged

    Private ReadOnly _clienteService As IDataService(Of Cliente)
    Private ReadOnly _remedidaService As IDataService(Of Remedida)
    Private ReadOnly _proyectoService As IDataService(Of Proyecto)
    Private ReadOnly _colaboradorService As IDataService(Of Colaborador)
    Private ReadOnly _contratistaService As IDataService(Of Contratista)
    Private ReadOnly _tramiteService As IDataService(Of Tramite)
    Private ReadOnly _contratoService As IDataService(Of Contrato)

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub Notify(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

    ' ── KPI counters ──────────────────────────────────────────────────
    Private _totalClientes As Integer
    Public Property TotalClientes As Integer
        Get
            Return _totalClientes
        End Get
        Set(value As Integer)
            _totalClientes = value
            Notify(NameOf(TotalClientes))
        End Set
    End Property

    Private _totalRemedidas As Integer
    Public Property TotalRemedidas As Integer
        Get
            Return _totalRemedidas
        End Get
        Set(value As Integer)
            _totalRemedidas = value
            Notify(NameOf(TotalRemedidas))
        End Set
    End Property

    Private _totalProyectos As Integer
    Public Property TotalProyectos As Integer
        Get
            Return _totalProyectos
        End Get
        Set(value As Integer)
            _totalProyectos = value
            Notify(NameOf(TotalProyectos))
        End Set
    End Property

    Private _totalColaboradores As Integer
    Public Property TotalColaboradores As Integer
        Get
            Return _totalColaboradores
        End Get
        Set(value As Integer)
            _totalColaboradores = value
            Notify(NameOf(TotalColaboradores))
        End Set
    End Property

    Private _totalContratistas As Integer
    Public Property TotalContratistas As Integer
        Get
            Return _totalContratistas
        End Get
        Set(value As Integer)
            _totalContratistas = value
            Notify(NameOf(TotalContratistas))
        End Set
    End Property

    Private _totalTramites As Integer
    Public Property TotalTramites As Integer
        Get
            Return _totalTramites
        End Get
        Set(value As Integer)
            _totalTramites = value
            Notify(NameOf(TotalTramites))
        End Set
    End Property

    Private _totalContratos As Integer
    Public Property TotalContratos As Integer
        Get
            Return _totalContratos
        End Get
        Set(value As Integer)
            _totalContratos = value
            Notify(NameOf(TotalContratos))
        End Set
    End Property

    ' ── Expedientes ───────────────────────────────────────────────────
    Private _remedidasEntregadas As Integer
    Public Property RemedidasEntregadas As Integer
        Get
            Return _remedidasEntregadas
        End Get
        Set(value As Integer)
            _remedidasEntregadas = value
            Notify(NameOf(RemedidasEntregadas))
        End Set
    End Property

    Private _remedidasPendientes As Integer
    Public Property RemedidasPendientes As Integer
        Get
            Return _remedidasPendientes
        End Get
        Set(value As Integer)
            _remedidasPendientes = value
            Notify(NameOf(RemedidasPendientes))
        End Set
    End Property

    Private _barraEntregadas As Double
    Public Property BarraEntregadas As Double
        Get
            Return _barraEntregadas
        End Get
        Set(value As Double)
            _barraEntregadas = value
            Notify(NameOf(BarraEntregadas))
        End Set
    End Property

    Private _barraPendientes As Double = 260
    Public Property BarraPendientes As Double
        Get
            Return _barraPendientes
        End Get
        Set(value As Double)
            _barraPendientes = value
            Notify(NameOf(BarraPendientes))
        End Set
    End Property

    Private _porcentajeEntregadas As String = "0%"
    Public Property PorcentajeEntregadas As String
        Get
            Return _porcentajeEntregadas
        End Get
        Set(value As String)
            _porcentajeEntregadas = value
            Notify(NameOf(PorcentajeEntregadas))
        End Set
    End Property

    ' ── Chart ─────────────────────────────────────────────────────────
    Private _tramitesPorEstado As New ObservableCollection(Of DashboardBarItem)
    Public Property TramitesPorEstado As ObservableCollection(Of DashboardBarItem)
        Get
            Return _tramitesPorEstado
        End Get
        Set(value As ObservableCollection(Of DashboardBarItem))
            _tramitesPorEstado = value
            Notify(NameOf(TramitesPorEstado))
        End Set
    End Property

    ' ── Actividad reciente ───────────────────────────────────────────
    Private _actividadReciente As New ObservableCollection(Of ActividadItem)
    Public Property ActividadReciente As ObservableCollection(Of ActividadItem)
        Get
            Return _actividadReciente
        End Get
        Set(value As ObservableCollection(Of ActividadItem))
            _actividadReciente = value
            Notify(NameOf(ActividadReciente))
        End Set
    End Property

    Private _sinActividad As Boolean = True
    Public Property SinActividad As Boolean
        Get
            Return _sinActividad
        End Get
        Set(value As Boolean)
            _sinActividad = value
            Notify(NameOf(SinActividad))
        End Set
    End Property

    ' ── Fecha ─────────────────────────────────────────────────────────
    Private _fechaHoy As String
    Public Property FechaHoy As String
        Get
            Return _fechaHoy
        End Get
        Set(value As String)
            _fechaHoy = value
            Notify(NameOf(FechaHoy))
        End Set
    End Property

    ' ── Constructor ───────────────────────────────────────────────────
    Public Sub New(
        clienteService As IDataService(Of Cliente),
        remedidaService As IDataService(Of Remedida),
        proyectoService As IDataService(Of Proyecto),
        colaboradorService As IDataService(Of Colaborador),
        contratistaService As IDataService(Of Contratista),
        tramiteService As IDataService(Of Tramite),
        contratoService As IDataService(Of Contrato))

        _clienteService = clienteService
        _remedidaService = remedidaService
        _proyectoService = proyectoService
        _colaboradorService = colaboradorService
        _contratistaService = contratistaService
        _tramiteService = tramiteService
        _contratoService = contratoService

        FechaHoy = Date.Today.ToString("dddd, dd 'de' MMMM 'de' yyyy",
                                       New Globalization.CultureInfo("es-HN"))
        CargarEstadisticas()
    End Sub

    Private Shared ReadOnly ChartColors As (Bar As String, Bg As String, Text As String)() = {
        ("#3B82F6", "#EFF6FF", "#1D4ED8"),
        ("#10B981", "#ECFDF5", "#065F46"),
        ("#F59E0B", "#FFFBEB", "#92400E"),
        ("#EF4444", "#FEF2F2", "#991B1B"),
        ("#8B5CF6", "#F5F3FF", "#5B21B6"),
        ("#06B6D4", "#ECFEFF", "#155E75")
    }

    Public Async Sub CargarEstadisticas()
        Try
            Dim clientes = Await _clienteService.GetAll()
            TotalClientes = clientes.Count()

            Dim remedidas = (Await _remedidaService.GetAll()).ToList()
            TotalRemedidas = remedidas.Count
            RemedidasEntregadas = remedidas.Where(Function(r) r.ExpedienteEntregado).Count()
            RemedidasPendientes = TotalRemedidas - RemedidasEntregadas

            Const barW As Double = 260.0
            If TotalRemedidas > 0 Then
                Dim pct = CDbl(RemedidasEntregadas) / TotalRemedidas
                BarraEntregadas = Math.Round(barW * pct)
                BarraPendientes = barW - BarraEntregadas
                PorcentajeEntregadas = $"{Math.Round(pct * 100, 0)}%"
            Else
                BarraEntregadas = 0
                BarraPendientes = barW
                PorcentajeEntregadas = "0%"
            End If

            Dim proyectos = Await _proyectoService.GetAll()
            TotalProyectos = proyectos.Count()

            Dim colaboradores = Await _colaboradorService.GetAll()
            TotalColaboradores = colaboradores.Count()

            Dim contratistas = Await _contratistaService.GetAll()
            TotalContratistas = contratistas.Count()

            Dim tramites = (Await _tramiteService.GetAll()).ToList()
            TotalTramites = tramites.Count

            Dim porEstado = tramites _
                .GroupBy(Function(t) If(t.EstadoTramite?.Estado, "Sin estado")) _
                .Select(Function(g) New With {.Estado = g.Key, .Total = g.Count()}) _
                .OrderByDescending(Function(x) x.Total) _
                .ToList()

            Dim maxCount = If(porEstado.Any(), CDbl(porEstado.Max(Function(x) x.Total)), 1.0)
            Dim items As New ObservableCollection(Of DashboardBarItem)
            For i = 0 To porEstado.Count - 1
                Dim c = ChartColors(i Mod ChartColors.Length)
                Dim barPx = Math.Round(240.0 * porEstado(i).Total / maxCount)
                items.Add(New DashboardBarItem With {
                    .Label = porEstado(i).Estado,
                    .Count = porEstado(i).Total,
                    .BarraPixels = barPx,
                    .BarBrush = New SolidColorBrush(CType(ColorConverter.ConvertFromString(c.Bar), Color)),
                    .BgBrush = New SolidColorBrush(CType(ColorConverter.ConvertFromString(c.Bg), Color)),
                    .TextBrush = New SolidColorBrush(CType(ColorConverter.ConvertFromString(c.Text), Color))
                })
            Next
            TramitesPorEstado = items

            Dim contratos = (Await _contratoService.GetAll()).ToList()
            TotalContratos = contratos.Count

            ConstruirActividadReciente(remedidas, tramites)

        Catch ex As Exception
            ' Dashboard is non-critical, silently ignore
        End Try
    End Sub

    ' Construye un feed cronológico con las últimas remedidas y trámites registrados.
    ' Solo estas entidades tienen campo Fecha; proyectos y contratos no lo tienen.
    Private Sub ConstruirActividadReciente(remedidas As List(Of Remedida), tramites As List(Of Tramite))
        Dim eventos As New List(Of (Fecha As Date, Item As ActividadItem))

        For Each r In remedidas
            Dim clave = If(String.IsNullOrWhiteSpace(r.ClaveSure), "#" & r.Id.ToString(), r.ClaveSure)
            eventos.Add((r.Fecha, New ActividadItem With {
                .Icono = "Ruler",
                .Titulo = "Remedida " & clave,
                .Subtitulo = If(r.ExpedienteEntregado, "Expediente entregado", "Expediente pendiente"),
                .FechaTexto = r.Fecha.ToString("dd/MM/yyyy"),
                .IconBrush = MakeBrush("#8B5CF6"),
                .IconBgBrush = MakeBrush("#F5F3FF")
            }))
        Next

        For Each t In tramites
            Dim fecha = t.Fecha.GetValueOrDefault()
            Dim titulo = If(String.IsNullOrWhiteSpace(t.Descripcion),
                            "Trámite #" & t.Id.ToString(), t.Descripcion)
            eventos.Add((fecha, New ActividadItem With {
                .Icono = "FolderOpen",
                .Titulo = titulo,
                .Subtitulo = "Estado: " & If(t.EstadoTramite?.Estado, "Sin estado"),
                .FechaTexto = If(t.Fecha.HasValue, fecha.ToString("dd/MM/yyyy"), "Sin fecha"),
                .IconBrush = MakeBrush("#F59E0B"),
                .IconBgBrush = MakeBrush("#FFFBEB")
            }))
        Next

        Dim recientes = eventos _
            .OrderByDescending(Function(e) e.Fecha) _
            .Take(6) _
            .Select(Function(e) e.Item) _
            .ToList()

        ActividadReciente = New ObservableCollection(Of ActividadItem)(recientes)
        SinActividad = recientes.Count = 0
    End Sub

    Private Shared Function MakeBrush(hex As String) As SolidColorBrush
        Return New SolidColorBrush(CType(ColorConverter.ConvertFromString(hex), Color))
    End Function
End Class
