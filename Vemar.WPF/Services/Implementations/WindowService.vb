Public Class WindowService : Implements IWindowService
    Private ReadOnly _windowAbstractFactory As IWindowAbstractFactory


    Public Sub New(windowFactory As IWindowAbstractFactory)
        _windowAbstractFactory = windowFactory
    End Sub

    Public Sub Show(winType As WindowType) Implements IWindowService.Show
        _windowAbstractFactory.CreateWindow(winType).Show()
    End Sub

    Public Function ShowDialog(winType As WindowType) As Boolean? Implements IWindowService.ShowDialog
        _windowAbstractFactory.CreateWindow(winType).ShowDialog()
    End Function
End Class
