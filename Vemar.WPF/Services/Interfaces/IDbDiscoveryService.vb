Public Interface IDbDiscoveryService
    Function GetSqlServerAsync() As Task(Of IEnumerable(Of String))
    Function GetDatabaseNamesAsync(serverName As String) As Task(Of IEnumerable(Of String))

End Interface
