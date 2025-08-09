Public Interface IWindowService
    Sub Show(Of TViewModel As ViewModelBase)()
    Function ShowDialog(Of TViewModel As ViewModelBase)() As Boolean?

End Interface
