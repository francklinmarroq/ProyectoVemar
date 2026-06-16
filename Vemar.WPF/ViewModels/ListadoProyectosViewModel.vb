Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class ListadoProyectosViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _dataService As IDataService(Of Proyecto)
    Private _proyectos As ObservableCollection(Of Proyecto)

    Public Property Proyectos As ObservableCollection(Of Proyecto)
        Get
            Return _proyectos
        End Get
        Set(value As ObservableCollection(Of Proyecto))
            _proyectos = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Proyectos)))
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New(dataService As IDataService(Of Proyecto))
        _dataService = dataService
        CargarProyectos()
    End Sub

    Public Async Sub CargarProyectos()
        Try
            Dim lista = Await _dataService.GetAll()
            Proyectos = New ObservableCollection(Of Proyecto)(lista)
        Catch ex As Exception
            MessageBox.Show("Error al cargar proyectos: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class

