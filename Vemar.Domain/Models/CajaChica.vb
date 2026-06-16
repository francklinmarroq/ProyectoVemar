Public Class CajaChica : Inherits DomainObject

    Private _fecha As Date
    Private _concepto As String
    Private _monto As Decimal
    Private _tipoOperacion As String   ' "Entrada" | "Salida"
    Private _remedida As Remedida
    Private _proyecto As Proyecto

    Public Property Fecha As Date
        Get
            Return _fecha
        End Get
        Set(value As Date)
            _fecha = value
        End Set
    End Property

    Public Property Concepto As String
        Get
            Return _concepto
        End Get
        Set(value As String)
            _concepto = value
        End Set
    End Property

    Public Property Monto As Decimal
        Get
            Return _monto
        End Get
        Set(value As Decimal)
            _monto = value
        End Set
    End Property

    ''' <summary>"Entrada" o "Salida"</summary>
    Public Property TipoOperacion As String
        Get
            Return _tipoOperacion
        End Get
        Set(value As String)
            _tipoOperacion = value
        End Set
    End Property

    ''' <summary>Remedida vinculada (opcional)</summary>
    Public Property Remedida As Remedida
        Get
            Return _remedida
        End Get
        Set(value As Remedida)
            _remedida = value
        End Set
    End Property

    ''' <summary>Proyecto vinculado (opcional)</summary>
    Public Property Proyecto As Proyecto
        Get
            Return _proyecto
        End Get
        Set(value As Proyecto)
            _proyecto = value
        End Set
    End Property

    ''' <summary>Texto para mostrar en UI: Remedida / Proyecto / —</summary>
    Public ReadOnly Property VinculoDisplay As String
        Get
            If _remedida IsNot Nothing Then
                Dim clave = If(String.IsNullOrWhiteSpace(_remedida.ClaveSure),
                               "#" & _remedida.Id.ToString(), _remedida.ClaveSure)
                Return "Remedida: " & clave
            ElseIf _proyecto IsNot Nothing Then
                Return "Proyecto: " & If(_proyecto.Nombre, "#" & _proyecto.Id.ToString())
            End If
            Return "—"
        End Get
    End Property

End Class
