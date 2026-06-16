Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Net.Sockets
Imports System.Threading.Tasks
Imports Microsoft.EntityFrameworkCore
Imports Vemar.EF

Public Class SplashViewModel
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event ReadyToLaunch As EventHandler

    Private Sub OnProp(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

    ' ── Campos de conexión ────────────────────────────────────────────────

    Private _server As String = ""
    Public Property Server As String
        Get
            Return _server
        End Get
        Set(value As String)
            _server = value
            OnProp(NameOf(Server))
            _connectCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Private _database As String = "vemar"
    Public Property Database As String
        Get
            Return _database
        End Get
        Set(value As String)
            _database = value
            OnProp(NameOf(Database))
            _connectCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Private _useWindowsAuth As Boolean = True
    Public Property UseWindowsAuth As Boolean
        Get
            Return _useWindowsAuth
        End Get
        Set(value As Boolean)
            _useWindowsAuth = value
            OnProp(NameOf(UseWindowsAuth))
            OnProp(NameOf(ShowSqlAuth))
        End Set
    End Property

    Public ReadOnly Property ShowSqlAuth As Boolean
        Get
            Return Not _useWindowsAuth
        End Get
    End Property

    Private _username As String = ""
    Public Property Username As String
        Get
            Return _username
        End Get
        Set(value As String)
            _username = value
            OnProp(NameOf(Username))
        End Set
    End Property

    Private _password As String = ""
    Public Property Password As String
        Get
            Return _password
        End Get
        Set(value As String)
            _password = value
            OnProp(NameOf(Password))
        End Set
    End Property

    ' ── Estado ────────────────────────────────────────────────────────────

    Private _statusMessage As String = "Ingrese los datos de conexión para continuar."
    Public Property StatusMessage As String
        Get
            Return _statusMessage
        End Get
        Set(value As String)
            _statusMessage = value
            OnProp(NameOf(StatusMessage))
        End Set
    End Property

    Private _statusType As String = "Info"
    Public Property StatusType As String
        Get
            Return _statusType
        End Get
        Set(value As String)
            _statusType = value
            OnProp(NameOf(StatusType))
        End Set
    End Property

    Private _isBusy As Boolean = False
    Public Property IsBusy As Boolean
        Get
            Return _isBusy
        End Get
        Set(value As Boolean)
            _isBusy = value
            OnProp(NameOf(IsBusy))
            OnProp(NameOf(IsNotBusy))
            _scanCommand?.RaiseCanExecuteChanged()
            _connectCommand?.RaiseCanExecuteChanged()
            _createDbCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public ReadOnly Property IsNotBusy As Boolean
        Get
            Return Not _isBusy
        End Get
    End Property

    Private _showCreateDb As Boolean = False
    Public Property ShowCreateDb As Boolean
        Get
            Return _showCreateDb
        End Get
        Set(value As Boolean)
            _showCreateDb = value
            OnProp(NameOf(ShowCreateDb))
        End Set
    End Property

    Private _showServerList As Boolean = False
    Public Property ShowServerList As Boolean
        Get
            Return _showServerList
        End Get
        Set(value As Boolean)
            _showServerList = value
            OnProp(NameOf(ShowServerList))
        End Set
    End Property

    ' ── Servidores encontrados ────────────────────────────────────────────

    Public ReadOnly Property FoundServers As New ObservableCollection(Of String)()

    ' ── Comandos ─────────────────────────────────────────────────────────

    Private _scanCommand As RelayCommand
    Private _connectCommand As RelayCommand
    Private _createDbCommand As RelayCommand

    Public ReadOnly Property ScanCommand As ICommand
        Get
            Return _scanCommand
        End Get
    End Property

    Public ReadOnly Property ConnectCommand As ICommand
        Get
            Return _connectCommand
        End Get
    End Property

    Public ReadOnly Property CreateDatabaseCommand As ICommand
        Get
            Return _createDbCommand
        End Get
    End Property

    Public ReadOnly Property SelectServerCommand As ICommand

    ' ── Constructor ───────────────────────────────────────────────────────

    Public Sub New()
        _scanCommand = New RelayCommand(
            Async Sub(o) Await ScanNetworkAsync(),
            Function(o) Not IsBusy)

        _connectCommand = New RelayCommand(
            Async Sub(o) Await ConnectAsync(),
            Function(o) Not IsBusy AndAlso
                        Not String.IsNullOrWhiteSpace(Server) AndAlso
                        Not String.IsNullOrWhiteSpace(Database))

        _createDbCommand = New RelayCommand(
            Async Sub(o) Await CreateDatabaseAsync(),
            Function(o) Not IsBusy)

        SelectServerCommand = New RelayCommand(
            Sub(o)
                If o IsNot Nothing Then
                    Server = o.ToString()
                    ShowServerList = False
                End If
            End Sub)

        ' Pre-cargar ajustes guardados
        If ConnectionSettings.HasSavedSettings() Then
            Dim saved = ConnectionSettings.Current
            Server = saved.Server
            Database = saved.Database
            UseWindowsAuth = saved.UseWindowsAuth
            Username = saved.Username
            Password = saved.Password
            StatusMessage = "Ajustes previos cargados. Pulse Conectar para continuar."
            StatusType = "Info"
        End If
    End Sub

    ' ── Escaneo de red ────────────────────────────────────────────────────
    ' Prueba el puerto TCP 1433 en hosts candidatos de la subred local.

    Private Async Function ScanNetworkAsync() As Task
        IsBusy = True
        StatusMessage = "Buscando instancias de SQL Server en la red…"
        StatusType = "Info"
        FoundServers.Clear()
        ShowServerList = False

        Try
            ' Construir lista de candidatos
            Dim candidates As New List(Of String) From {
                "localhost",
                ".\SQLEXPRESS",
                "localhost\SQLEXPRESS",
                Environment.MachineName,
                Environment.MachineName & "\SQLEXPRESS",
                Environment.MachineName & "\MSSQLSERVER"
            }

            ' Agregar hosts de la subred local
            Try
                Dim entry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName())
                For Each addr In entry.AddressList
                    If addr.AddressFamily = Net.Sockets.AddressFamily.InterNetwork Then
                        Dim parts = addr.ToString().Split("."c)
                        If parts.Length = 4 Then
                            Dim prefix = $"{parts(0)}.{parts(1)}.{parts(2)}."
                            For i = 1 To 30
                                candidates.Add(prefix & i.ToString())
                            Next
                        End If
                    End If
                Next
            Catch
            End Try

            ' Probar cada candidato en paralelo
            Dim checkTasks = candidates.Distinct().ToList() _
                             .Select(Function(h) CheckSqlPortAsync(h)).ToList()

            Await Task.WhenAll(checkTasks)

            For Each tsk In checkTasks
                If tsk.Result IsNot Nothing Then
                    FoundServers.Add(tsk.Result)
                End If
            Next

            If FoundServers.Count > 0 Then
                ShowServerList = True
                StatusMessage = $"Se encontraron {FoundServers.Count} instancia(s). Seleccione una o escríbala manualmente."
                StatusType = "Success"
            Else
                StatusMessage = "No se detectaron instancias. Escriba el servidor manualmente (ej: SERVIDOR\SQLEXPRESS)."
                StatusType = "Warning"
                For Each cand In {"localhost", ".\SQLEXPRESS", Environment.MachineName & "\SQLEXPRESS"}
                    FoundServers.Add(cand)
                Next
                ShowServerList = True
            End If

        Catch ex As Exception
            StatusMessage = "Error al escanear: " & ex.Message
            StatusType = "Error"
        Finally
            IsBusy = False
        End Try
    End Function

    ''' <summary>Intenta conectar al puerto 1433 con timeout de 400ms.</summary>
    Private Async Function CheckSqlPortAsync(host As String) As Task(Of String)
        Try
            Dim cleanHost = host.Split("\"c)(0)
            Using tcp As New Net.Sockets.TcpClient()
                Dim connectTask = tcp.ConnectAsync(cleanHost, 1433)
                Dim winner = Await Task.WhenAny(connectTask, Task.Delay(400))
                If winner Is connectTask AndAlso Not connectTask.IsFaulted Then
                    Return host
                End If
            End Using
        Catch
        End Try
        Return Nothing
    End Function

    ' ── Conectar ─────────────────────────────────────────────────────────

    Private Async Function ConnectAsync() As Task
        IsBusy = True
        ShowCreateDb = False
        StatusMessage = "Probando conexión…"
        StatusType = "Info"

        Try
            Dim settings As New ConnectionSettings With {
                .Server = Me.Server,
                .Database = Me.Database,
                .UseWindowsAuth = Me.UseWindowsAuth,
                .Username = Me.Username,
                .Password = Me.Password
            }

            ' 1. Servidor accesible
            Dim serverOk = Await ConnectionSettings.TestServerAsync(settings.ConnectionString)
            If Not serverOk Then
                StatusMessage = $"No se puede conectar al servidor '{Server}'. Verifique el nombre y que SQL Server esté activo."
                StatusType = "Error"
                IsBusy = False
                Return
            End If

            ' 2. Base de datos existe
            Dim dbOk = Await ConnectionSettings.DatabaseExistsAsync(settings.ConnectionString)
            If Not dbOk Then
                StatusMessage = $"El servidor responde pero la base de datos '{Database}' no existe."
                StatusType = "Warning"
                ShowCreateDb = True
                IsBusy = False
                Return
            End If

            ' 3. Esquema válido
            Dim schemaOk = Await VerifySchemaAsync(settings.ConnectionString)
            If Not schemaOk Then
                StatusMessage = "La BD existe pero no tiene el esquema de Vemar. Pulse 'Crear base de datos' para inicializarlo."
                StatusType = "Warning"
                ShowCreateDb = True
                IsBusy = False
                Return
            End If

            ' 4. Listo
            ConnectionSettings.SetCurrent(settings)
            settings.Save()
            StatusMessage = "Conexión exitosa. Iniciando sistema…"
            StatusType = "Success"
            Await Task.Delay(600)
            RaiseEvent ReadyToLaunch(Me, EventArgs.Empty)

        Catch ex As Exception
            StatusMessage = "Error: " & ex.Message
            StatusType = "Error"
            IsBusy = False
        End Try
    End Function

    ' ── Crear base de datos ───────────────────────────────────────────────

    Private Async Function CreateDatabaseAsync() As Task
        IsBusy = True
        StatusMessage = "Creando base de datos y aplicando esquema…"
        StatusType = "Info"

        Try
            Dim settings As New ConnectionSettings With {
                .Server = Me.Server,
                .Database = Me.Database,
                .UseWindowsAuth = Me.UseWindowsAuth,
                .Username = Me.Username,
                .Password = Me.Password
            }

            Await Task.Run(Async Function()
                               ConnectionSettings.SetCurrent(settings)
                               Dim factory As New VemarDbContextFactory()
                               Using ctx = factory.CreateDbContext()
                                   Await ctx.Database.MigrateAsync()
                               End Using
                           End Function)

            settings.Save()
            ShowCreateDb = False
            StatusMessage = "Base de datos creada correctamente. Iniciando sistema…"
            StatusType = "Success"
            Await Task.Delay(800)
            RaiseEvent ReadyToLaunch(Me, EventArgs.Empty)

        Catch ex As Exception
            StatusMessage = "Error al crear la base de datos: " & ex.Message
            StatusType = "Error"
            IsBusy = False
        End Try
    End Function

    ' ── Verificar esquema ─────────────────────────────────────────────────

    Private Async Function VerifySchemaAsync(connStr As String) As Task(Of Boolean)
        Try
            Using conn As New Microsoft.Data.SqlClient.SqlConnection(connStr)
                Await conn.OpenAsync()
                Using cmd = conn.CreateCommand()
                    cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " &
                                      "WHERE TABLE_NAME IN ('Clientes','Proyectos','Remedidas','Contratos')"
                    Dim count = CInt(Await cmd.ExecuteScalarAsync())
                    Return count >= 4
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function

End Class
