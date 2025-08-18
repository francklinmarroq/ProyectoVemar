Public Class InicioViewModel : Inherits ViewModelBase
    Public ReadOnly Property RemedidasViewModel As New RemedidasViewModel() 'cambiar esto para que use DI
    Public ReadOnly Property AgregarClienteWindowFactory As New AgregarClienteWindowFactory() 'cambiar esto para que use DI
    Public Property AbrirAgregarClienteCommand As ICommand

    Public Sub New()
        AbrirAgregarClienteCommand = New RelayCommand(AddressOf AbrirAgregarCliente, AddressOf PuedeEjecutar)
    End Sub

    Private Sub AbrirAgregarCliente(param As Object)
        AgregarClienteWindowFactory.CreateWindow.Show()
    End Sub
    Private Function PuedeEjecutar(param As Object) As Boolean
        Return True ' O tu condición
    End Function

End Class
