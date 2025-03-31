Public Class Colaborador : Inherits DomainObject
    Private _dni As String
    Private _nombre As String

    Public Property Dni As String
        Get
            Return _dni
        End Get
        Set(value As String)
            _dni = value
        End Set
    End Property

    Public Property Nombre As String
        Get
            Return _nombre
        End Get
        Set(value As String)
            _nombre = value
        End Set
    End Property
End Class
