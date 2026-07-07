Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class MovimientosRemedidaViewModel : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of Movimiento)
    Private ReadOnly _tipoService As IDataService(Of TipoMovimiento)
    Private ReadOnly _remedidaFija As Remedida
    Private _items As New ObservableCollection(Of Movimiento)
    Private _tipos As New ObservableCollection(Of TipoMovimiento)
    Private _guardarCommand As RelayCommand
    Private _tipoSel As TipoMovimiento
    Private _fecha As DateTime = DateTime.Today
    Private _descripcion As String = ""

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public ReadOnly Property TituloRemedida As String
        Get
            Dim nombre = If(Not String.IsNullOrWhiteSpace(_remedidaFija.Propietario),
                            _remedidaFija.Propietario,
                            If(Not String.IsNullOrWhiteSpace(_remedidaFija.Representante),
                               _remedidaFija.Representante, "—"))
            Return $"Movimientos del Expediente — {nombre} / {_remedidaFija.ClaveSure}"
        End Get
    End Property

    Public Property Items As ObservableCollection(Of Movimiento)
        Get
            Return _items
        End Get
        Set(v As ObservableCollection(Of Movimiento))
            _items = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Items)))
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

    Public Property Fecha As DateTime
        Get
            Return _fecha
        End Get
        Set(v As DateTime)
            _fecha = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Fecha)))
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

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of Movimiento),
                   tipoService As IDataService(Of TipoMovimiento),
                   remedidaFija As Remedida)
        _service = service
        _tipoService = tipoService
        _remedidaFija = remedidaFija

        AgregarCommand = New RelayCommand(Sub(o)
                                             Descripcion = ""
                                             Fecha = DateTime.Today
                                             TipoSeleccionado = Tipos.FirstOrDefault()
                                             Dim win As New AgregarMovimientoRemedidaWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow
                                             win.ShowDialog()
                                         End Sub)

        EliminarCommand = New RelayCommand(AddressOf Eliminar)

        _guardarCommand = New RelayCommand(AddressOf Guardar,
            Function(o) TipoSeleccionado IsNot Nothing)

        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Dim tiposLista = Await _tipoService.GetAll()
            Tipos = New ObservableCollection(Of TipoMovimiento)(tiposLista)

            Dim todos = Await _service.GetAll()
            Dim filtrados = todos.Where(Function(m) m.Remedida?.Id = _remedidaFija.Id).OrderByDescending(Function(m) m.Fecha)
            Items = New ObservableCollection(Of Movimiento)(filtrados)
        Catch ex As Exception
            MessageBox.Show("Error al cargar movimientos: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim m = TryCast(obj, Movimiento)
        If m Is Nothing Then Return
        Dim tipo = If(m.TipoMovimiento?.Tipo, "movimiento")
        Dim resultado = MessageBox.Show($"¿Eliminar el registro ""{tipo}""?",
                                        "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning)
        If resultado = MessageBoxResult.Yes Then
            Try
                Await _service.Delete(m.Id)
                CargarItems()
            Catch ex As Exception
                MessageBox.Show("Error al eliminar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
    End Sub

    Public Async Sub Guardar(obj As Object)
        Try
            Await _service.Add(New Movimiento With {
                .TipoMovimiento = TipoSeleccionado,
                .Fecha = Fecha,
                .Descripcion = Descripcion,
                .Remedida = _remedidaFija
            })
            RaiseEvent GuardadoExitoso(Me, EventArgs.Empty)
            CargarItems()
        Catch ex As Exception
            MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class
