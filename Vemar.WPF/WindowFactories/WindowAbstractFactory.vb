Public Class WindowAbstractFactory : Implements IWindowAbstractFactory
    Private ReadOnly _detalleClienteWindowFactory As IWindowFactory(Of DetalleClienteWindow)
    Private ReadOnly _listadoClientesWindowFactory As IWindowFactory(Of ListadoClientesWindow)
    Private ReadOnly _detalleRemedidaWindowFactory As IWindowFactory(Of DetalleRemedidaWindow)
    Private ReadOnly _listadoRemedidasWindowFactory As IWindowFactory(Of ListadoRemedidasWindow)
    Private ReadOnly _detalleProyectoWindowFactory As IWindowFactory(Of DetalleProyectoWindow)
    Private ReadOnly _listadoProyectosWindowFactory As IWindowFactory(Of ListadoProyectosWindow)
    Private ReadOnly _detalleColaboradorWindowFactory As IWindowFactory(Of DetalleColaboradorWindow)
    Private ReadOnly _listadoColaboradoresWindowFactory As IWindowFactory(Of ListadoColaboradoresWindow)
    Private ReadOnly _detalleContratistaWindowFactory As IWindowFactory(Of DetalleContratistaWindow)
    Private ReadOnly _listadoContratistasWindowFactory As IWindowFactory(Of ListadoContratistasWindow)
    Private ReadOnly _detalleTramiteWindowFactory As IWindowFactory(Of DetalleTramiteWindow)
    Private ReadOnly _listadoTramitesWindowFactory As IWindowFactory(Of ListadoTramitesWindow)

    Public Sub New(
        detalleClienteFactory As IWindowFactory(Of DetalleClienteWindow),
        listadoClientesWindowFactory As IWindowFactory(Of ListadoClientesWindow),
        detalleRemedidaWindowFactory As IWindowFactory(Of DetalleRemedidaWindow),
        listadoRemedidasWindowFactory As IWindowFactory(Of ListadoRemedidasWindow),
        detalleProyectoWindowFactory As IWindowFactory(Of DetalleProyectoWindow),
        listadoProyectosWindowFactory As IWindowFactory(Of ListadoProyectosWindow),
        detalleColaboradorWindowFactory As IWindowFactory(Of DetalleColaboradorWindow),
        listadoColaboradoresWindowFactory As IWindowFactory(Of ListadoColaboradoresWindow),
        detalleContratistaWindowFactory As IWindowFactory(Of DetalleContratistaWindow),
        listadoContratistasWindowFactory As IWindowFactory(Of ListadoContratistasWindow),
        detalleTramiteWindowFactory As IWindowFactory(Of DetalleTramiteWindow),
        listadoTramitesWindowFactory As IWindowFactory(Of ListadoTramitesWindow))

        _detalleClienteWindowFactory = detalleClienteFactory
        _listadoClientesWindowFactory = listadoClientesWindowFactory
        _detalleRemedidaWindowFactory = detalleRemedidaWindowFactory
        _listadoRemedidasWindowFactory = listadoRemedidasWindowFactory
        _detalleProyectoWindowFactory = detalleProyectoWindowFactory
        _listadoProyectosWindowFactory = listadoProyectosWindowFactory
        _detalleColaboradorWindowFactory = detalleColaboradorWindowFactory
        _listadoColaboradoresWindowFactory = listadoColaboradoresWindowFactory
        _detalleContratistaWindowFactory = detalleContratistaWindowFactory
        _listadoContratistasWindowFactory = listadoContratistasWindowFactory
        _detalleTramiteWindowFactory = detalleTramiteWindowFactory
        _listadoTramitesWindowFactory = listadoTramitesWindowFactory
    End Sub

    Public Function CreateWindow(winType As WindowType) As Window Implements IWindowAbstractFactory.CreateWindow
        Select Case winType
            Case WindowType.DetalleClienteWindow
                Return _detalleClienteWindowFactory.CreateWindow()
            Case WindowType.ListadoClientesWindow
                Return _listadoClientesWindowFactory.CreateWindow()
            Case WindowType.DetalleRemedidaWindow
                Return _detalleRemedidaWindowFactory.CreateWindow()
            Case WindowType.ListadoRemedidasWindow
                Return _listadoRemedidasWindowFactory.CreateWindow()
            Case WindowType.DetalleProyectoWindow
                Return _detalleProyectoWindowFactory.CreateWindow()
            Case WindowType.ListadoProyectosWindow
                Return _listadoProyectosWindowFactory.CreateWindow()
            Case WindowType.DetalleColaboradorWindow
                Return _detalleColaboradorWindowFactory.CreateWindow()
            Case WindowType.ListadoColaboradoresWindow
                Return _listadoColaboradoresWindowFactory.CreateWindow()
            Case WindowType.DetalleContratistaWindow
                Return _detalleContratistaWindowFactory.CreateWindow()
            Case WindowType.ListadoContratistasWindow
                Return _listadoContratistasWindowFactory.CreateWindow()
            Case WindowType.DetalleTramiteWindow
                Return _detalleTramiteWindowFactory.CreateWindow()
            Case WindowType.ListadoTramitesWindow
                Return _listadoTramitesWindowFactory.CreateWindow()
            Case Else
                Throw New ArgumentException("Window type not supported.")
        End Select
    End Function
End Class
