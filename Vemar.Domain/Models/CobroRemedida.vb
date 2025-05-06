Public Class CobroRemedida : Inherits DomainObject
    Private _remedida As Remedida
    Private _cantidad As Decimal

    Public Property Remedida As Remedida
        Get
            Return _remedida
        End Get
        Set(value As Remedida)
            _remedida = value
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

End Class
