Imports System.ComponentModel

Public Class Navigator : Inherits ObservableObject : Implements INavigator
    Private _currentViewModel As ViewModelBase = New InicioViewModel()

    Public Sub New(viewModelFactory As IVemarViewModelAbstractFactory)
        UpdateCurrentViewModelCommand = New UpdateCurrentViewModelCommand(Me, viewModelFactory)
    End Sub

    Public Property CurrentViewModel As ViewModelBase Implements INavigator.CurrentViewModel
        Get
            Return _currentViewModel
        End Get
        Set(value As ViewModelBase)
            _currentViewModel = value
            OnPropertyChanged(NameOf(CurrentViewModel))
        End Set
    End Property

    Public Property UpdateCurrentViewModelCommand As ICommand Implements INavigator.UpdateCurrentViewModelCommand


End Class
