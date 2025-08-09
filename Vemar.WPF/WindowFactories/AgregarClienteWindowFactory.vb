Public Class AgregarClienteWindowFactory : Implements IWindowFactory(Of AgregarClienteWindow)

    Public Function CreateWindow() As AgregarClienteWindow Implements IWindowFactory(Of AgregarClienteWindow).CreateWindow
        Return New AgregarClienteWindow()
    End Function
End Class
