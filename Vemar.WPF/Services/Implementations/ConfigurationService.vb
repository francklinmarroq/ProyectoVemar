Imports System.IO
Imports System.Text.Json

Public Class ConfigurationService : Implements IConfigurationService
    Private Const _configFileName As String = "config.json"

    Public Sub SaveConfig(config As DatabaseConfig) Implements IConfigurationService.SaveConfig
        Dim jsonString = JsonSerializer.Serialize(config)
        File.WriteAllText(_configFileName, jsonString)
    End Sub

    Public Function LoadConfig() As DatabaseConfig Implements IConfigurationService.LoadConfig
        If File.Exists(_configFileName) Then
            Dim jsonString = File.ReadAllText(_configFileName)
            Return JsonSerializer.Deserialize(Of DatabaseConfig)(jsonString)
        End If
        Return Nothing
    End Function
End Class
