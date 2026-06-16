Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain
Imports Vemar.WPF.Reports

Public Class ContratistasViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of Contratista)
    Private _items As New ObservableCollection(Of Contratista)
    Private _guardarCommand As RelayCommand
    Private _nombre As String = ""
    Private _telefono As String = ""

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public Property Items As ObservableCollection(Of Contratista)
        Get
            Return _items
        End Get
        Set(value As ObservableCollection(Of Contratista))
            _items = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Items)))
        End Set
    End Property

    Public Property Nombre As String
        Get
            Return _nombre
        End Get
        Set(value As String)
            _nombre = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Nombre)))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public Property Telefono As String
        Get
            Return _telefono
        End Get
        Set(value As String)
            _telefono = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Telefono)))
        End Set
    End Property

    Private _editando As Contratista = Nothing

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property ModificarCommand As ICommand
    Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property ExportarExcelCommand As ICommand
    Public ReadOnly Property ExportarPdfCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of Contratista))
        _service = service
        AgregarCommand = New RelayCommand(Sub(o)
                                             _editando = Nothing
                                             Nombre = ""
                                             Telefono = ""
                                             Dim win As New AgregarContratistaWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow
                                             win.ShowDialog()
                                         End Sub)
        ModificarCommand = New RelayCommand(Sub(o)
                                               Dim item = TryCast(o, Contratista)
                                               If item Is Nothing Then Return
                                               _editando = item
                                               Nombre = item.Nombre
                                               Telefono = item.Telefono
                                               Dim win As New AgregarContratistaWindow()
                                               win.DataContext = Me
                                               win.Owner = Application.Current.MainWindow
                                               win.ShowDialog()
                                           End Sub)
        EliminarCommand = New RelayCommand(AddressOf Eliminar)

        Dim _report As New Vemar.WPF.Reports.ContratistasReport()
        ExportarExcelCommand = New RelayCommand(Sub(o)
                                                   _report.GenerateExcelAsync(_items.ToList(), "Contratistas")
                                               End Sub)
        ExportarPdfCommand = New RelayCommand(Sub(o)
                                                  _report.GeneratePdfAsync(_items.ToList(), "Contratistas")
                                              End Sub)

        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) Not String.IsNullOrWhiteSpace(Nombre))
        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Items = New ObservableCollection(Of Contratista)(Await _service.GetAll())
        Catch ex As Exception
            MessageBox.Show("Error al cargar contratistas: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim item = TryCast(obj, Contratista)
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
            Dim item As New Contratista With {.Nombre = Nombre, .Telefono = Telefono}
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
