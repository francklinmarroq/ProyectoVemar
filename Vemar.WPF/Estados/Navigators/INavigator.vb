Public Enum ViewType
    Inicio
    Remedidas
End Enum

Public Interface INavigator
    Property CurrentViewModel As ViewModelBase
    ReadOnly Property UpdateCurrentViewModelCommand As ICommand

End Interface
