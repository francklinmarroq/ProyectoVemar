Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class ListadoContratistasViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _dataService As IDataService(Of Contratista)
    Private _contratistas As ObservableCollection(Of Contratista)

    Public Property Contratistas As ObservableCollection(Of Contratista)
        Get
            Return _contratistas
        End Get
        Set(value As ObservableCollection(Of Contratista))
            _contratistas = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Contratistas)))
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New(dataService As IDataService(Of Contratista))
        _dataService = dataService
        CargarContratistas()
    End Sub

    Public Async Sub CargarContratistas()
        Try
            Dim lista = Await _dataService.GetAll()
            Contratistas = New ObservableCollection(Of Contratista)(lista)
        Catch ex As Exception
            MessageBox.Show("Error al cargar contratistas: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class

