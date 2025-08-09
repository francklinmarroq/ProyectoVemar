Public Class WindowService : Implements IWindowService
    'Private ReadOnly _windowFactory As Func(Of Type, Window)
    Private ReadOnly _windowFactory As IVemarWindowAbstractFactory

    Public Sub New(windowFactory As IVemarWindowAbstractFactory)
        _windowFactory = windowFactory
    End Sub

    Public Sub Show(Of TViewModel As ViewModelBase)() Implements IWindowService.Show
        Dim window = _windowFactory.CreateWindow(GetType(TViewModel))
        window.Show()
    End Sub

    Public Function ShowDialog(Of TViewModel As ViewModelBase)() As Boolean? Implements IWindowService.ShowDialog
        Throw New NotImplementedException()
    End Function
End Class
