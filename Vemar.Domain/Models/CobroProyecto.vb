Public Class CobroProyecto : Inherits DomainObject
    Private _proyecto As Proyecto
    Private _fecha As DateTime
    Private _monto As Decimal
    Private _descripcion As String
    Private _formaPago As String
    Private _esEfectuado As Boolean

    Public Property Proyecto As Proyecto
        Get
            Return _proyecto
        End Get
        Set(value As Proyecto)
            _proyecto = value
        End Set
    End Property

    Public Property Fecha As DateTime
        Get
            Return _fecha
        End Get
        Set(value As DateTime)
            _fecha = value
        End Set
    End Property

    Public Property Monto As Decimal
        Get
            Return _monto
        End Get
        Set(value As Decimal)
            _monto = value
        End Set
    End Property

    Public Property Descripcion As String
        Get
            Return _descripcion
        End Get
        Set(value As String)
            _descripcion = value
        End Set
    End Property

    Public Property FormaPago As String
        Get
            Return _formaPago
        End Get
        Set(value As String)
            _formaPago = value
        End Set
    End Property

    Public Property EsEfectuado As Boolean
        Get
            Return _esEfectuado
        End Get
        Set(value As Boolean)
            _esEfectuado = value
        End Set
    End Property

End Class
