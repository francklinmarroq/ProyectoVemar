Imports System.Runtime.InteropServices.Marshalling

Public Class Proyecto : Inherits DomainObject
    Private _nombre As String
    Private _cliente As Cliente
    Private _ubicacion As String
    Private _zonificacion As Zonificacion
    Private _area As Decimal
    Private _categoriaProyecto As CategoriaProyecto
    Private _matricula As String
    Private _claveSure As String

    Public Property Nombre As String
        Get
            Return _nombre
        End Get
        Set(value As String)
            _nombre = value
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


    Public Property Ubicacion As String
        Get
            Return _ubicacion
        End Get
        Set(value As String)
            _ubicacion = value
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

    Public Property CategoriaProyecto As CategoriaProyecto
        Get
            Return _categoriaProyecto
        End Get
        Set(value As CategoriaProyecto)
            _categoriaProyecto = value
        End Set
    End Property

    Public Property Matricula As String
        Get
            Return _matricula
        End Get
        Set(value As String)
            _matricula = value
        End Set
    End Property

    Public Property ClaveSure As String
        Get
            Return _claveSure
        End Get
        Set(value As String)
            _claveSure = value
        End Set
    End Property

End Class
