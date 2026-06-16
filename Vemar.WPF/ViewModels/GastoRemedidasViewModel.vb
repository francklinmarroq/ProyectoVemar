Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class GastoRemedidasViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of GastoRemedida)
    Private ReadOnly _remedidaService As IDataService(Of Remedida)
    Private _items As New ObservableCollection(Of GastoRemedida)
    Private _remedidas As New ObservableCollection(Of Remedida)
    Private _guardarCommand As RelayCommand
    Private _descripcion As String = ""
    Private _cantidad As String = ""
    Private _costoUnitario As String = ""
    Private _pendiente As Boolean
    Private _remedidaSel As Remedida

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public Property Items As ObservableCollection(Of GastoRemedida)
        Get
            Return _items
        End Get
        Set(v As ObservableCollection(Of GastoRemedida))
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

    Public Property Descripcion As String
        Get
            Return _descripcion
        End Get
        Set(v As String)
            _descripcion = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Descripcion)))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public Property Cantidad As String
        Get
            Return _cantidad
        End Get
        Set(v As String)
            _cantidad = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Cantidad)))
        End Set
    End Property

    Public Property CostoUnitario As String
        Get
            Return _costoUnitario
        End Get
        Set(v As String)
            _costoUnitario = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CostoUnitario)))
        End Set
    End Property

    Public Property PendienteDePago As Boolean
        Get
            Return _pendiente
        End Get
        Set(v As Boolean)
            _pendiente = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PendienteDePago)))
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

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of GastoRemedida), remedidaService As IDataService(Of Remedida))
        _service = service
        _remedidaService = remedidaService
        AgregarCommand = New RelayCommand(Sub(o)
                                             Descripcion = ""
                                             Cantidad = ""
                                             CostoUnitario = ""
                                             PendienteDePago = False
                                             RemedidaSeleccionada = Nothing
                                             Dim win As New AgregarGastoRemedidaWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow

                                             win.ShowDialog()
                                         End Sub)
        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) Not String.IsNullOrWhiteSpace(Descripcion) AndAlso RemedidaSeleccionada IsNot Nothing)
        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Items = New ObservableCollection(Of GastoRemedida)(Await _service.GetAll())
            Remedidas = New ObservableCollection(Of Remedida)(Await _remedidaService.GetAll())
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Guardar(obj As Object)
        Try
            Dim c As Decimal = 0, cu As Decimal = 0
            Decimal.TryParse(Cantidad, c)
            Decimal.TryParse(CostoUnitario, cu)
            Await _service.Add(New GastoRemedida With {
                .Descripcion = Descripcion,
                .Cantidad = c,
                .CostoUnitario = cu,
                .PendienteDePago = PendienteDePago,
                .Remedida = RemedidaSeleccionada
            })
            RaiseEvent GuardadoExitoso(Me, EventArgs.Empty)
            CargarItems()
        Catch ex As Exception
            MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class


