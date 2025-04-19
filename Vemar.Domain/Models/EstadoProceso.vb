Public Class EstadoProceso : Inherits DomainObject
    Private _estado As String

    Public Property Estado As String
        Get
            Return _estado
        End Get
        Set(value As String)
            _estado = value
        End Set
    End Property


End Class
