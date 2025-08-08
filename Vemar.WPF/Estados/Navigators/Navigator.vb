Imports System.ComponentModel

Public Class Navigator : Inherits ObservableObject : Implements INavigator
    Private _currentViewModel As ViewModelBase = New InicioViewModel()

    Public Property CurrentViewModel As ViewModelBase Implements INavigator.CurrentViewModel
        Get
            Return _currentViewModel
        End Get
        Set(value As ViewModelBase)
            _currentViewModel = value
            OnPropertyChanged(NameOf(CurrentViewModel))
        End Set
    End Property

    Public ReadOnly Property UpdateCurrentViewModelCommand As ICommand Implements INavigator.UpdateCurrentViewModelCommand
        Get
            Return New UpdateCurrentViewModelCommand(Me)
        End Get
    End Property

End Class
