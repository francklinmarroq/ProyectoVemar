Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class CategoriaProyectosViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of CategoriaProyecto)
    Private _items As New ObservableCollection(Of CategoriaProyecto)
    Private _guardarCommand As RelayCommand
    Private _nombre As String = ""
    Private _cantidadLotes As String = ""

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public Property Items As ObservableCollection(Of CategoriaProyecto)
        Get
            Return _items
        End Get
        Set(v As ObservableCollection(Of CategoriaProyecto))
            _items = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Items)))
        End Set
    End Property

    Public Property Nombre As String
        Get
            Return _nombre
        End Get
        Set(v As String)
            _nombre = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Nombre)))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public Property CantidadLotes As String
        Get
            Return _cantidadLotes
        End Get
        Set(v As String)
            _cantidadLotes = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CantidadLotes)))
        End Set
    End Property

    Private _editando As CategoriaProyecto = Nothing

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property ModificarCommand As ICommand
    Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of CategoriaProyecto))
        _service = service
        AgregarCommand = New RelayCommand(Sub(o)
                                             _editando = Nothing
                                             Nombre = ""
                                             CantidadLotes = ""
                                             Dim win As New AgregarCategoriaProyectoWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow
                                             win.ShowDialog()
                                         End Sub)
        ModificarCommand = New RelayCommand(Sub(o)
                                               Dim item = TryCast(o, CategoriaProyecto)
                                               If item Is Nothing Then Return
                                               _editando = item
                                               Nombre = item.Nombre
                                               CantidadLotes = item.CantidadLotes.ToString()
                                               Dim win As New AgregarCategoriaProyectoWindow()
                                               win.DataContext = Me
                                               win.Owner = Application.Current.MainWindow
                                               win.ShowDialog()
                                           End Sub)
        EliminarCommand = New RelayCommand(AddressOf Eliminar)
        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) Not String.IsNullOrWhiteSpace(Nombre))
        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Items = New ObservableCollection(Of CategoriaProyecto)(Await _service.GetAll())
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim item = TryCast(obj, CategoriaProyecto)
        If item Is Nothing Then Return
        If MessageBox.Show($"¿Eliminar ""{item.Nombre}""?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) = MessageBoxResult.Yes Then
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
            Dim lotes As Integer = 0
            Integer.TryParse(CantidadLotes, lotes)
            Dim item As New CategoriaProyecto With {.Nombre = Nombre, .CantidadLotes = lotes}
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


