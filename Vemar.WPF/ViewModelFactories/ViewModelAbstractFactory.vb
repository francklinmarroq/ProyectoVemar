Public Class ViewModelAbstractFactory : Implements IViewModelAbstractFactory
    Private ReadOnly _detalleClienteViewModelFactory As IViewModelFactory(Of DetalleClienteViewModel)
    Private ReadOnly _listadoClientesViewModelFactory As IViewModelFactory(Of ListadoClientesViewModel)

    Public Sub New(detalleClienteFactory As IViewModelFactory(Of DetalleClienteViewModel), listadoClientesViewModelFactory As IViewModelFactory(Of ListadoClientesViewModel))
        _detalleClienteViewModelFactory = detalleClienteFactory
        _listadoClientesViewModelFactory = listadoClientesViewModelFactory
    End Sub

    Public Function CreateViewModel(vmType As ViewModelType) As ViewModelBase Implements IViewModelAbstractFactory.CreateViewModel
        Select Case vmType
            Case vmType.DetalleClienteViewModel
                Return _detalleClienteViewModelFactory.CreateViewModel()
            Case vmType.ListadoClientesViewModel
                Return _listadoClientesViewModelFactory.CreateViewModel()
            Case Else
                Throw New ArgumentException("ViewModel type not supported.")
        End Select
    End Function
End Class
