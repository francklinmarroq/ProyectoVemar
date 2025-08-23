Public Class ViewModelAbstractFactory : Implements IViewModelAbstractFactory
    Private ReadOnly _detalleClienteViewModelFactory As IViewModelFactory(Of DetalleClienteViewModel)
    Private ReadOnly _listadoClientesViewModelFactory As IViewModelFactory(Of ListadoClientesViewModel)
    Private ReadOnly _detalleRemedidaViewModelFactory As IViewModelFactory(Of DetalleRemedidaViewModel)
    Private ReadOnly _listadoRemedidasViewModelFactory As IViewModelFactory(Of ListadoRemedidasViewModel)
    Private ReadOnly _configViewModelFactory As IViewModelFactory(Of ConfigViewModel)

    Public Sub New(detalleClienteFactory As IViewModelFactory(Of DetalleClienteViewModel), listadoClientesViewModelFactory As IViewModelFactory(Of ListadoClientesViewModel), detalleRemedidaViewModelFactory As IViewModelFactory(Of DetalleRemedidaViewModel), listadoRemedidasViewModelFactory As IViewModelFactory(Of ListadoRemedidasViewModel), configViewModelFactory As IViewModelFactory(Of ConfigViewModel))
        _detalleClienteViewModelFactory = detalleClienteFactory
        _listadoClientesViewModelFactory = listadoClientesViewModelFactory
        _detalleRemedidaViewModelFactory = detalleRemedidaViewModelFactory
        _listadoRemedidasViewModelFactory = listadoRemedidasViewModelFactory
        _configViewModelFactory = configViewModelFactory
    End Sub

    Public Function CreateViewModel(vmType As ViewModelType) As ViewModelBase Implements IViewModelAbstractFactory.CreateViewModel
        Select Case vmType
            Case vmType.DetalleClienteViewModel
                Return _detalleClienteViewModelFactory.CreateViewModel()
            Case vmType.ListadoClientesViewModel
                Return _listadoClientesViewModelFactory.CreateViewModel()
            Case vmType.DetalleRemedidaViewModel
                Return _detalleRemedidaViewModelFactory.CreateViewModel()
            Case vmType.ListadoRemedidasViewModel
                Return _listadoRemedidasViewModelFactory.CreateViewModel()
            Case vmType.ConfigViewModel
                Return _configViewModelFactory.CreateViewModel()
            Case Else
                Throw New ArgumentException("ViewModel type not supported.")
        End Select
    End Function
End Class
