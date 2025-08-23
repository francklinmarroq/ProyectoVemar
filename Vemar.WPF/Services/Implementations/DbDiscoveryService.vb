Imports System.Data
Imports Microsoft.Data.Sql
Imports Microsoft.Data.SqlClient

Public Class DbDiscoveryService : Implements IDbDiscoveryService

    Public Async Function GetSqlServerAsync() As Task(Of IEnumerable(Of String)) Implements IDbDiscoveryService.GetSqlServerAsync
        Return Await Task.Run(Function()
                                  Dim serverNames As New List(Of String)
                                  Try
                                      Dim servers As DataTable = SqlDataSourceEnumerator.Instance.GetDataSources()
                                      For Each row As DataRow In servers.Rows
                                          serverNames.Add(row("ServerName").ToString())
                                      Next
                                  Catch ex As Exception
                                      'implementar un servicio para manejar las excepciones  
                                  End Try
                                  Return serverNames
                              End Function)
    End Function

    Public Async Function GetDatabaseNamesAsync(serverName As String) As Task(Of IEnumerable(Of String)) Implements IDbDiscoveryService.GetDatabaseNamesAsync
        Return Await Task.Run(Function()
                                  Dim dbNames As New List(Of String)
                                  Dim connectionString As String = $"Server={serverName};Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=True;"
                                  Using connection As New SqlConnection(connectionString)
                                      Try
                                          connection.Open()
                                          Dim databases As DataTable = connection.GetSchema("Databases")
                                          For Each dbRow As DataRow In databases.Rows
                                              Dim dbName As String = dbRow("database_name").ToString()
                                              If dbName <> "master" AndAlso dbName <> "tempdb" AndAlso dbName <> "model" AndAlso dbName <> "msdb" Then
                                                  dbNames.Add(dbName)
                                              End If
                                          Next
                                      Catch ex As Exception
                                          'Implementar servicio para manejo de errores
                                      End Try
                                  End Using
                                  Return dbNames
                              End Function)
    End Function
End Class
