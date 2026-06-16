Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class ListadoClientesViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _dataService As IDataService(Of Cliente)
    Public ReadOnly Property EliminarClienteCommand As ICommand
    Public ReadOnly Property EditarClienteCommand As ICommand
    Public Property Clientes As ObservableCollection(Of Cliente)

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New(dataService As IDataService(Of Cliente))
        _dataService = dataService
        CargarClientes()
        EliminarClienteCommand = New RelayCommand(Of Cliente)(AddressOf EliminarCliente)
        EditarClienteCommand = New RelayCommand(Of Cliente)(AddressOf EditarCliente)
    End Sub

    Private Sub EliminarCliente(cliente As Cliente)
        If cliente Is Nothing Then Return
        If MsgBox("¿Desea eliminar el cliente: " & cliente.Nombre & "?", MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            Try
                _dataService.Delete(cliente.Id)
                Clientes.Remove(cliente)
            Catch ex As Exception
                MsgBox("Error al eliminar el cliente: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub EditarCliente(cliente As Cliente)
        If cliente Is Nothing Then Return
        MsgBox("Editar cliente: " & cliente.Nombre)
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

