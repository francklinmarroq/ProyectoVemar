Imports Microsoft.Extensions.DependencyInjection
Public Class DetalleContratistaWindowFactory : Implements IWindowFactory(Of DetalleContratistaWindow)
    Private ReadOnly _sp As IServiceProvider
    Public Sub New(sp As IServiceProvider)
        _sp = sp
    End Sub
    Public Function CreateWindow() As DetalleContratistaWindow Implements IWindowFactory(Of DetalleContratistaWindow).CreateWindow
        Return New DetalleContratistaWindow With {.DataContext = _sp.GetRequiredService(Of DetalleContratistaViewModel)()}
    End Function
End Class
