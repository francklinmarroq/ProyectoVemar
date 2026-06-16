Imports System.ComponentModel
Imports Vemar.Domain

Public Class DetalleColaboradorViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _dataService As IDataService(Of Colaborador)
    Private _guardarCommand As RelayCommand

    Private _dni As String = String.Empty
    Private _nombre As String = String.Empty
    Private _telefono As String = String.Empty
    Private _fechaNacimiento As Date = Date.Today
    Private _domicilio As String = String.Empty
    Private _email As String = String.Empty

    Public Property Dni As String
        Get
            Return _dni
        End Get
        Set(value As String)
            If _dni <> value Then
                _dni = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Dni)))
                _guardarCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property

    Public Property Nombre As String
        Get
            Return _nombre
        End Get
        Set(value As String)
            If _nombre <> value Then
                _nombre = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Nombre)))
                _guardarCommand.RaiseCanExecuteChanged()
            End If
        End Set
    End Property

    Public Property Telefono As String
        Get
            Return _telefono
        End Get
        Set(value As String)
            If _telefono <> value Then
                _telefono = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Telefono)))
            End If
        End Set
    End Property

    Public Property FechaNacimiento As Date
        Get
            Return _fechaNacimiento
        End Get
        Set(value As Date)
            If _fechaNacimiento <> value Then
                _fechaNacimiento = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(FechaNacimiento)))
            End If
        End Set
    End Property

    Public Property Domicilio As String
        Get
            Return _domicilio
        End Get
        Set(value As String)
            If _domicilio <> value Then
                _domicilio = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Domicilio)))
            End If
        End Set
    End Property

    Public Property Email As String
        Get
            Return _email
        End Get
        Set(value As String)
            If _email <> value Then
                _email = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Email)))
            End If
        End Set
    End Property

    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New(dataService As IDataService(Of Colaborador))
        _dataService = dataService
        _guardarCommand = New RelayCommand(AddressOf Guardar, AddressOf CanGuardar)
    End Sub

    Public Function CanGuardar() As Boolean
        Return Not String.IsNullOrWhiteSpace(Nombre) AndAlso Not String.IsNullOrWhiteSpace(Dni)
    End Function

    Public Async Sub Guardar()
        Try
            Dim colaborador As New Colaborador With {
                .Dni = Dni,
                .Nombre = Nombre,
                .Telefono = Telefono,
                .FechaNacimiento = FechaNacimiento,
                .Domicilio = Domicilio,
                .Email = Email
            }
            Await _dataService.Add(colaborador)
            MessageBox.Show("Colaborador guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information)
        Catch ex As Exception
            MessageBox.Show($"Error al guardar el colaborador: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class

