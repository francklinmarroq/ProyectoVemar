Public Class RemedidasViewModelFactory : Implements IVemarViewModelFactory(Of RemedidasViewModel)
    Public Function CreateViewModel() As RemedidasViewModel Implements IVemarViewModelFactory(Of RemedidasViewModel).CreateViewModel
        Return New RemedidasViewModel()
    End Function

End Class
