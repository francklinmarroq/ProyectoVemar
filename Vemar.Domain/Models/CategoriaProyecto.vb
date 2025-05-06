Public Class CategoriaProyecto : Inherits DomainObject
    Private _nombre As String
    Private _cantidadLotes As Integer

    Public Property Nombre As String
        Get
            Return _nombre
        End Get
        Set(value As String)
            _nombre = value
        End Set
    End Property

    Public Property CantidadLotes As Integer
        Get
            Return _cantidadLotes
        End Get
        Set(value As Integer)
            _cantidadLotes = value
        End Set

    End Property

End Class
