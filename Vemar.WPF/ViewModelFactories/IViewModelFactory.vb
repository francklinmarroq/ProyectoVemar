Public Interface IViewModelFactory(Of T As ViewModelBase)
    Function CreateViewModel() As T
End Interface
