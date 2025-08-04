Public Enum ViewType
    Inicio
    Remedidas
End Enum

Public Interface INavigator
    Property CurrentViewModel As ViewModelBase
    ReadOnly Property UpdateViewModel As ICommand

End Interface
