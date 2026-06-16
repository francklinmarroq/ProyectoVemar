Imports Microsoft.Extensions.DependencyInjection
Public Class DetalleClienteWindowFactory : Implements IWindowFactory(Of DetalleClienteWindow)
    Private ReadOnly _sp As IServiceProvider
    Public Sub New(sp As IServiceProvider)
        _sp = sp
    End Sub
    Public Function CreateWindow() As DetalleClienteWindow Implements IWindowFactory(Of DetalleClienteWindow).CreateWindow
        Return New DetalleClienteWindow With {.DataContext = _sp.GetRequiredService(Of DetalleClienteViewModel)()}
    End Function
End Class
