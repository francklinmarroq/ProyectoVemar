Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain
Imports Vemar.WPF.Reports

Public Class MovimientosViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of Movimiento)
    Private ReadOnly _remedidaService As IDataService(Of Remedida)
    Private ReadOnly _tipoService As IDataService(Of TipoMovimiento)
    Private _items As New ObservableCollection(Of Movimiento)
    Private _remedidas As New ObservableCollection(Of Remedida)
    Private _tipos As New ObservableCollection(Of TipoMovimiento)
    Private _guardarCommand As RelayCommand
    Private _fecha As DateTime = DateTime.Today
    Private _remedidaSel As Remedida
    Private _tipoSel As TipoMovimiento
    Private _descripcion As String = ""

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public Property Items As ObservableCollection(Of Movimiento)
        Get
            Return _items
        End Get
        Set(v As ObservableCollection(Of Movimiento))
            _items = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Items)))
        End Set
    End Property

    Public Property Remedidas As ObservableCollection(Of Remedida)
        Get
            Return _remedidas
        End Get
        Set(v As ObservableCollection(Of Remedida))
            _remedidas = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Remedidas)))
        End Set
    End Property

    Public Property Tipos As ObservableCollection(Of TipoMovimiento)
        Get
            Return _tipos
        End Get
        Set(v As ObservableCollection(Of TipoMovimiento))
            _tipos = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Tipos)))
        End Set
    End Property

    Public Property Fecha As DateTime
        Get
            Return _fecha
        End Get
        Set(v As DateTime)
            _fecha = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Fecha)))
        End Set
    End Property

    Public Property RemedidaSeleccionada As Remedida
        Get
            Return _remedidaSel
        End Get
        Set(v As Remedida)
            _remedidaSel = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RemedidaSeleccionada)))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public Property TipoSeleccionado As TipoMovimiento
        Get
            Return _tipoSel
        End Get
        Set(v As TipoMovimiento)
            _tipoSel = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TipoSeleccionado)))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public Property Descripcion As String
        Get
            Return _descripcion
        End Get
        Set(v As String)
            _descripcion = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Descripcion)))
        End Set
    End Property

    Public ReadOnly Property ExportarExcelCommand As ICommand
    Public ReadOnly Property ExportarPdfCommand As ICommand
    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of Movimiento), remedidaService As IDataService(Of Remedida), tipoService As IDataService(Of TipoMovimiento))
        _service = service
        _remedidaService = remedidaService
        _tipoService = tipoService
        AgregarCommand = New RelayCommand(Sub(o)
                                             RemedidaSeleccionada = Nothing
                                             TipoSeleccionado = Nothing
                                             Fecha = DateTime.Today
                                             Descripcion = ""
                                             Dim win As New AgregarMovimientoWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow

                                             win.ShowDialog()
                                         End Sub)

        Dim _reportMov As New Vemar.WPF.Reports.MovimientosReport()
        ExportarExcelCommand = New RelayCommand(Sub(o)
                                                   _reportMov.GenerateExcelAsync(_items.ToList(), "Movimientos")
                                               End Sub)
        ExportarPdfCommand = New RelayCommand(Sub(o)
                                                  _reportMov.GeneratePdfAsync(_items.ToList(), "Movimientos")
                                              End Sub)

        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) RemedidaSeleccionada IsNot Nothing AndAlso TipoSeleccionado IsNot Nothing)
        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Items = New ObservableCollection(Of Movimiento)(Await _service.GetAll())
            Remedidas = New ObservableCollection(Of Remedida)(Await _remedidaService.GetAll())
            Tipos = New ObservableCollection(Of TipoMovimiento)(Await _tipoService.GetAll())
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Guardar(obj As Object)
        Try
            Await _service.Add(New Movimiento With {
                .Remedida = RemedidaSeleccionada,
                .TipoMovimiento = TipoSeleccionado,
                .Fecha = Fecha,
                .Descripcion = Descripcion
            })
            RaiseEvent GuardadoExitoso(Me, EventArgs.Empty)
            CargarItems()
        Catch ex As Exception
            MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class


