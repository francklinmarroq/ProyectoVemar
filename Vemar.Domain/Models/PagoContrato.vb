Public Class PagoContrato : Inherits DomainObject
    Private _contrato As Contrato
    Private _valor As Decimal
    Private _descripcion As String
    Private _formaPago As String
    Private _fecha As DateTime = DateTime.Today

    Public Property Contrato As Contrato
        Get
            Return _contrato
        End Get
        Set(value As Contrato)
            _contrato = value
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

    Public Property Fecha As DateTime
        Get
            Return _fecha
        End Get
        Set(value As DateTime)
            _fecha = value
        End Set
    End Property
End Class
