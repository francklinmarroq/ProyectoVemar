Public Interface IWindowService
    Sub Show(winType As WindowType)
    Function ShowDialog(winType As WindowType) As Boolean?

End Interface
