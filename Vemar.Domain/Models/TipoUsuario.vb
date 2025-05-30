Public Class TipoUsuario : Inherits DomainObject
    Private _tipo As String

    Public Property Tipo As String
        Get
            Return _tipo
        End Get
        Set(value As String)
            _tipo = value
        End Set
    End Property
End Class
