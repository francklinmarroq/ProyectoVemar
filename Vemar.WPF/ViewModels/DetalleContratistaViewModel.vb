Imports System.ComponentModel
Imports Vemar.Domain

Public Class DetalleContratistaViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _dataService As IDataService(Of Contratista)
    Private _guardarCommand As RelayCommand

    Private _nombre As String = String.Empty
    Private _telefono As String = String.Empty

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

    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New(dataService As IDataService(Of Contratista))
        _dataService = dataService
        _guardarCommand = New RelayCommand(AddressOf Guardar, AddressOf CanGuardar)
    End Sub

    Public Function CanGuardar() As Boolean
        Return Not String.IsNullOrWhiteSpace(Nombre)
    End Function

    Public Async Sub Guardar()
        Try
            Dim contratista As New Contratista With {
                .Nombre = Nombre,
                .Telefono = Telefono
            }
            Await _dataService.Add(contratista)
            MessageBox.Show("Contratista guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information)
        Catch ex As Exception
            MessageBox.Show($"Error al guardar el contratista: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class

