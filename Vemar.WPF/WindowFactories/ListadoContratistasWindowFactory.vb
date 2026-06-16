Imports Microsoft.Extensions.DependencyInjection
Public Class ListadoContratistasWindowFactory : Implements IWindowFactory(Of ListadoContratistasWindow)
    Private ReadOnly _sp As IServiceProvider
    Public Sub New(sp As IServiceProvider)
        _sp = sp
    End Sub
    Public Function CreateWindow() As ListadoContratistasWindow Implements IWindowFactory(Of ListadoContratistasWindow).CreateWindow
        Return New ListadoContratistasWindow With {.DataContext = _sp.GetRequiredService(Of ListadoContratistasViewModel)()}
    End Function
End Class
