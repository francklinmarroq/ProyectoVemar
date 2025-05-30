Public Class Cliente : Inherits DomainObject
    Private _rtn As String
    Private _nombre As String
    Private _direccion As String
    Private _representante As String
    Private _dniRepresentante As String
    Private _rtnRepresentante As String
    Private _telefono As String
    Private _emailRepresentante As String
    Private _emailCorporativo As String

    Public Property Rtn As String
        Get
            Return _rtn
        End Get
        Set(value As String)
            _rtn = value
        End Set
    End Property

    Public Property Nombre As String
        Get
            Return _nombre
        End Get
        Set(value As String)
            _nombre = value
        End Set
    End Property

    Public Property Direccion As String
        Get
            Return _direccion
        End Get
        Set(value As String)
            _direccion = value
        End Set
    End Property

    Public Property Representante As String
        Get
            Return _representante
        End Get
        Set(value As String)
            _representante = value
        End Set
    End Property

    Public Property DniRepresentante As String
        Get
            Return _dniRepresentante
        End Get
        Set(value As String)
            _dniRepresentante = value
        End Set
    End Property

    Public Property RtnRepresentante As String
        Get
            Return _rtnRepresentante
        End Get
        Set(value As String)
            _rtnRepresentante = value
        End Set
    End Property

    Public Property Telefono As String
        Get
            Return _telefono
        End Get
        Set(value As String)
            _telefono = value
        End Set
    End Property

    Public Property EmailRepresentante As String
        Get
            Return _emailRepresentante
        End Get
        Set(value As String)
            _emailRepresentante = value
        End Set
    End Property

    Public Property EmailCorporativo As String
        Get
            Return _emailCorporativo
        End Get
        Set(value As String)
            _emailCorporativo = value
        End Set
    End Property

End Class
