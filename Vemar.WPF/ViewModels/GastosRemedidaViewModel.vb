Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Data
Imports Vemar.Domain

Public Class GastosRemedidaViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of GastoRemedida)
    Private ReadOnly _remedidaFija As Remedida
    Private _itemsSource As New ObservableCollection(Of GastoRemedida)
    Private _itemsView As ICollectionView
    Private _guardarCommand As RelayCommand

    Private _descripcion As String = ""
    Private _cantidad As String = ""
    Private _costoUnitario As String = ""
    Private _pendiente As Boolean = False

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public ReadOnly Property TituloRemedida As String
        Get
            Return $"Gastos — {_remedidaFija.Representante} ({_remedidaFija.ClaveSure})"
        End Get
    End Property

    Public ReadOnly Property ItemsView As ICollectionView
        Get
            Return _itemsView
        End Get
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

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of GastoRemedida), remedidaFija As Remedida)
        _service = service
        _remedidaFija = remedidaFija

        AgregarCommand = New RelayCommand(Sub(o)
                                             Descripcion = ""
                                             Cantidad = ""
                                             CostoUnitario = ""
                                             PendienteDePago = False
                                             Dim win As New AgregarGastoRemedidaRWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow

                                             win.ShowDialog()
                                         End Sub)

        EliminarCommand = New RelayCommand(AddressOf Eliminar)

        _guardarCommand = New RelayCommand(AddressOf Guardar,
            Function(o) Not String.IsNullOrWhiteSpace(Descripcion))

        _itemsView = CollectionViewSource.GetDefaultView(_itemsSource)
        _itemsView.Filter = Function(obj)
                                Dim g = TryCast(obj, GastoRemedida)
                                If g Is Nothing Then Return False
                                Return g.Remedida?.Id = _remedidaFija.Id
                            End Function
        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Dim lista = Await _service.GetAll()
            _itemsSource.Clear()
            For Each g In lista
                _itemsSource.Add(g)
            Next
            _itemsView.Refresh()
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ItemsView)))
        Catch ex As Exception
            MessageBox.Show("Error al cargar gastos: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim g = TryCast(obj, GastoRemedida)
        If g Is Nothing Then Return
        Dim resultado = MessageBox.Show($"¿Está seguro que desea eliminar el gasto ""{g.Descripcion}""?",
                                        "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning)
        If resultado = MessageBoxResult.Yes Then
            Try
                Await _service.Delete(g.Id)
                CargarItems()
            Catch ex As Exception
                MessageBox.Show("Error al eliminar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
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
                .Remedida = _remedidaFija
            })
            RaiseEvent GuardadoExitoso(Me, EventArgs.Empty)
            CargarItems()
        Catch ex As Exception
            MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class
