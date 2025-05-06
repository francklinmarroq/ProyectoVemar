Public Class Tramite : Inherits DomainObject
    Private _tipoTramite As TipoTramite
    Private _proyecto As Proyecto
    Private _estadoTramite As EstadoTramite
    Private _descripcion As String

    Public Property TipoTramite As TipoTramite
        Get
            Return _tipoTramite
        End Get
        Set(value As TipoTramite)
            _tipoTramite = value
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

    Public Property EstadoTramite As EstadoTramite
        Get
            Return _estadoTramite
        End Get
        Set(value As EstadoTramite)
            _estadoTramite = value
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

End Class
