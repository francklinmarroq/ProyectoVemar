Imports Microsoft.Extensions.DependencyInjection
Public Class ListadoProyectosWindowFactory : Implements IWindowFactory(Of ListadoProyectosWindow)
    Private ReadOnly _sp As IServiceProvider
    Public Sub New(sp As IServiceProvider)
        _sp = sp
    End Sub
    Public Function CreateWindow() As ListadoProyectosWindow Implements IWindowFactory(Of ListadoProyectosWindow).CreateWindow
        Return New ListadoProyectosWindow With {.DataContext = _sp.GetRequiredService(Of ListadoProyectosViewModel)()}
    End Function
End Class
