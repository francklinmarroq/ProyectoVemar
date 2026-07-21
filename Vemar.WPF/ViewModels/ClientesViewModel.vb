Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Data
Imports Vemar.Domain
Imports Vemar.WPF.Reports

Public Class ClientesViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged

    Private ReadOnly _service As IDataService(Of Cliente)
    Private _itemsSource As New ObservableCollection(Of Cliente)
    Private _itemsView As ICollectionView
    Private _busqueda As String = ""
    Private _guardarCommand As RelayCommand
    Private _clienteEditando As Cliente = Nothing

    ' Campos del formulario
    Private _nombre As String = ""
    Private _rtn As String = ""
    Private _telefono As String = ""
    Private _direccion As String = ""
    Private _email As String = ""
    Private _dniPropietario As String = ""
    Private _representante As String = ""
    Private _dniRepresentante As String = ""
    Private _rtnRepresentante As String = ""
    Private _telefonoRepresentante As String = ""
    Private _emailRepresentante As String = ""
    Private _tituloFormulario As String = "Nuevo Cliente"

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public ReadOnly Property ItemsView As ICollectionView
        Get
            Return _itemsView
        End Get
    End Property

    Public Property Busqueda As String
        Get
            Return _busqueda
        End Get
        Set(value As String)
            _busqueda = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Busqueda)))
            _itemsView?.Refresh()
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

    Public Property Rtn As String
        Get
            Return _rtn
        End Get
        Set(value As String)
            _rtn = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Rtn)))
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

    Public Property Direccion As String
        Get
            Return _direccion
        End Get
        Set(value As String)
            _direccion = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Direccion)))
        End Set
    End Property

    Public Property EmailCorporativo As String
        Get
            Return _email
        End Get
        Set(value As String)
            _email = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(EmailCorporativo)))
        End Set
    End Property

    Public Property DniPropietario As String
        Get
            Return _dniPropietario
        End Get
        Set(value As String)
            _dniPropietario = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DniPropietario)))
        End Set
    End Property

    Public Property Representante As String
        Get
            Return _representante
        End Get
        Set(value As String)
            _representante = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Representante)))
        End Set
    End Property

    Public Property RtnRepresentante As String
        Get
            Return _rtnRepresentante
        End Get
        Set(value As String)
            _rtnRepresentante = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RtnRepresentante)))
        End Set
    End Property

    Public Property TelefonoRepresentante As String
        Get
            Return _telefonoRepresentante
        End Get
        Set(value As String)
            _telefonoRepresentante = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TelefonoRepresentante)))
        End Set
    End Property

    Public Property DniRepresentante As String
        Get
            Return _dniRepresentante
        End Get
        Set(value As String)
            _dniRepresentante = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DniRepresentante)))
        End Set
    End Property

    Public Property EmailRepresentante As String
        Get
            Return _emailRepresentante
        End Get
        Set(value As String)
            _emailRepresentante = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(EmailRepresentante)))
        End Set
    End Property

    Public Property TituloFormulario As String
        Get
            Return _tituloFormulario
        End Get
        Set(value As String)
            _tituloFormulario = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TituloFormulario)))
        End Set
    End Property

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property ModificarCommand As ICommand
    Public ReadOnly Property ExportarExcelCommand As ICommand
    Public ReadOnly Property ExportarPdfCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of Cliente))
        _service = service
        AgregarCommand = New RelayCommand(Sub(o)
                                             _clienteEditando = Nothing
                                             TituloFormulario = "Nuevo Cliente"
                                             LimpiarFormulario()
                                             Dim win As New AgregarClienteWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow

                                             win.ShowDialog()
                                         End Sub)
        ModificarCommand = New RelayCommand(Sub(o)
                                               Dim cliente = TryCast(o, Cliente)
                                               If cliente Is Nothing Then Return
                                               _clienteEditando = cliente
                                               TituloFormulario = "Modificar Cliente"
                                               Nombre = cliente.Nombre
                                               Rtn = cliente.Rtn
                                               Telefono = cliente.Telefono
                                               Direccion = cliente.Direccion
                                               EmailCorporativo = cliente.EmailCorporativo
                                               DniPropietario = cliente.DniPropietario
                                               Representante = cliente.Representante
                                               DniRepresentante = cliente.DniRepresentante
                                               RtnRepresentante = cliente.RtnRepresentante
                                               TelefonoRepresentante = cliente.TelefonoRepresentante
                                               EmailRepresentante = cliente.EmailRepresentante
                                               Dim win As New AgregarClienteWindow()
                                               win.DataContext = Me
                                               win.Owner = Application.Current.MainWindow

                                               win.ShowDialog()
                                           End Sub)
        EliminarCommand = New RelayCommand(AddressOf Eliminar)

        Dim _report As New Vemar.WPF.Reports.ClientesReport()
        ExportarExcelCommand = New RelayCommand(Sub(o)
                                                   _report.GenerateExcelAsync(_itemsSource.ToList(), "Clientes")
                                               End Sub)
        ExportarPdfCommand = New RelayCommand(Sub(o)
                                                  _report.GeneratePdfAsync(_itemsSource.ToList(), "Clientes")
                                              End Sub)

        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) Not String.IsNullOrWhiteSpace(Nombre))
        _itemsView = CollectionViewSource.GetDefaultView(_itemsSource)
        _itemsView.Filter = Function(obj)
                                Dim c = TryCast(obj, Cliente)
                                If c Is Nothing Then Return False
                                If String.IsNullOrWhiteSpace(_busqueda) Then Return True
                                Dim q = _busqueda.ToLower()
                                Return (c.Nombre?.ToLower().Contains(q) OrElse c.Rtn?.ToLower().Contains(q))
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
        Catch ex As Exception
            MessageBox.Show("Error al cargar clientes: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub LimpiarFormulario()
        Nombre = "" : Rtn = "" : Telefono = "" : Direccion = ""
        EmailCorporativo = "" : DniPropietario = "" : Representante = "" : DniRepresentante = ""
        RtnRepresentante = "" : TelefonoRepresentante = "" : EmailRepresentante = ""
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim cliente = TryCast(obj, Cliente)
        If cliente Is Nothing Then Return
        Dim resultado = MessageBox.Show($"¿Está seguro que desea eliminar al cliente ""{cliente.Nombre}""?",
                                        "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning)
        If resultado = MessageBoxResult.Yes Then
            Try
                Await _service.Delete(cliente.Id)
                CargarItems()
            Catch ex As Exception
                MessageBox.Show("Error al eliminar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
    End Sub

    Public Async Sub Guardar(obj As Object)
        Try
            Dim item As New Cliente With {
                .Nombre = Nombre, .Rtn = Rtn, .Telefono = Telefono,
                .Direccion = Direccion, .EmailCorporativo = EmailCorporativo,
                .DniPropietario = DniPropietario,
                .Representante = Representante, .DniRepresentante = DniRepresentante,
                .RtnRepresentante = RtnRepresentante, .TelefonoRepresentante = TelefonoRepresentante,
                .EmailRepresentante = EmailRepresentante
            }
            If _clienteEditando IsNot Nothing Then
                Await _service.Update(_clienteEditando.Id, item)
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


