Imports Microsoft.Extensions.DependencyInjection
Public Class DetalleProyectoWindowFactory : Implements IWindowFactory(Of DetalleProyectoWindow)
    Private ReadOnly _sp As IServiceProvider
    Public Sub New(sp As IServiceProvider)
        _sp = sp
    End Sub
    Public Function CreateWindow() As DetalleProyectoWindow Implements IWindowFactory(Of DetalleProyectoWindow).CreateWindow
        Return New DetalleProyectoWindow With {.DataContext = _sp.GetRequiredService(Of DetalleProyectoViewModel)()}
    End Function
End Class
