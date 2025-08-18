
Public Class MainViewModel : Inherits ViewModelBase
    Public Property Navigator As INavigator
    Public Property WindowService As IWindowService
    Public Property CerrarAppCommand As ICommand

    Public Sub New(navigator As INavigator, windowService As IWindowService)
        Me.Navigator = navigator
        Me.WindowService = windowService
        CerrarAppCommand = New RelayCommand(AddressOf CerrarApp, AddressOf PuedeCerrarApp)
    End Sub

    Private Function PuedeCerrarApp(obj As Object) As Boolean
        Return True ' O tu condición
    End Function

    Private Sub CerrarApp(obj As Object)
        Application.Current.Shutdown()
    End Sub
End Class
