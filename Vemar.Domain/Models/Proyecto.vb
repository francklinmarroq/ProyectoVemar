Public Class Proyecto : Inherits DomainObject
    Private _nombre As String
    Private _direccion As String
    Private _zonificacion As Zonificacion
    Private _area As Decimal
    Private _tipóProyecto As TipoProyecto
    Private _estadoProceso As EstadoProceso

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

    Public Property Zonificacion As Zonificacion
        Get
            Return _zonificacion
        End Get
        Set(value As Zonificacion)
            _zonificacion = value
        End Set
    End Property

    Public Property Area As Decimal
        Get
            Return _area
        End Get
        Set(value As Decimal)
            _area = value
        End Set
    End Property

    Public Property TipoProyecto As TipoProyecto
        Get
            Return _tipóProyecto
        End Get
        Set(value As TipoProyecto)
            _tipóProyecto = value
        End Set
    End Property

    Public Property EstadoProceso As EstadoProceso
        Get
            Return _estadoProceso
        End Get
        Set(value As EstadoProceso)
            _estadoProceso = value
        End Set
    End Property

End Class
