Imports Vemar.Domain

Public Class ListadoRemedidasViewModel : Inherits ViewModelBase
    Private ReadOnly _dataService As IDataService(Of Remedida)
    Public Sub New(dataService As IDataService(Of Remedida))
        _dataService = dataService
    End Sub

    Public Async Sub CargarRemedidas()
        'Try
        '    Dim remedidasList = Await _dataService.GetAll()
        '    Remedidas = New ObservableCollection(Of Remedida)(remedidasList)
        '    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Remedidas)))
        'Catch ex As Exception
        '    ' Manejo de errores, por ejemplo, mostrar un mensaje al usuario
        '    MsgBox("Error al cargar las remedidas: " & ex.Message)
        'End Try
    End Sub

End Class
