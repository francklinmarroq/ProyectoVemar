Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class TiposMovimientoViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of TipoMovimiento)
    Private _items As New ObservableCollection(Of TipoMovimiento)
    Private _guardarCommand As RelayCommand
    Private _tipo As String = ""

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public Property Items As ObservableCollection(Of TipoMovimiento)
        Get
            Return _items
        End Get
        Set(v As ObservableCollection(Of TipoMovimiento))
            _items = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Items)))
        End Set
    End Property

    Public Property Tipo As String
        Get
            Return _tipo
        End Get
        Set(v As String)
            _tipo = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Tipo)))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Private _editando As TipoMovimiento = Nothing

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property ModificarCommand As ICommand
    Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of TipoMovimiento))
        _service = service
        AgregarCommand = New RelayCommand(Sub(o)
                                             _editando = Nothing
                                             Tipo = ""
                                             Dim win As New AgregarTipoMovimientoWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow
                                             win.ShowDialog()
                                         End Sub)
        ModificarCommand = New RelayCommand(Sub(o)
                                               Dim item = TryCast(o, TipoMovimiento)
                                               If item Is Nothing Then Return
                                               _editando = item
                                               Tipo = item.Tipo
                                               Dim win As New AgregarTipoMovimientoWindow()
                                               win.DataContext = Me
                                               win.Owner = Application.Current.MainWindow
                                               win.ShowDialog()
                                           End Sub)
        EliminarCommand = New RelayCommand(AddressOf Eliminar)
        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) Not String.IsNullOrWhiteSpace(Tipo))
        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Items = New ObservableCollection(Of TipoMovimiento)(Await _service.GetAll())
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim item = TryCast(obj, TipoMovimiento)
        If item Is Nothing Then Return
        If MessageBox.Show($"¿Eliminar ""{item.Tipo}""?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) = MessageBoxResult.Yes Then
            Try
                Await _service.Delete(item.Id)
                CargarItems()
            Catch ex As Exception
                MessageBox.Show("Error al eliminar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
    End Sub

    Public Async Sub Guardar(obj As Object)
        Try
            Dim item As New TipoMovimiento With {.Tipo = Tipo}
            If _editando IsNot Nothing Then
                Await _service.Update(_editando.Id, item)
                _editando = Nothing
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


