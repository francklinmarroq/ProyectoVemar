Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Data
Imports Vemar.Domain
Imports Vemar.WPF.Reports

Public Class ColaboradoresViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged

    Private ReadOnly _service As IDataService(Of Colaborador)
    Private _itemsSource As New ObservableCollection(Of Colaborador)
    Private _itemsView As ICollectionView
    Private _guardarCommand As RelayCommand
    Private _colaboradorEditando As Colaborador = Nothing

    Private _busqueda As String = ""
    Private _tituloFormulario As String = "Nuevo Colaborador"
    Private _dni As String = ""
    Private _nombre As String = ""
    Private _telefono As String = ""
    Private _cargo As String = ""
    Private _domicilio As String = ""
    Private _email As String = ""
    Private _fechaNacimiento As DateTime = DateTime.Today.AddYears(-25)

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
        Set(v As String)
            _busqueda = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Busqueda)))
            _itemsView?.Refresh()
        End Set
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

    Public Property Dni As String
        Get
            Return _dni
        End Get
        Set(v As String)
            _dni = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Dni)))
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

    Public Property Telefono As String
        Get
            Return _telefono
        End Get
        Set(v As String)
            _telefono = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Telefono)))
        End Set
    End Property

    Public Property Cargo As String
        Get
            Return _cargo
        End Get
        Set(v As String)
            _cargo = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Cargo)))
        End Set
    End Property

    Public Property Domicilio As String
        Get
            Return _domicilio
        End Get
        Set(v As String)
            _domicilio = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Domicilio)))
        End Set
    End Property

    Public Property Email As String
        Get
            Return _email
        End Get
        Set(v As String)
            _email = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Email)))
        End Set
    End Property

    Public Property FechaNacimiento As DateTime
        Get
            Return _fechaNacimiento
        End Get
        Set(v As DateTime)
            _fechaNacimiento = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(FechaNacimiento)))
        End Set
    End Property

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property ModificarCommand As ICommand
    Public ReadOnly Property ExportarExcelCommand As ICommand
    Public ReadOnly Property ExportarPdfCommand As ICommand
        Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of Colaborador))
        _service = service

        AgregarCommand = New RelayCommand(Sub(o)
                                             _colaboradorEditando = Nothing
                                             TituloFormulario = "Nuevo Colaborador"
                                             LimpiarFormulario()
                                             Dim win As New AgregarColaboradorWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow

                                             win.ShowDialog()
                                         End Sub)

        ModificarCommand = New RelayCommand(Sub(o)
                                               Dim c = TryCast(o, Colaborador)
                                               If c Is Nothing Then Return
                                               _colaboradorEditando = c
                                               TituloFormulario = "Modificar Colaborador"
                                               Dni = c.Dni
                                               Nombre = c.Nombre
                                               Telefono = c.Telefono
                                               Cargo = c.Cargo
                                               Domicilio = c.Domicilio
                                               Email = c.Email
                                               FechaNacimiento = If(c.FechaNacimiento = Nothing, DateTime.Today.AddYears(-25), c.FechaNacimiento)
                                               Dim win As New AgregarColaboradorWindow()
                                               win.DataContext = Me
                                               win.Owner = Application.Current.MainWindow

                                               win.ShowDialog()
                                           End Sub)

        EliminarCommand = New RelayCommand(AddressOf Eliminar)

        Dim _reportCol As New Vemar.WPF.Reports.ColaboradoresReport()
        ExportarExcelCommand = New RelayCommand(Sub(o)
                                                   _reportCol.GenerateExcelAsync(_itemsSource.ToList(), "Colaboradores")
                                               End Sub)
        ExportarPdfCommand = New RelayCommand(Sub(o)
                                                  _reportCol.GeneratePdfAsync(_itemsSource.ToList(), "Colaboradores")
                                              End Sub)

        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) Not String.IsNullOrWhiteSpace(Nombre))

        _itemsView = CollectionViewSource.GetDefaultView(_itemsSource)
        _itemsView.Filter = Function(obj)
                                Dim c = TryCast(obj, Colaborador)
                                If c Is Nothing Then Return False
                                If String.IsNullOrWhiteSpace(_busqueda) Then Return True
                                Return c.Nombre?.ToLower().Contains(_busqueda.ToLower())
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
            MessageBox.Show("Error al cargar colaboradores: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub LimpiarFormulario()
        Dni = "" : Nombre = "" : Telefono = "" : Cargo = "" : Domicilio = "" : Email = ""
        FechaNacimiento = DateTime.Today.AddYears(-25)
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim c = TryCast(obj, Colaborador)
        If c Is Nothing Then Return
        Dim resultado = MessageBox.Show($"¿Está seguro que desea eliminar al colaborador ""{c.Nombre}""?",
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
            Dim item As New Colaborador With {
                .Dni = Dni, .Nombre = Nombre, .Telefono = Telefono,
                .Cargo = Cargo, .Domicilio = Domicilio, .Email = Email,
                .FechaNacimiento = FechaNacimiento
            }
            If _colaboradorEditando IsNot Nothing Then
                Await _service.Update(_colaboradorEditando.Id, item)
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
