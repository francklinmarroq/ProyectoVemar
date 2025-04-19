Public Class Asignacion
    Private _colaborador As Colaborador
    Private _proyecto As Proyecto
    Private _fechaAsignacion As New DateTime
    Private _fechaFinalizacion As New DateTime
    Private _observaciones As String
    Private _cliente As Cliente

    Public Property Colaborador As Colaborador
        Get
            Return _colaborador
        End Get
        Set(value As Colaborador)
            _colaborador = value
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

    Public Property FechaAsignacion As DateTime
        Get
            Return _fechaAsignacion
        End Get
        Set(value As DateTime)
            _fechaAsignacion = value
        End Set
    End Property

    Public Property FechaFinalizacion As DateTime
        Get
            Return _fechaFinalizacion
        End Get
        Set(value As DateTime)
            _fechaFinalizacion = value
        End Set
    End Property

    Public Property Observaciones As String
        Get
            Return _observaciones
        End Get
        Set(value As String)
            _observaciones = value
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

End Class
