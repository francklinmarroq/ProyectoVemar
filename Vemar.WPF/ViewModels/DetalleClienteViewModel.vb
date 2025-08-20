Imports System.ComponentModel
Imports Vemar.Domain

Public Class DetalleClienteViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private _nombreCliente As String
    Private _rtnCliente As String
    Private _telefonoCliente As String
    Private _direccionCliente As String
    Private _emailCliente As String
    Private _nombreRepresentante As String
    Private _dniRepresentante As String
    Private _rtnRepresentante As String
    Private _emailRepresentante As String
    Private ReadOnly _dataService As IDataService(Of Cliente)

    Private _guardarClienteCommand As RelayCommand
    Public Property NombreCliente As String
        Get
            Return _nombreCliente
        End Get
        Set(value As String)
            If _nombreCliente <> value Then
                _nombreCliente = value
                RaiseEvent ProperyChanged(Me, New PropertyChangedEventArgs(NameOf(NombreCliente)))
                _guardarClienteCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property RtnCliente As String
        Get
            Return _rtnCliente
        End Get
        Set(value As String)
            If _rtnCliente <> value Then
                _rtnCliente = value
                RaiseEvent ProperyChanged(Me, New PropertyChangedEventArgs(NameOf(RtnCliente)))
                _guardarClienteCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property TelefonoCliente As String
        Get
            Return _telefonoCliente
        End Get
        Set(value As String)
            If _telefonoCliente <> value Then
                _telefonoCliente = value
                RaiseEvent ProperyChanged(Me, New PropertyChangedEventArgs(NameOf(TelefonoCliente)))
                _guardarClienteCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property DireccionCliente As String
        Get
            Return _direccionCliente
        End Get
        Set(value As String)
            If _direccionCliente <> value Then
                _direccionCliente = value
                RaiseEvent ProperyChanged(Me, New PropertyChangedEventArgs(NameOf(DireccionCliente)))
                _guardarClienteCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property EmailCliente As String
        Get
            Return _emailCliente
        End Get
        Set(value As String)
            If _emailCliente <> value Then
                _emailCliente = value
                RaiseEvent ProperyChanged(Me, New PropertyChangedEventArgs(NameOf(EmailCliente)))
                _guardarClienteCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property NombreRepresentante As String
        Get
            Return _nombreRepresentante
        End Get
        Set(value As String)
            If _nombreRepresentante <> value Then
                _nombreRepresentante = value
                RaiseEvent ProperyChanged(Me, New PropertyChangedEventArgs(NameOf(NombreRepresentante)))
                _guardarClienteCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property DniRepresentante As String
        Get
            Return _dniRepresentante
        End Get
        Set(value As String)
            If _dniRepresentante <> value Then
                _dniRepresentante = value
                RaiseEvent ProperyChanged(Me, New PropertyChangedEventArgs(NameOf(DniRepresentante)))
                _guardarClienteCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property RtnRepresentante As String
        Get
            Return _rtnRepresentante
        End Get
        Set(value As String)
            If _rtnRepresentante <> value Then
                _rtnRepresentante = value
                RaiseEvent ProperyChanged(Me, New PropertyChangedEventArgs(NameOf(RtnRepresentante)))
                _guardarClienteCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property
    Public Property EmailRepresentante As String
        Get
            Return _emailRepresentante
        End Get
        Set(value As String)
            If _emailRepresentante <> value Then
                _emailRepresentante = value
                RaiseEvent ProperyChanged(Me, New PropertyChangedEventArgs(NameOf(EmailRepresentante)))
                _guardarClienteCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property

    Public ReadOnly Property GuardarClienteCommand As ICommand
        Get
            Return _guardarClienteCommand
        End Get
    End Property

    Public Event ProperyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New(dataService As IDataService(Of Cliente))
        ' Inicializar el servicio de datos
        _dataService = dataService
        ' Inicializar las propiedades con valores por defecto
        NombreCliente = String.Empty
        RtnCliente = String.Empty
        TelefonoCliente = String.Empty
        DireccionCliente = String.Empty
        EmailCliente = String.Empty
        NombreRepresentante = String.Empty
        DniRepresentante = String.Empty
        RtnRepresentante = String.Empty
        EmailRepresentante = String.Empty
        ' Inicializar el comando de guardar
        _guardarClienteCommand = New RelayCommand(AddressOf Guardar, AddressOf CanGuardar)
        ' Inicializar el comando de cancelar
    End Sub
    Public Async Sub Guardar()
        Try
            Dim nuevoCliente As New Cliente With {
            .Nombre = NombreCliente,
            .Rtn = RtnCliente,
            .Telefono = TelefonoCliente,
            .Direccion = DireccionCliente,
            .EmailCorporativo = EmailCliente,
            .Representante = NombreRepresentante,
            .DniRepresentante = DniRepresentante,
            .RtnRepresentante = RtnRepresentante,
            .EmailRepresentante = EmailRepresentante
        }
            Await _dataService.Add(nuevoCliente)
            MessageBox.Show("Cliente guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information)
        Catch ex As Exception
            MessageBox.Show($"Error al guardar el cliente: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try


    End Sub
    Public Function CanGuardar() As Boolean
        ' Lógica para determinar si se puede guardar
        ' Aquí puedes implementar la lógica para validar los datos antes de permitir el guardado
        Return Not String.IsNullOrWhiteSpace(NombreCliente) AndAlso
            Not String.IsNullOrWhiteSpace(RtnCliente) AndAlso
            Not String.IsNullOrWhiteSpace(TelefonoCliente) AndAlso
            Not String.IsNullOrWhiteSpace(DireccionCliente) AndAlso
            Not String.IsNullOrWhiteSpace(EmailCliente) AndAlso
            Not String.IsNullOrWhiteSpace(NombreRepresentante) AndAlso
            Not String.IsNullOrWhiteSpace(DniRepresentante) AndAlso
            Not String.IsNullOrWhiteSpace(RtnRepresentante) AndAlso
            Not String.IsNullOrWhiteSpace(EmailRepresentante)
    End Function

End Class
