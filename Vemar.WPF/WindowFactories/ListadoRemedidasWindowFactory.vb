Public Class ListadoRemedidasWindowFactory : Implements IWindowFactory(Of ListadoRemedidasWindow)
    Private ReadOnly _viewModelAbstractFactory As IViewModelAbstractFactory
    Public Sub New(viewModelFactory As IViewModelAbstractFactory)
        _viewModelAbstractFactory = viewModelFactory
    End Sub
    Public Function CreateWindow() As ListadoRemedidasWindow Implements IWindowFactory(Of ListadoRemedidasWindow).CreateWindow
        Dim window As New ListadoRemedidasWindow With {.DataContext = _viewModelAbstractFactory.CreateViewModel(ViewModelType.ListadoRemedidasViewModel)}
        Return window
    End Function
End Class
