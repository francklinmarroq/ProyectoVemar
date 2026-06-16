Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class ListadoRemedidasViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _dataService As IDataService(Of Remedida)
    Private _remedidas As ObservableCollection(Of Remedida)

    Public Property Remedidas As ObservableCollection(Of Remedida)
        Get
            Return _remedidas
        End Get
        Set(value As ObservableCollection(Of Remedida))
            _remedidas = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Remedidas)))
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New(dataService As IDataService(Of Remedida))
        _dataService = dataService
        CargarRemedidas()
    End Sub

    Public Async Sub CargarRemedidas()
        Try
            Dim lista = Await _dataService.GetAll()
            Remedidas = New ObservableCollection(Of Remedida)(lista)
        Catch ex As Exception
            MessageBox.Show("Error al cargar las remedidas: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class

