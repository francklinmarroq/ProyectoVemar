Public Class TipoProyecto : Inherits DomainObject
    Private _tipoProryecto As String

    Public Property TipoProyecto As String
        Get
            Return _tipoProryecto
        End Get
        Set(value As String)
            _tipoProryecto = value
        End Set
    End Property

End Class
