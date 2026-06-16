Imports Microsoft.Extensions.DependencyInjection
Public Class ListadoRemedidasWindowFactory : Implements IWindowFactory(Of ListadoRemedidasWindow)
    Private ReadOnly _sp As IServiceProvider
    Public Sub New(sp As IServiceProvider)
        _sp = sp
    End Sub
    Public Function CreateWindow() As ListadoRemedidasWindow Implements IWindowFactory(Of ListadoRemedidasWindow).CreateWindow
        Return New ListadoRemedidasWindow With {.DataContext = _sp.GetRequiredService(Of ListadoRemedidasViewModel)()}
    End Function
End Class
