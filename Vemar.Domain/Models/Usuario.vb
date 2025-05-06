Public Class Usuario : Inherits DomainObject
    Public Property _usuario As String
    Private _hashContrasena As String
    Private _tipoUsuario As TipoUsuario


    Public Property Usuario As String
        Get
            Return _usuario
        End Get
        Set(value As String)
            _usuario = value
        End Set
    End Property

    Public Property HashContrasena As String
        Get
            Return _hashContrasena
        End Get
        Set(value As String)
            _hashContrasena = value
        End Set
    End Property

    Public Property TipoUsuario As TipoUsuario
        Get
            Return _tipoUsuario
        End Get
        Set(value As TipoUsuario)
            _tipoUsuario = value
        End Set
    End Property

End Class
