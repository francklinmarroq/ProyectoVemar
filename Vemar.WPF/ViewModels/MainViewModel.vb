
Public Class MainViewModel : Inherits ViewModelBase
    Public Property Navigator As INavigator
    Public Property WindowService As IWindowService

    Public Sub New(navigator As INavigator, windowService As IWindowService)
        Me.Navigator = navigator
        Me.WindowService = windowService
        windowService.Show(Of AgregarClienteViewModel)()
    End Sub
End Class
