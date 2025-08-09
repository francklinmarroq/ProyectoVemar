Public Class AgregarClienteViewModelFactory : Implements IVemarViewModelFactory(Of AgregarClienteViewModel)
    Public Function CreateViewModel() As AgregarClienteViewModel Implements IVemarViewModelFactory(Of AgregarClienteViewModel).CreateViewModel
        Return New AgregarClienteViewModel()
    End Function

End Class
