Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Data
Imports Vemar.Domain

Public Class ContratosProyectoViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of Contrato)
    Private ReadOnly _contratistaService As IDataService(Of Contratista)
    Private ReadOnly _pagoService As IDataService(Of PagoContrato)
    Private ReadOnly _proyectoFijo As Proyecto
    Private _itemsSource As New ObservableCollection(Of Contrato)
    Private _itemsView As ICollectionView
    Private _contratistas As New ObservableCollection(Of Contratista)
    Private _guardarCommand As RelayCommand
    Private _contratoEditando As Contrato = Nothing

    Private _tituloFormulario As String = "Nuevo Contrato"
    Private _valor As String = ""
    Private _descripcion As String = ""
    Private _contratistaSel As Contratista

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public ReadOnly Property TituloProyecto As String
        Get
            Return $"Contratos — {_proyectoFijo.Nombre}"
        End Get
    End Property

    Public ReadOnly Property ItemsView As ICollectionView
        Get
            Return _itemsView
        End Get
    End Property

    Public Property TituloFormulario As String
        Get
            Return _tituloFormulario
        End Get
        Set(v As String)
            _tituloFormulario = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TituloFormulario)))
        End Set
    End Property

    Public Property Contratistas As ObservableCollection(Of Contratista)
        Get
            Return _contratistas
        End Get
        Set(v As ObservableCollection(Of Contratista))
            _contratistas = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Contratistas)))
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

    Public Property ContratistaSeleccionado As Contratista
        Get
            Return _contratistaSel
        End Get
        Set(v As Contratista)
            _contratistaSel = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ContratistaSeleccionado)))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property ModificarCommand As ICommand
    Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property VerPagosCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of Contrato),
                   contratistaService As IDataService(Of Contratista),
                   pagoService As IDataService(Of PagoContrato),
                   proyectoFijo As Proyecto)
        _service = service
        _contratistaService = contratistaService
        _pagoService = pagoService
        _proyectoFijo = proyectoFijo

        AgregarCommand = New RelayCommand(Sub(o)
                                             _contratoEditando = Nothing
                                             TituloFormulario = "Nuevo Contrato"
                                             Valor = ""
                                             Descripcion = ""
                                             ContratistaSeleccionado = Nothing
                                             Dim win As New AgregarContratoProyectoWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow

                                             win.ShowDialog()
                                         End Sub)

        ModificarCommand = New RelayCommand(Sub(o)
                                               Dim c = TryCast(o, Contrato)
                                               If c Is Nothing Then Return
                                               _contratoEditando = c
                                               TituloFormulario = "Modificar Contrato"
                                               Valor = c.Valor.ToString()
                                               Descripcion = If(c.Descripcion, "")
                                               ContratistaSeleccionado = Contratistas.FirstOrDefault(Function(x) x.Id = c.Contratista?.Id)
                                               Dim win As New AgregarContratoProyectoWindow()
                                               win.DataContext = Me
                                               win.Owner = Application.Current.MainWindow

                                               win.ShowDialog()
                                           End Sub)

        EliminarCommand = New RelayCommand(AddressOf Eliminar)

        VerPagosCommand = New RelayCommand(Sub(o)
                                               Dim c = TryCast(o, Contrato)
                                               If c Is Nothing Then Return
                                               Dim vm As New PagosContratoViewModel(_pagoService, c)
                                               Dim win As New PagosContratoWindow()
                                               win.DataContext = vm
                                               AddHandler vm.GuardadoExitoso, Sub(s, e)
                                                                                   Dim agregarWin = Application.Current.Windows.OfType(Of AgregarPagoContratoWindow)().FirstOrDefault()
                                                                                   agregarWin?.Close()
                                                                               End Sub
                                               win.Owner = Application.Current.MainWindow

                                               win.ShowDialog()
                                           End Sub)

        _guardarCommand = New RelayCommand(AddressOf Guardar,
            Function(o) Not String.IsNullOrWhiteSpace(Valor) AndAlso ContratistaSeleccionado IsNot Nothing)

        _itemsView = CollectionViewSource.GetDefaultView(_itemsSource)
        _itemsView.Filter = Function(obj)
                                Dim c = TryCast(obj, Contrato)
                                If c Is Nothing Then Return False
                                Return c.Proyecto?.Id = _proyectoFijo.Id
                            End Function
        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Dim lista = Await _service.GetAll()
            _itemsSource.Clear()
            For Each c In lista
                _itemsSource.Add(c)
            Next
            _itemsView.Refresh()
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ItemsView)))
            Contratistas = New ObservableCollection(Of Contratista)(Await _contratistaService.GetAll())
        Catch ex As Exception
            MessageBox.Show("Error al cargar contratos: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim c = TryCast(obj, Contrato)
        If c Is Nothing Then Return
        Dim resultado = MessageBox.Show($"¿Está seguro que desea eliminar el contrato de ""{c.Contratista?.Nombre}""?",
                                        "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning)
        If resultado = MessageBoxResult.Yes Then
            Try
                Await _service.Delete(c.Id)
                CargarItems()
            Catch ex As Exception
                MessageBox.Show("Error al eliminar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
    End Sub

    Public Async Sub Guardar(obj As Object)
        Try
            Dim v As Decimal = 0
            Decimal.TryParse(Valor, v)
            Dim item As New Contrato With {
                .Valor = v,
                .Descripcion = Descripcion,
                .Contratista = ContratistaSeleccionado,
                .Proyecto = _proyectoFijo
            }
            If _contratoEditando IsNot Nothing Then
                Await _service.Update(_contratoEditando.Id, item)
            Else
                Await _service.Add(item)
            End If
            RaiseEvent GuardadoExitoso(Me, EventArgs.Empty)
            CargarItems()
        Catch ex As Exception
            MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class
