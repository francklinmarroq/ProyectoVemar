Imports Microsoft.Extensions.DependencyInjection
Imports Vemar.Domain
Imports Vemar.EF
Imports Vemar.EF.Services

Module DIContenedor
    Private _serviceProvider As IServiceProvider
    Public ReadOnly Property Services As IServiceProvider
        Get
            If _serviceProvider Is Nothing Then
                Throw New InvalidOperationException("Service provider is not initialized. Please call Initialize first.")
            End If
            Return _serviceProvider
        End Get
    End Property

    Public Sub ConfigureServices()
        Dim services As IServiceCollection = New ServiceCollection()

        ' Window Service
        services.AddSingleton(Of IWindowService, WindowService)()
        services.AddSingleton(Of IWindowAbstractFactory, WindowAbstractFactory)()

        ' Window Factories (para ventanas flotantes del menú superior)
        services.AddSingleton(Of IWindowFactory(Of DetalleClienteWindow), DetalleClienteWindowFactory)
        services.AddSingleton(Of IWindowFactory(Of ListadoClientesWindow), ListadoClientesWindowFactory)()
        services.AddSingleton(Of IWindowFactory(Of DetalleRemedidaWindow), DetalleRemedidaWindowFactory)()
        services.AddSingleton(Of IWindowFactory(Of ListadoRemedidasWindow), ListadoRemedidasWindowFactory)()
        services.AddSingleton(Of IWindowFactory(Of DetalleProyectoWindow), DetalleProyectoWindowFactory)()
        services.AddSingleton(Of IWindowFactory(Of ListadoProyectosWindow), ListadoProyectosWindowFactory)()
        services.AddSingleton(Of IWindowFactory(Of DetalleColaboradorWindow), DetalleColaboradorWindowFactory)()
        services.AddSingleton(Of IWindowFactory(Of ListadoColaboradoresWindow), ListadoColaboradoresWindowFactory)()
        services.AddSingleton(Of IWindowFactory(Of DetalleContratistaWindow), DetalleContratistaWindowFactory)()
        services.AddSingleton(Of IWindowFactory(Of ListadoContratistasWindow), ListadoContratistasWindowFactory)()
        services.AddSingleton(Of IWindowFactory(Of DetalleTramiteWindow), DetalleTramiteWindowFactory)()
        services.AddSingleton(Of IWindowFactory(Of ListadoTramitesWindow), ListadoTramitesWindowFactory)()

        ' ViewModel Abstract Factory (usa IServiceProvider)
        services.AddSingleton(Of IViewModelAbstractFactory, ViewModelAbstractFactory)()

        ' ViewModels combinados del panel lateral (Transient = nueva instancia cada navegación)
        services.AddTransient(Of ClientesViewModel)()
        services.AddTransient(Of RemedidasViewModel)()
        services.AddTransient(Of ProyectosViewModel)()
        services.AddTransient(Of ColaboradoresViewModel)()
        services.AddTransient(Of ContratistasViewModel)()
        services.AddTransient(Of TramitesViewModel)()
        services.AddTransient(Of AsignacionesViewModel)()
        services.AddTransient(Of AvancesViewModel)()
        services.AddTransient(Of ContratosViewModel)()
        services.AddTransient(Of CobroRemedidasViewModel)()
        services.AddTransient(Of GastoRemedidasViewModel)()
        services.AddTransient(Of MovimientosViewModel)()
        services.AddTransient(Of PagoContratosViewModel)()
        services.AddTransient(Of ZonificacionesViewModel)()
        services.AddTransient(Of CategoriaProyectosViewModel)()
        services.AddTransient(Of TiposTramiteViewModel)()
        services.AddTransient(Of EstadosTramiteViewModel)()
        services.AddTransient(Of TiposMovimientoViewModel)()
        services.AddTransient(Of UsuariosViewModel)()
        services.AddTransient(Of CajaChicaViewModel)()

        ' ViewModels para ventanas flotantes (legacy)
        services.AddTransient(Of DetalleClienteViewModel)()
        services.AddTransient(Of ListadoClientesViewModel)()
        services.AddTransient(Of DetalleRemedidaViewModel)()
        services.AddTransient(Of ListadoRemedidasViewModel)()
        services.AddTransient(Of DetalleProyectoViewModel)()
        services.AddTransient(Of ListadoProyectosViewModel)()
        services.AddTransient(Of DetalleColaboradorViewModel)()
        services.AddTransient(Of ListadoColaboradoresViewModel)()
        services.AddTransient(Of DetalleContratistaViewModel)()
        services.AddTransient(Of ListadoContratistasViewModel)()
        services.AddTransient(Of DetalleTramiteViewModel)()
        services.AddTransient(Of ListadoTramitesViewModel)()

        ' DbContext
        services.AddSingleton(Of VemarDbContextFactory)()

        ' Data Services
        services.AddSingleton(Of IDataService(Of Cliente), ClienteDataService)()
        services.AddSingleton(Of IDataService(Of Remedida), GenericDataService(Of Remedida))()
        services.AddSingleton(Of IDataService(Of Proyecto), ProyectoDataService)()
        services.AddSingleton(Of IDataService(Of Colaborador), GenericDataService(Of Colaborador))()
        services.AddSingleton(Of IDataService(Of Contratista), GenericDataService(Of Contratista))()
        services.AddSingleton(Of IDataService(Of Tramite), TramiteDataService)()
        services.AddSingleton(Of IDataService(Of Asignacion), AsignacionDataService)()
        services.AddSingleton(Of IDataService(Of Avance), AvanceDataService)()
        services.AddSingleton(Of IDataService(Of Contrato), ContratoDataService)()
        services.AddSingleton(Of IDataService(Of CobroRemedida), CobroRemedidaDataService)()
        services.AddSingleton(Of IDataService(Of GastoProyecto), GastoProyectoDataService)()
        services.AddSingleton(Of IDataService(Of GastoRemedida), GastoRemedidaDataService)()
        services.AddSingleton(Of IDataService(Of Movimiento), MovimientoDataService)()
        services.AddSingleton(Of IDataService(Of PagoContrato), PagoContratoDataService)()
        services.AddSingleton(Of IDataService(Of Zonificacion), GenericDataService(Of Zonificacion))()
        services.AddSingleton(Of IDataService(Of CategoriaProyecto), GenericDataService(Of CategoriaProyecto))()
        services.AddSingleton(Of IDataService(Of TipoTramite), GenericDataService(Of TipoTramite))()
        services.AddSingleton(Of IDataService(Of EstadoTramite), GenericDataService(Of EstadoTramite))()
        services.AddSingleton(Of IDataService(Of TipoMovimiento), GenericDataService(Of TipoMovimiento))()
        services.AddSingleton(Of IDataService(Of Usuario), GenericDataService(Of Usuario))()
        services.AddSingleton(Of IDataService(Of CajaChica), CajaChicaDataService)()
        services.AddSingleton(Of IDataService(Of CobroProyecto), CobroProyectoDataService)()

        ' Dashboard ViewModel
        services.AddSingleton(Of DashboardViewModel)()

        ' Main
        services.AddSingleton(Of MainViewModel)()
        services.AddSingleton(Of MainWindow)(Function(s) New MainWindow(s.GetRequiredService(Of MainViewModel)()))

        _serviceProvider = services.BuildServiceProvider()
    End Sub

End Module
