Imports Microsoft.Extensions.DependencyInjection
Public Class DetalleRemedidaWindowFactory : Implements IWindowFactory(Of DetalleRemedidaWindow)
    Private ReadOnly _sp As IServiceProvider
    Public Sub New(sp As IServiceProvider)
        _sp = sp
    End Sub
    Public Function CreateWindow() As DetalleRemedidaWindow Implements IWindowFactory(Of DetalleRemedidaWindow).CreateWindow
        Return New DetalleRemedidaWindow With {.DataContext = _sp.GetRequiredService(Of DetalleRemedidaViewModel)()}
    End Function
End Class
