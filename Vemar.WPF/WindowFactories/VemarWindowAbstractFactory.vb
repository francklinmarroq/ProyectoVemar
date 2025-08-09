Public Class VemarWindowAbstractFactory : Implements IVemarWindowAbstractFactory
    Private ReadOnly _agregarClienteViewModel As IVemarViewModelFactory(Of AgregarClienteViewModel)
    Private ReadOnly _agregarClienteWindowFactory As IWindowFactory(Of AgregarClienteWindow)

    Public Sub New(agregarClienteViewModel As IVemarViewModelFactory(Of AgregarClienteViewModel), agregarClienteWindowFactory As IWindowFactory(Of AgregarClienteWindow))
        _agregarClienteViewModel = agregarClienteViewModel
        _agregarClienteWindowFactory = agregarClienteWindowFactory
    End Sub

    Public Function CreateWindow(viewModelType As Type) As Window Implements IVemarWindowAbstractFactory.CreateWindow
        Select Case viewModelType.Name
            Case NameOf(AgregarClienteViewModel)
                Dim viewModel As AgregarClienteViewModel = _agregarClienteViewModel.CreateViewModel()
                Dim window As Window = _agregarClienteWindowFactory.CreateWindow()
                window.DataContext = viewModel
                Return window
            Case Else
                Throw New ArgumentException($"No se puede crear una ventana para el ViewModel {viewModelType.Name}. Asegúrate de que el ViewModel esté registrado en la fábrica.", NameOf(viewModelType))
        End Select

    End Function
End Class
