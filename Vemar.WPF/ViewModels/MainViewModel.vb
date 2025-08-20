Public Class MainViewModel : Inherits ViewModelBase

    Public ReadOnly Property ExitApplicationCommand As ICommand
    Public ReadOnly Property AbrirAgregarClienteCommand As ICommand
    Public ReadOnly Property AbrirListadoClientesCommand As ICommand
    Private _windowService As IWindowService


    Public Sub New(windowService As IWindowService)
        _windowService = windowService

        ' Inicializar el comando de salida
        ExitApplicationCommand = New RelayCommand(AddressOf ExitApplication)
        AbrirAgregarClienteCommand = New RelayCommand(AddressOf AbrirAgregarCliente)
        AbrirListadoClientesCommand = New RelayCommand(AddressOf AbrirListadoClientes)
    End Sub

    Private Sub AbrirListadoClientes()
        _windowService.Show(WindowType.ListadoClientesWindow)
    End Sub

    Private Sub ExitApplication()
        ' Lógica para salir de la aplicación
        Application.Current.Shutdown()
    End Sub

    Private Sub AbrirAgregarCliente()
        ' Lógica para abrir la ventana de agregar cliente
        _windowService.Show(WindowType.DetalleClienteWindow)

    End Sub

End Class
