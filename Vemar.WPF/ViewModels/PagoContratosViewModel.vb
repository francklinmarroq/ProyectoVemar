Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class PagoContratosViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of PagoContrato)
    Private ReadOnly _contratoService As IDataService(Of Contrato)
    Private _items As New ObservableCollection(Of PagoContrato)
    Private _contratos As New ObservableCollection(Of Contrato)
    Private _guardarCommand As RelayCommand
    Private _valor As String = ""
    Private _descripcion As String = ""
    Private _contratoSel As Contrato

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public Property Items As ObservableCollection(Of PagoContrato)
        Get
            Return _items
        End Get
        Set(v As ObservableCollection(Of PagoContrato))
            _items = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Items)))
        End Set
    End Property

    Public Property Contratos As ObservableCollection(Of Contrato)
        Get
            Return _contratos
        End Get
        Set(v As ObservableCollection(Of Contrato))
            _contratos = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Contratos)))
        End Set
    End Property

    Public Property Valor As String
        Get
            Return _valor
        End Get
        Set(v As String)
            _valor = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Valor)))
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

    Public Property ContratoSeleccionado As Contrato
        Get
            Return _contratoSel
        End Get
        Set(v As Contrato)
            _contratoSel = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ContratoSeleccionado)))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of PagoContrato), contratoService As IDataService(Of Contrato))
        _service = service
        _contratoService = contratoService
        AgregarCommand = New RelayCommand(Sub(o)
                                             Valor = ""
                                             Descripcion = ""
                                             ContratoSeleccionado = Nothing
                                             Dim win As New AgregarPagoContratoWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow

                                             win.ShowDialog()
                                         End Sub)
        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) Not String.IsNullOrWhiteSpace(Valor) AndAlso ContratoSeleccionado IsNot Nothing)
        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Items = New ObservableCollection(Of PagoContrato)(Await _service.GetAll())
            Contratos = New ObservableCollection(Of Contrato)(Await _contratoService.GetAll())
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Guardar(obj As Object)
        Try
            Dim v As Decimal = 0
            Decimal.TryParse(Valor, v)
            Await _service.Add(New PagoContrato With {
                .Valor = v,
                .Descripcion = Descripcion,
                .Contrato = ContratoSeleccionado
            })
            RaiseEvent GuardadoExitoso(Me, EventArgs.Empty)
            CargarItems()
        Catch ex As Exception
            MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class


