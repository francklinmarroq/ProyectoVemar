Imports Microsoft.Extensions.DependencyInjection
Public Class ListadoTramitesWindowFactory : Implements IWindowFactory(Of ListadoTramitesWindow)
    Private ReadOnly _sp As IServiceProvider
    Public Sub New(sp As IServiceProvider)
        _sp = sp
    End Sub
    Public Function CreateWindow() As ListadoTramitesWindow Implements IWindowFactory(Of ListadoTramitesWindow).CreateWindow
        Return New ListadoTramitesWindow With {.DataContext = _sp.GetRequiredService(Of ListadoTramitesViewModel)()}
    End Function
End Class
