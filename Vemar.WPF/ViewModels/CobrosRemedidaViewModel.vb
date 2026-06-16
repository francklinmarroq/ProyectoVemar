Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class CobrosRemedidaViewModel : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of CobroRemedida)
    Private ReadOnly _remedidaFija As Remedida
    Private _items As New ObservableCollection(Of CobroRemedida)
    Private _guardarCommand As RelayCommand
    Private _cantidad As String = ""

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public ReadOnly Property TituloRemedida As String
        Get
            Return $"Cobros de Remedida — {_remedidaFija.Representante} / {_remedidaFija.ClaveSure}"
        End Get
    End Property

    Public Property Items As ObservableCollection(Of CobroRemedida)
        Get
            Return _items
        End Get
        Set(v As ObservableCollection(Of CobroRemedida))
            _items = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Items)))
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

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of CobroRemedida), remedidaFija As Remedida)
        _service = service
        _remedidaFija = remedidaFija

        AgregarCommand = New RelayCommand(Sub(o)
                                             Cantidad = ""
                                             Dim win As New AgregarCobroRemedidaWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow

                                             win.ShowDialog()
                                         End Sub)

        EliminarCommand = New RelayCommand(AddressOf Eliminar)

        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) Not String.IsNullOrWhiteSpace(Cantidad))

        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Dim todos = Await _service.GetAll()
            Dim filtrados = todos.Where(Function(c) c.Remedida?.Id = _remedidaFija.Id)
            Items = New ObservableCollection(Of CobroRemedida)(filtrados)
        Catch ex As Exception
            MessageBox.Show("Error al cargar cobros: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim c = TryCast(obj, CobroRemedida)
        If c Is Nothing Then Return
        Dim resultado = MessageBox.Show($"¿Eliminar el cobro de L {c.Cantidad:N2}?",
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
            Dim c As Decimal = 0
            Decimal.TryParse(Cantidad, c)
            Await _service.Add(New CobroRemedida With {
                .Cantidad = c,
                .Remedida = _remedidaFija
            })
            RaiseEvent GuardadoExitoso(Me, EventArgs.Empty)
            CargarItems()
        Catch ex As Exception
            MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class
