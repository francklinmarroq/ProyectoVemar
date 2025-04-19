Public Class DomainObject
    Private _id As Integer

    Public Sub New()
    End Sub

    Public Sub New(id As Integer)
        Me._id = id
    End Sub

    Public Property Id As Integer
        Get
            Return _id
        End Get
        Set(value As Integer)
            _id = value
        End Set
    End Property

End Class
