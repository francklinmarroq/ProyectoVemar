Public Class Zonificacion : Inherits DomainObject
    Private _zonificacion As String

    Public Property Zonificacion As String
        Get
            Return _zonificacion
        End Get
        Set(value As String)
            _zonificacion = value
        End Set
    End Property

End Class
