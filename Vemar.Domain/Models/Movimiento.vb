Public Class Movimiento
    Private _remedida As Remedida
    Private _tipoMovimiento As TipoMovimiento
    Private _fecha As Date

    Public Property Remedida As Remedida
        Get
            Return _remedida
        End Get
        Set(value As Remedida)
            _remedida = value
        End Set
    End Property

    Public Property TipoMovimiento As TipoMovimiento
        Get
            Return _tipoMovimiento
        End Get
        Set(value As TipoMovimiento)
            _tipoMovimiento = value
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

End Class
