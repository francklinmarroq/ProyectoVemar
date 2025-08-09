Public Class InicioViewModelFactory : Implements IVemarViewModelFactory(Of InicioViewModel)
    Public Function CreateViewModel() As InicioViewModel Implements IVemarViewModelFactory(Of InicioViewModel).CreateViewModel
        Return New InicioViewModel()
    End Function
End Class
