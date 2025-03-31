Public Class Usuario
    Public Property _usuario As String
    Private _contrasena As String
    Private _tipoUsuario As TipoUsuario


    Public Property Usuario As String
        Get
            Return _usuario
        End Get
        Set(value As String)
            _usuario = value
        End Set
    End Property

    Public Property Contrasena As String
        Get
            Return _contrasena
        End Get
        Set(value As String)
            _contrasena = value
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
