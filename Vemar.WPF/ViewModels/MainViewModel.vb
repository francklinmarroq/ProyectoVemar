
Public Class MainViewModel : Inherits ViewModelBase
    Public Property Navigator As INavigator

    Public Sub New(nav As INavigator)
        Navigator = nav
        Navigator.UpdateCurrentViewModelCommand.Execute(ViewType.Inicio)
    End Sub

End Class
