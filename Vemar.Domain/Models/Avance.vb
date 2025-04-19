Public Class Avance : Inherits DomainObject
    Private _proyecto As Proyecto
    Private _descripcion As String

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

End Class
