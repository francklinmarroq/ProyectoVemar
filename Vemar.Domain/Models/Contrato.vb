Public Class Contrato : Inherits DomainObject
    Private _contratista As Contratista
    Private _valor As Decimal
    Private _proyecto As Proyecto

    Public Property Contratista As Contratista
        Get
            Return _contratista
        End Get
        Set(value As Contratista)
            _contratista = value
        End Set
    End Property

    Public Property Valor As Decimal
        Get
            Return _valor
        End Get
        Set(value As Decimal)
            _valor = value
        End Set
    End Property

    Public Property Proyecto As Proyecto
        Get
            Return _proyecto
        End Get
        Set(value As Proyecto)
            _proyecto = value
        End Set
    End Property

End Class
