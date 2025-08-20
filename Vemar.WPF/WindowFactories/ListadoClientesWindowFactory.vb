Public Class ListadoClientesWindowFactory : Implements IWindowFactory(Of ListadoClientesWindow)
    Private ReadOnly _viewModelAbstractFactory As IViewModelAbstractFactory

    Public Sub New(viewModelAbstractFactory As IViewModelAbstractFactory)
        _viewModelAbstractFactory = viewModelAbstractFactory
    End Sub

    Public Function CreateWindow() As ListadoClientesWindow Implements IWindowFactory(Of ListadoClientesWindow).CreateWindow
        Dim window As New ListadoClientesWindow With {.DataContext = _viewModelAbstractFactory.CreateViewModel(ViewModelType.ListadoClientesViewModel)}
        Return window
    End Function
End Class
