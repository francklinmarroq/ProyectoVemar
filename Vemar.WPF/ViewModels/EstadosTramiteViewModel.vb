Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class EstadosTramiteViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of EstadoTramite)
    Private _items As New ObservableCollection(Of EstadoTramite)
    Private _guardarCommand As RelayCommand
    Private _estado As String = ""
    Private _editandoId As Integer? = Nothing

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public Property Items As ObservableCollection(Of EstadoTramite)
        Get
            Return _items
        End Get
        Set(v As ObservableCollection(Of EstadoTramite))
            _items = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Items)))
        End Set
    End Property

    Public Property Estado As String
        Get
            Return _estado
        End Get
        Set(v As String)
            _estado = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Estado)))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property ModificarCommand As ICommand
    Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of EstadoTramite))
        _service = service
        AgregarCommand = New RelayCommand(Sub(o)
                                             _editandoId = Nothing
                                             Estado = ""
                                             Dim win As New AgregarEstadoTramiteWindow()
                                             win.Title = "Nuevo Estado de Trámite"
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow
                                             win.ShowDialog()
                                         End Sub)

        ModificarCommand = New RelayCommand(Sub(o)
                                                Dim item = TryCast(o, EstadoTramite)
                                                If item Is Nothing Then Return
                                                _editandoId = item.Id
                                                Estado = item.Estado
                                                Dim win As New AgregarEstadoTramiteWindow()
                                                win.Title = "Modificar Estado de Trámite"
                                                win.DataContext = Me
                                                win.Owner = Application.Current.MainWindow
                                                win.ShowDialog()
                                            End Sub)

        EliminarCommand = New RelayCommand(AddressOf Eliminar)
        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) Not String.IsNullOrWhiteSpace(Estado))
        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Items = New ObservableCollection(Of EstadoTramite)(Await _service.GetAll())
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim item = TryCast(obj, EstadoTramite)
        If item Is Nothing Then Return
        Dim resultado = MessageBox.Show($"¿Está seguro que desea eliminar el estado ""{item.Estado}""?",
                                        "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning)
        If resultado = MessageBoxResult.Yes Then
            Try
                Await _service.Delete(item.Id)
                CargarItems()
            Catch ex As Exception
                ' Detectar error de restricción de clave foránea
                Dim inner As Exception = ex
                Do While inner.InnerException IsNot Nothing
                    inner = inner.InnerException
                Loop
                Dim msg = inner.Message.ToLower()
                If msg.Contains("foreign key") OrElse msg.Contains("constraint") OrElse
                   msg.Contains("reference") OrElse msg.Contains("fk_") Then
                    MessageBox.Show($"No se puede eliminar el estado ""{item.Estado}"" porque está siendo usado por uno o más trámites.",
                                    "No se puede eliminar", MessageBoxButton.OK, MessageBoxImage.Warning)
                Else
                    MessageBox.Show("Error al eliminar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                End If
            End Try
        End If
    End Sub

    Public Async Sub Guardar(obj As Object)
        Try
            If _editandoId.HasValue Then
                Await _service.Update(_editandoId.Value, New EstadoTramite With {.Id = _editandoId.Value, .Estado = Estado})
            Else
                Await _service.Add(New EstadoTramite With {.Estado = Estado})
            End If
            RaiseEvent GuardadoExitoso(Me, EventArgs.Empty)
            CargarItems()
        Catch ex As Exception
            MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class
