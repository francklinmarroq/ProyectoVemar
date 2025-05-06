Public Class GastoRemedida : Inherits DomainObject
    Private _remedida As Remedida
    Private _descripocion As String
    Private _cantidad As Decimal
    Private _costoUnitario As Decimal
    Private _pendienteDePago As Boolean

    Public Property Remedida As Remedida
        Get
            Return _remedida
        End Get
        Set(value As Remedida)
            _remedida = value
        End Set
    End Property

    Public Property Descripcion As String
        Get
            Return _descripocion
        End Get
        Set(value As String)
            _descripocion = value
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
