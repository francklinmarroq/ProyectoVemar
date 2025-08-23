Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Class ConfigViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _configurationService As IConfigurationService
    Private ReadOnly _dbDiscoveryService As IDbDiscoveryService
    Private _selectedServer As String
    Private _selectedDb As String

    Public ReadOnly Property AvailableServers As New ObservableCollection(Of String)
    Public ReadOnly Property AvailableDatabases As New ObservableCollection(Of String)
    Public ReadOnly Property LoadServersCommand As ICommand
    Public ReadOnly Property SaveConfigCommand As ICommand

    Public Property SelectedServer
        Get
            Return _selectedServer
        End Get
        Set(value)
            If _selectedServer <> value Then
                _selectedServer = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SelectedServer)))
                If Not String.IsNullOrWhiteSpace(value) Then
                    Dim a As Task = LoadDatabasesAsync(value)
                End If
            End If
        End Set
    End Property

    Public Property SelectedDatabase
        Get
            Return _selectedDb
        End Get
        Set(value)
            If _selectedDb <> value Then
                _selectedDb = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SelectedDatabase)))
            End If
        End Set
    End Property
    Public Sub New(configurationService As IConfigurationService, dbDiscoveryService As IDbDiscoveryService)
        _configurationService = configurationService
        _dbDiscoveryService = dbDiscoveryService

        LoadServersCommand = New RelayCommand(AddressOf LoadServersAsync)
        SaveConfigCommand = New RelayCommand(AddressOf SaveConfig)
    End Sub
    Private Async Function LoadServersAsync(obj As Object) As Task
        AvailableServers.Clear()
        Dim servers = Await _dbDiscoveryService.GetSqlServerAsync()
        For Each server In servers
            AvailableServers.Add(server)
        Next
    End Function

    Private Async Function LoadDatabasesAsync(serverName As String) As Task
        AvailableDatabases.Clear()
        Dim databases = Await _dbDiscoveryService.GetDatabaseNamesAsync(serverName)
        For Each db In databases
            AvailableDatabases.Add(db)
        Next
    End Function

    Private Sub SaveConfig(obj As Object)
        'Logica para guardar la configuración
    End Sub


    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

End Class
