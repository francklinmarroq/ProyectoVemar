Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Vemar.Domain

Public Class UsuariosViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private ReadOnly _service As IDataService(Of Usuario)
    Private _items As New ObservableCollection(Of Usuario)
    Private _guardarCommand As RelayCommand
    Private _nombreUsuario As String = ""
    Private _contrasena As String = ""

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public Property Items As ObservableCollection(Of Usuario)
        Get
            Return _items
        End Get
        Set(v As ObservableCollection(Of Usuario))
            _items = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Items)))
        End Set
    End Property

    Public Property NombreUsuario As String
        Get
            Return _nombreUsuario
        End Get
        Set(v As String)
            _nombreUsuario = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(NombreUsuario)))
            _guardarCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public Property Contrasena As String
        Get
            Return _contrasena
        End Get
        Set(v As String)
            _contrasena = v
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Contrasena)))
        End Set
    End Property

    Private _editando As Usuario = Nothing

    Public ReadOnly Property AgregarCommand As ICommand
    Public ReadOnly Property ModificarCommand As ICommand
    Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property GuardarCommand As ICommand
        Get
            Return _guardarCommand
        End Get
    End Property

    Public Sub New(service As IDataService(Of Usuario))
        _service = service
        AgregarCommand = New RelayCommand(Sub(o)
                                             _editando = Nothing
                                             NombreUsuario = ""
                                             Contrasena = ""
                                             Dim win As New AgregarUsuarioWindow()
                                             win.DataContext = Me
                                             win.Owner = Application.Current.MainWindow
                                             win.ShowDialog()
                                         End Sub)
        ModificarCommand = New RelayCommand(Sub(o)
                                               Dim item = TryCast(o, Usuario)
                                               If item Is Nothing Then Return
                                               _editando = item
                                               NombreUsuario = item.Usuario
                                               Contrasena = ""   ' No mostrar hash
                                               Dim win As New AgregarUsuarioWindow()
                                               win.DataContext = Me
                                               win.Owner = Application.Current.MainWindow
                                               win.ShowDialog()
                                           End Sub)
        EliminarCommand = New RelayCommand(AddressOf Eliminar)
        _guardarCommand = New RelayCommand(AddressOf Guardar, Function(o) Not String.IsNullOrWhiteSpace(NombreUsuario))
        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Items = New ObservableCollection(Of Usuario)(Await _service.GetAll())
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim item = TryCast(obj, Usuario)
        If item Is Nothing Then Return
        If MessageBox.Show($"¿Eliminar usuario ""{item.Usuario}""?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) = MessageBoxResult.Yes Then
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
            Dim hash As String
            If _editando IsNot Nothing Then
                ' Al modificar: si no se ingresa nueva contraseña conservar el hash anterior
                hash = If(String.IsNullOrWhiteSpace(Contrasena), _editando.HashContrasena, PasswordHashService.HashPassword(Contrasena))
                Await _service.Update(_editando.Id, New Usuario With {
                    .Usuario = NombreUsuario,
                    .HashContrasena = hash
                })
                _editando = Nothing
            Else
                If String.IsNullOrWhiteSpace(Contrasena) Then
                    MessageBox.Show("Debe ingresar una contraseña.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning)
                    Return
                End If
                hash = PasswordHashService.HashPassword(Contrasena)
                Await _service.Add(New Usuario With {
                    .Usuario = NombreUsuario,
                    .HashContrasena = hash
                })
            End If
            RaiseEvent GuardadoExitoso(Me, EventArgs.Empty)
            CargarItems()
        Catch ex As Exception
            MessageBox.Show("Error al guardar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class


