Imports Microsoft.Extensions.DependencyInjection
Public Class ListadoClientesWindowFactory : Implements IWindowFactory(Of ListadoClientesWindow)
    Private ReadOnly _sp As IServiceProvider
    Public Sub New(sp As IServiceProvider)
        _sp = sp
    End Sub
    Public Function CreateWindow() As ListadoClientesWindow Implements IWindowFactory(Of ListadoClientesWindow).CreateWindow
        Return New ListadoClientesWindow With {.DataContext = _sp.GetRequiredService(Of ListadoClientesViewModel)()}
    End Function
End Class
