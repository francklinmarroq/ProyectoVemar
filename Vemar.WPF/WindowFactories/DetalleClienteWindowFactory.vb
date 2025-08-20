Public Class DetalleClienteWindowFactory : Implements IWindowFactory(Of DetalleClienteWindow)
    Private ReadOnly _viewModelAbstractFactory As IViewModelAbstractFactory

    Public Sub New(viewModelAbstractFactory As IViewModelAbstractFactory)
        _viewModelAbstractFactory = viewModelAbstractFactory
    End Sub

    Public Function CreateWindow() As DetalleClienteWindow Implements IWindowFactory(Of DetalleClienteWindow).CreateWindow
        Dim window As New DetalleClienteWindow With {.DataContext = _viewModelAbstractFactory.CreateViewModel(ViewModelType.DetalleClienteViewModel)}
        Return window
    End Function

End Class
