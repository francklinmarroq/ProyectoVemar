Public Class Remedida : Inherits DomainObject
    Private _representante As String
    Private _ubicacion As String
    Private _claveSure As String
    Private _cam As String
    Private _objeto As String
    Private _fecha As Date
    Private _costo As Decimal
    Private _colaborador As Colaborador

    Public Property Representante As String
        Get
            Return _representante
        End Get
        Set(value As String)
            _representante = value
        End Set
    End Property

    Public Property Ubicacion As String
        Get
            Return _ubicacion
        End Get
        Set(value As String)
            _ubicacion = value
        End Set
    End Property

    Public Property ClaveSure As String
        Get
            Return _claveSure
        End Get
        Set(value As String)
            _claveSure = value
        End Set
    End Property

    Public Property Cam As String
        Get
            Return _cam
        End Get
        Set(value As String)
            _cam = value
        End Set
    End Property

    Public Property Objeto As String
        Get
            Return _objeto
        End Get
        Set(value As String)
            _objeto = value
        End Set
    End Property

    Public Property Fecha As Date
        Get
            Return _fecha
        End Get
        Set(value As Date)
            _fecha = value
        End Set
    End Property

    Public Property Costo As Decimal
        Get
            Return _costo
        End Get
        Set(value As Decimal)
            _costo = value
        End Set
    End Property

    Public Property Colaborador As Colaborador
        Get
            Return _colaborador
        End Get
        Set(value As Colaborador)
            _colaborador = value
        End Set
    End Property

End Class
