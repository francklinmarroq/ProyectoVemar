Public Class WindowAbstractFactory : Implements IWindowAbstractFactory
    Private ReadOnly _detalleClienteWindowFactory As IWindowFactory(Of DetalleClienteWindow)
    Private ReadOnly _listadoClientesWindowFactory As IWindowFactory(Of ListadoClientesWindow)
    Private ReadOnly _detalleRemedidaWindowFactory As IWindowFactory(Of DetalleRemedidaWindow)
    Private ReadOnly _listadoRemedidasWindowFactory As IWindowFactory(Of ListadoRemedidasWindow)
    Public Sub New(detalleClienteFactory As IWindowFactory(Of DetalleClienteWindow), listadoClientesWindowFactory As IWindowFactory(Of ListadoClientesWindow), detalleRemedidaWindowFactory As IWindowFactory(Of DetalleRemedidaWindow), listadoRemedidasWindowFactory As IWindowFactory(Of ListadoRemedidasWindow))
        _detalleClienteWindowFactory = detalleClienteFactory
        _listadoClientesWindowFactory = listadoClientesWindowFactory
        _detalleRemedidaWindowFactory = detalleRemedidaWindowFactory
        _listadoRemedidasWindowFactory = listadoRemedidasWindowFactory
    End Sub
    Public Function CreateWindow(winType As WindowType) As Window Implements IWindowAbstractFactory.CreateWindow
        Select Case winType
            Case winType.DetalleClienteWindow
                Return _detalleClienteWindowFactory.CreateWindow()
            Case winType.ListadoClientesWindow
                Return _listadoClientesWindowFactory.CreateWindow()
            Case winType.DetalleRemedidaWindow
                Return _detalleRemedidaWindowFactory.CreateWindow()
            Case WindowType.ListadoRemedidasWindow
                Return _listadoRemedidasWindowFactory.CreateWindow()
                Throw New ArgumentException("Window type not supported.")
        End Select
    End Function

End Class
