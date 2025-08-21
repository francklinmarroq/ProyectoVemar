Public Class DetalleRemedidaWindowFactory : Implements IWindowFactory(Of DetalleRemedidaWindow)
    Private ReadOnly _viewModelAbstractFactory As IViewModelAbstractFactory
    Public Sub New(viewModelAbstractFactory As IViewModelAbstractFactory)
        _viewModelAbstractFactory = viewModelAbstractFactory
    End Sub
    Public Function CreateWindow() As DetalleRemedidaWindow Implements IWindowFactory(Of DetalleRemedidaWindow).CreateWindow
        Dim window As New DetalleRemedidaWindow With {.DataContext = _viewModelAbstractFactory.CreateViewModel(ViewModelType.DetalleRemedidaViewModel)}
        Return window
    End Function

End Class
