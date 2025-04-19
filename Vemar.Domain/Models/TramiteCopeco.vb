Public Class TramiteCopeco
    Private _proyecto As Proyecto
    Private _estado As String

    Public Property Proyecto As Proyecto
        Get
            Return _proyecto
        End Get
        Set(value As Proyecto)
            _proyecto = value
        End Set
    End Property

    Public Property Estado As String
        Get
            Return _estado
        End Get
        Set(value As String)
            _estado = value
        End Set
    End Property

End Class
