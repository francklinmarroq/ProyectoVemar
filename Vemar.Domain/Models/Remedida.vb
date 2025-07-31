Public Class Remedida : Inherits DomainObject
    Private _cliente As Cliente
    Private _representante As String
    Private _ubicacion As String
    Private _claveSure As String
    Private _matricula As String
    Private _cam As String
    Private _objeto As String
    Private _fecha As Date
    Private _precio As Decimal
    Private _expedienteEntregado As Boolean

    Public Property Representante As String
        Get
            Return _representante
        End Get
        Set(value As String)
            _representante = value
        End Set
    End Property

    Public Property Cliente As Cliente
        Get
            Return _cliente
        End Get
        Set(value As Cliente)
            _cliente = value
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

    Public Property Matricula As String
        Get
            Return _matricula
        End Get
        Set(value As String)
            _matricula = value
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

    Public Property Precio As Decimal
        Get
            Return _precio
        End Get
        Set(value As Decimal)
            _precio = value
        End Set
    End Property

    Public Property ExpedienteEntregado As Boolean
        Get
            Return _expedienteEntregado
        End Get
        Set(value As Boolean)
            _expedienteEntregado = value
        End Set
    End Property

End Class
