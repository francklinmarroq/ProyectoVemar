Imports Microsoft.Extensions.DependencyInjection
Public Class DetalleTramiteWindowFactory : Implements IWindowFactory(Of DetalleTramiteWindow)
    Private ReadOnly _sp As IServiceProvider
    Public Sub New(sp As IServiceProvider)
        _sp = sp
    End Sub
    Public Function CreateWindow() As DetalleTramiteWindow Implements IWindowFactory(Of DetalleTramiteWindow).CreateWindow
        Return New DetalleTramiteWindow With {.DataContext = _sp.GetRequiredService(Of DetalleTramiteViewModel)()}
    End Function
End Class
