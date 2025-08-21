Imports System.ComponentModel
Imports Vemar.Domain
Imports Vemar.EF.Services

Public Class DetalleRemedidaViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private _clienteId As Integer
    Private _rntCliente As String
    Private _nombreCliente As String
    Private _representante As String
    Private _ubicacion As String
    Private _claveSure As String
    Private _matricula As String
    Private _cam As String
    Private _objeto As String
    Private _fecha As Date
    Private _precio As Decimal
    Private _expedienteEntregado As Boolean

    Public ReadOnly Property BuscarClienteCommand As ICommand

    Private _guardarRemedidaCommand As RelayCommand

    Private ReadOnly _clienteDataService As ClienteDataService
    Private ReadOnly _remedidaDataService As IDataService(Of Remedida)
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

#Region "Propiedades"
    Public Property ClienteId As Integer
        Get
            Return _clienteId
        End Get
        Set(value As Integer)
            If _clienteId <> value Then
                _clienteId = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ClienteId)))
                _guardarRemedidaCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property RtnCliente As String
        Get
            Return _rntCliente
        End Get
        Set(value As String)
            If _rntCliente <> value Then
                _rntCliente = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RtnCliente)))
                _guardarRemedidaCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property NombreCliente As String
        Get
            Return _nombreCliente
        End Get
        Set(value As String)
            If _nombreCliente <> value Then
                _nombreCliente = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(NombreCliente)))
                _guardarRemedidaCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property Representante As String
        Get
            Return _representante
        End Get
        Set(value As String)
            If _representante <> value Then
                _representante = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Representante)))
                _guardarRemedidaCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property Ubicacion As String
        Get
            Return _ubicacion
        End Get
        Set(value As String)
            If _ubicacion <> value Then
                _ubicacion = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Ubicacion)))
                _guardarRemedidaCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property ClaveSure As String
        Get
            Return _claveSure
        End Get
        Set(value As String)
            If _claveSure <> value Then
                _claveSure = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ClaveSure)))
                _guardarRemedidaCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property Matricula As String
        Get
            Return _matricula
        End Get
        Set(value As String)
            If _matricula <> value Then
                _matricula = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Matricula)))
                _guardarRemedidaCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property Cam As String
        Get
            Return _cam
        End Get
        Set(value As String)
            If _cam <> value Then
                _cam = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Cam)))
            End If
        End Set
    End Property
    Public Property Objeto As String
        Get
            Return _objeto
        End Get
        Set(value As String)
            If _objeto <> value Then
                _objeto = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Objeto)))
                _guardarRemedidaCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property Fecha As Date
        Get
            Return _fecha
        End Get
        Set(value As Date)
            If _fecha <> value Then
                _fecha = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Fecha)))
            End If
        End Set
    End Property
    Public Property Precio As Decimal
        Get
            Return _precio
        End Get
        Set(value As Decimal)
            If _precio <> value Then
                _precio = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Precio)))
                _guardarRemedidaCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property ExpedienteEntregado As Boolean
        Get
            Return _expedienteEntregado
        End Get
        Set(value As Boolean)
            If _expedienteEntregado <> value Then
                _expedienteEntregado = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ExpedienteEntregado)))
            End If
        End Set
    End Property

    Public ReadOnly Property GuardarRemedidaCommand As ICommand
        Get
            Return _guardarRemedidaCommand
        End Get
    End Property
#End Region

    Public Sub New(dataService As ClienteDataService, remedidaDataService As IDataService(Of Remedida))
        _clienteDataService = dataService
        _remedidaDataService = remedidaDataService

        ClienteId = Nothing
        RtnCliente = String.Empty
        NombreCliente = String.Empty
        Representante = String.Empty
        Ubicacion = String.Empty
        ClaveSure = String.Empty
        Matricula = String.Empty
        Cam = String.Empty
        Objeto = String.Empty
        Fecha = Date.Now
        Precio = 0D
        ExpedienteEntregado = False
        BuscarClienteCommand = New RelayCommand(AddressOf BuscarCliente)
        _guardarRemedidaCommand = New RelayCommand(AddressOf GuardarRemedida, AddressOf CanGuardarRemedida)
    End Sub

    Private Function CanGuardarRemedida(arg As Object) As Boolean
        'evalua que ningun campo este vacio antes de permitir guardar
        Return Not String.IsNullOrWhiteSpace(RtnCliente) AndAlso
               Not String.IsNullOrWhiteSpace(NombreCliente) AndAlso
               Not String.IsNullOrWhiteSpace(Representante) AndAlso
               Not String.IsNullOrWhiteSpace(Ubicacion) AndAlso
               Not String.IsNullOrWhiteSpace(ClaveSure) AndAlso
               Not String.IsNullOrWhiteSpace(Matricula) AndAlso
               Not String.IsNullOrWhiteSpace(Cam) AndAlso
               Not String.IsNullOrWhiteSpace(Objeto) AndAlso
               Fecha <> Date.MinValue
    End Function

    Private Async Sub GuardarRemedida()
        Try
            Dim nuevaRemedida As New Remedida With {
                .ClienteId = ClienteId,
                .Representante = Representante,
                .Ubicacion = Ubicacion,
                .ClaveSure = ClaveSure,
                .Matricula = Matricula,
                .Cam = Cam,
                .Objeto = Objeto,
                .Fecha = Fecha,
                .Precio = Precio,
                .ExpedienteEntregado = ExpedienteEntregado
            }
            Await _remedidaDataService.Add(nuevaRemedida)
            ' Await _dataService.Add(nuevaRemedida)
            MessageBox.Show(ClienteId.ToString()) 'Implementar un servicio para esto
            MessageBox.Show("Remedida guardada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information) 'Implementar un servicio para esto
        Catch ex As Exception
            MessageBox.Show($"Error al guardar la remedida: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error) 'Implementar un servicio para esto
        End Try
    End Sub

    Private Async Sub BuscarCliente(rtnCliente As String)
        rtnCliente = _rntCliente
        Try
            Dim cliente = Await _clienteDataService.GetByRtn(rtnCliente)
            If cliente IsNot Nothing Then
                NombreCliente = cliente.Nombre
                Representante = cliente.Representante
                Me.ClienteId = cliente.Id
            Else
                MessageBox.Show("Cliente no encontrado.", "Información", MessageBoxButton.OK, MessageBoxImage.Information) 'Implementar un servicio para esto
                NombreCliente = String.Empty
                Representante = String.Empty
            End If
        Catch ex As Exception
            MessageBox.Show($"Error al buscar el cliente: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error) 'Implementar un servicio para esto
        End Try
    End Sub
End Class
