Imports System.ComponentModel
Imports Vemar.Domain

Public Class MainViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged

    Private ReadOnly _windowService As IWindowService
    Private ReadOnly _vmFactory As IViewModelAbstractFactory
    Private ReadOnly _dashboardVM As DashboardViewModel
    Private _currentView As ViewModelBase

    Public ReadOnly Property ExitApplicationCommand As ICommand

    ' Comandos del sidebar (un comando por módulo)
    Public ReadOnly Property NavegarDashboardCommand As ICommand
    Public ReadOnly Property NavegarClientesCommand As ICommand
    Public ReadOnly Property NavegarRemedidasCommand As ICommand
    Public ReadOnly Property NavegarProyectosCommand As ICommand
    Public ReadOnly Property NavegarColaboradoresCommand As ICommand
    Public ReadOnly Property NavegarContratistasCommand As ICommand
    Public ReadOnly Property NavegarTramitesCommand As ICommand
    Public ReadOnly Property NavegarAsignacionesCommand As ICommand
    Public ReadOnly Property NavegarAvancesCommand As ICommand
    Public ReadOnly Property NavegarContratosCommand As ICommand
    Public ReadOnly Property NavegarCobroRemedidasCommand As ICommand
    Public ReadOnly Property NavegarGastoRemedidasCommand As ICommand
    Public ReadOnly Property NavegarMovimientosCommand As ICommand
    Public ReadOnly Property NavegarPagoContratosCommand As ICommand
    Public ReadOnly Property NavegarZonificacionesCommand As ICommand
    Public ReadOnly Property NavegarCategoriaProyectosCommand As ICommand
    Public ReadOnly Property NavegarTiposTramiteCommand As ICommand
    Public ReadOnly Property NavegarEstadosTramiteCommand As ICommand
    Public ReadOnly Property NavegarTiposMovimientoCommand As ICommand
    Public ReadOnly Property NavegarUsuariosCommand As ICommand
    Public ReadOnly Property NavegarCajaChicaCommand As ICommand

    Public Property CurrentView As ViewModelBase
        Get
            Return _currentView
        End Get
        Set(value As ViewModelBase)
            _currentView = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CurrentView)))
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub New(windowService As IWindowService, vmFactory As IViewModelAbstractFactory, dashboardVM As DashboardViewModel)
        _windowService = windowService
        _vmFactory = vmFactory
        _dashboardVM = dashboardVM

        ExitApplicationCommand = New RelayCommand(Sub(o) Application.Current.Shutdown())

        NavegarDashboardCommand = New RelayCommand(Sub(o)
                                                        CurrentView = _dashboardVM
                                                        _dashboardVM.CargarEstadisticas()
                                                    End Sub)
        NavegarClientesCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.ClientesViewModel))
        NavegarRemedidasCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.RemedidasViewModel))
        NavegarProyectosCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.ProyectosViewModel))
        NavegarColaboradoresCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.ColaboradoresViewModel))
        NavegarContratistasCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.ContratistasViewModel))
        NavegarTramitesCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.TramitesViewModel))
        NavegarAsignacionesCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.AsignacionesViewModel))
        NavegarAvancesCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.AvancesViewModel))
        NavegarContratosCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.ContratosViewModel))
        NavegarCobroRemedidasCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.CobroRemedidasViewModel))
        NavegarGastoRemedidasCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.GastoRemedidasViewModel))
        NavegarMovimientosCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.MovimientosViewModel))
        NavegarPagoContratosCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.PagoContratosViewModel))
        NavegarZonificacionesCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.ZonificacionesViewModel))
        NavegarCategoriaProyectosCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.CategoriaProyectosViewModel))
        NavegarTiposTramiteCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.TiposTramiteViewModel))
        NavegarEstadosTramiteCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.EstadosTramiteViewModel))
        NavegarTiposMovimientoCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.TiposMovimientoViewModel))
        NavegarUsuariosCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.UsuariosViewModel))
        NavegarCajaChicaCommand = New RelayCommand(Sub(o) CurrentView = _vmFactory.CreateViewModel(ViewModelType.CajaChicaViewModel))

        CurrentView = _dashboardVM
    End Sub
End Class

