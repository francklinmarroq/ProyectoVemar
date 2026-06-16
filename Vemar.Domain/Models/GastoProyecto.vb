Public Class GastoProyecto : Inherits DomainObject
    Private _proyecto As Proyecto
    Private _descripcion As String
    Private _cantidad As Decimal
    Private _costoUnitario As Decimal
    Private _pendienteDePago As Boolean

    Public Property Proyecto As Proyecto
        Get
            Return _proyecto
        End Get
        Set(value As Proyecto)
            _proyecto = value
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

    Public Property Cantidad As Decimal
        Get
            Return _cantidad
        End Get
        Set(value As Decimal)
            _cantidad = value
        End Set
    End Property

    Public Property CostoUnitario As Decimal
        Get
            Return _costoUnitario
        End Get
        Set(value As Decimal)
            _costoUnitario = value
        End Set
    End Property

    Public Property PendienteDePago As Boolean
        Get
            Return _pendienteDePago
        End Get
        Set(value As Boolean)
            _pendienteDePago = value
        End Set
    End Property

End Class
