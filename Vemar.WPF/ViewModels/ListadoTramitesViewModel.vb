Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class ListadoTramitesViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _dataService As IDataService(Of Tramite)
    Private _tramites As ObservableCollection(Of Tramite)

    Public Property Tramites As ObservableCollection(Of Tramite)
        Get
            Return _tramites
        End Get
        Set(value As ObservableCollection(Of Tramite))
            _tramites = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Tramites)))
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New(dataService As IDataService(Of Tramite))
        _dataService = dataService
        CargarTramites()
    End Sub

    Public Async Sub CargarTramites()
        Try
            Dim lista = Await _dataService.GetAll()
            Tramites = New ObservableCollection(Of Tramite)(lista)
        Catch ex As Exception
            MessageBox.Show("Error al cargar trámites: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class

