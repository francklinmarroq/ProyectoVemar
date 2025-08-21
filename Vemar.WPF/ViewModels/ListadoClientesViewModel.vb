Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class ListadoClientesViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _dataService As IDataService(Of Cliente)
    Public ReadOnly Property EliminarClienteCommand As ICommand
    Public Property Clientes As ObservableCollection(Of Cliente)

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New(dataService As IDataService(Of Cliente))
        _dataService = dataService
        CargarClientes()
        EliminarClienteCommand = New RelayCommand(AddressOf EliminarCliente)
    End Sub

    Private Sub EliminarCliente()
        'MsgBox(cliente.ToString() & " ha sido eliminado.")
    End Sub

    Public Async Sub CargarClientes()
        Try
            Dim clientesList = Await _dataService.GetAll()
            Clientes = New ObservableCollection(Of Cliente)(clientesList)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Clientes)))
        Catch ex As Exception
            ' Manejo de errores, por ejemplo, mostrar un mensaje al usuario
            MsgBox("Error al cargar los clientes: " & ex.Message & Clientes.ToString()) 'implementar un servicio para esto
        End Try
    End Sub
End Class
