Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class CobroRemedidasViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of CobroRemedida)
    Private ReadOnly _remedidaService As IDataService(Of Remedida)
    Private _items As New ObservableCollection(Of CobroRemedida)
    Private _remedidas As New ObservableCollection(Of Remedida)
    Private _guardarCommand As RelayCommand
    Private _cantidad As String = ""
    Private _remedidaSel As Remedida

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public Property Items As ObservableCollection(Of CobroRemedida)
        Get
            Return _items
        End Get
        Set(v As ObservableCollection(Of CobroRemedida))
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

    Public Property Cantidad As String
        Get
            Return _cantidad
        End Get
        Set(v As String)
            _cantidad = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Cantidad)))
            _guardarCommand?.RaiseCanExecuteChanged()
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

    Public Sub New(service As IDataService(Of CobroRemedida), remedidaService As IDataService(Of Remedida))
        _service = service
        _remedidaService = remedidaService
        AgregarCommand = New RelayCommand(Sub(o)
                                             Cantidad = ""
                                             RemedidaSeleccionada = Nothing
                                             Dim win As New AgregarCobroRemedidaWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow

                                             win.ShowDialog()
                                         End Sub)
        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) Not String.IsNullOrWhiteSpace(Cantidad) AndAlso RemedidaSeleccionada IsNot Nothing)
        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Items = New ObservableCollection(Of CobroRemedida)(Await _service.GetAll())
            Remedidas = New ObservableCollection(Of Remedida)(Await _remedidaService.GetAll())
        Catch ex As Exception
            MessageBox.Show("Error al cargar cobros: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Guardar(obj As Object)
        Try
            Dim c As Decimal = 0
            Decimal.TryParse(Cantidad, c)
            Await _service.Add(New CobroRemedida With {.Cantidad = c, .Remedida = RemedidaSeleccionada})
            RaiseEvent GuardadoExitoso(Me, EventArgs.Empty)
            CargarItems()
        Catch ex As Exception
            MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class


