Imports Microsoft.Extensions.DependencyInjection
Public Class ListadoColaboradoresWindowFactory : Implements IWindowFactory(Of ListadoColaboradoresWindow)
    Private ReadOnly _sp As IServiceProvider
    Public Sub New(sp As IServiceProvider)
        _sp = sp
    End Sub
    Public Function CreateWindow() As ListadoColaboradoresWindow Implements IWindowFactory(Of ListadoColaboradoresWindow).CreateWindow
        Return New ListadoColaboradoresWindow With {.DataContext = _sp.GetRequiredService(Of ListadoColaboradoresViewModel)()}
    End Function
End Class
