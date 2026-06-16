Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class ListadoColaboradoresViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _dataService As IDataService(Of Colaborador)
    Private _colaboradores As ObservableCollection(Of Colaborador)

    Public Property Colaboradores As ObservableCollection(Of Colaborador)
        Get
            Return _colaboradores
        End Get
        Set(value As ObservableCollection(Of Colaborador))
            _colaboradores = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Colaboradores)))
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New(dataService As IDataService(Of Colaborador))
        _dataService = dataService
        CargarColaboradores()
    End Sub

    Public Async Sub CargarColaboradores()
        Try
            Dim lista = Await _dataService.GetAll()
            Colaboradores = New ObservableCollection(Of Colaborador)(lista)
        Catch ex As Exception
            MessageBox.Show("Error al cargar colaboradores: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class

