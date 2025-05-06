Public Class TipoTramite : Inherits DomainObject
    Private _nombre As String

    Public Property Nombre As String
        Get
            Return _nombre
        End Get
        Set(value As String)
            _nombre = value
        End Set
    End Property

End Class
