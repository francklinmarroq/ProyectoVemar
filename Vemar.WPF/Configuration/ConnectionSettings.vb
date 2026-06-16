Imports System.IO
Imports System.Text.Json
Imports Vemar.EF

Public Class ConnectionSettings

    Public Property Server As String = ""
    Public Property Database As String = "vemar"
    Public Property UseWindowsAuth As Boolean = True
    Public Property Username As String = ""
    Public Property Password As String = ""

    Public ReadOnly Property ConnectionString As String
        Get
            Dim base = $"Server={Server};Database={Database};TrustServerCertificate=True;"
            If UseWindowsAuth Then
                Return base & "Trusted_Connection=True;"
            Else
                Return base & $"User Id={Username};Password={Password};"
            End If
        End Get
    End Property

    ' ── Ruta de persistencia ──────────────────────────────────────────────
    Private Shared ReadOnly SettingsFilePath As String = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Vemar", "connection.json")

    Private Shared _current As ConnectionSettings = Nothing

    Public Shared ReadOnly Property Current As ConnectionSettings
        Get
            If _current Is Nothing Then
                _current = LoadFromDisk()
            End If
            Return _current
        End Get
    End Property

    Public Shared Sub SetCurrent(settings As ConnectionSettings)
        _current = settings
        ConnectionConfig.ConnectionString = settings.ConnectionString
    End Sub

    Public Sub Save()
        Dim dir = Path.GetDirectoryName(SettingsFilePath)
        If Not Directory.Exists(dir) Then
            Directory.CreateDirectory(dir)
        End If
        Dim toSave As New ConnectionSettings With {
            .Server = Me.Server,
            .Database = Me.Database,
            .UseWindowsAuth = Me.UseWindowsAuth,
            .Username = Me.Username,
            .Password = If(Me.UseWindowsAuth, "", Me.Password)
        }
        File.WriteAllText(SettingsFilePath, JsonSerializer.Serialize(toSave))
    End Sub

    Public Shared Function HasSavedSettings() As Boolean
        Return File.Exists(SettingsFilePath) AndAlso
               Not String.IsNullOrWhiteSpace(Current.Server)
    End Function

    Public Shared Function LoadFromDisk() As ConnectionSettings
        Try
            If File.Exists(SettingsFilePath) Then
                Dim json = File.ReadAllText(SettingsFilePath)
                Dim s = JsonSerializer.Deserialize(Of ConnectionSettings)(json)
                If s IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(s.Server) Then
                    Return s
                End If
            End If
        Catch
        End Try
        Return New ConnectionSettings()
    End Function

    ''' <summary>Prueba conectar al servidor (usando master para no requerir la BD).</summary>
    Public Shared Async Function TestServerAsync(connStr As String) As Task(Of Boolean)
        Try
            Dim builder As New Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connStr)
            builder.InitialCatalog = "master"
            builder.ConnectTimeout = 5
            Using conn As New Microsoft.Data.SqlClient.SqlConnection(builder.ConnectionString)
                Await conn.OpenAsync()
                Return True
            End Using
        Catch
            Return False
        End Try
    End Function

    ''' <summary>Verifica si la base de datos existe y es accesible.</summary>
    Public Shared Async Function DatabaseExistsAsync(connStr As String) As Task(Of Boolean)
        Try
            Dim builder As New Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connStr)
            builder.ConnectTimeout = 5
            Using conn As New Microsoft.Data.SqlClient.SqlConnection(builder.ConnectionString)
                Await conn.OpenAsync()
                Return True
            End Using
        Catch
            Return False
        End Try
    End Function

    ''' <summary>Reemplaza el nombre de la BD en el connection string.</summary>
    Public Shared Function SwapDatabase(connStr As String, newDb As String) As String
        Try
            Dim builder As New Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connStr)
            builder.InitialCatalog = newDb
            Return builder.ConnectionString
        Catch
            Return connStr
        End Try
    End Function

End Class
