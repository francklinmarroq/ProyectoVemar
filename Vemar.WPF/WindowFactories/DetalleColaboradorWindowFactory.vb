Imports Microsoft.Extensions.DependencyInjection
Public Class DetalleColaboradorWindowFactory : Implements IWindowFactory(Of DetalleColaboradorWindow)
    Private ReadOnly _sp As IServiceProvider
    Public Sub New(sp As IServiceProvider)
        _sp = sp
    End Sub
    Public Function CreateWindow() As DetalleColaboradorWindow Implements IWindowFactory(Of DetalleColaboradorWindow).CreateWindow
        Return New DetalleColaboradorWindow With {.DataContext = _sp.GetRequiredService(Of DetalleColaboradorViewModel)()}
    End Function
End Class
