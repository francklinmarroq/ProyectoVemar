Imports System.ComponentModel
Imports System.Threading.Tasks
Imports Vemar.Domain

Public Class LoginViewModel
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event LoginSuccess As EventHandler

    Private ReadOnly _userService As IDataService(Of Usuario)

    Private Sub OnProp(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

    ' ── Propiedades ───────────────────────────────────────────────────────

    Private _username As String = ""
    Public Property Username As String
        Get
            Return _username
        End Get
        Set(value As String)
            _username = value
            OnProp(NameOf(Username))
            _loginCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Private _password As String = ""
    Public Property Password As String
        Get
            Return _password
        End Get
        Set(value As String)
            _password = value
            OnProp(NameOf(Password))
            _loginCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Private _errorMessage As String = ""
    Public Property ErrorMessage As String
        Get
            Return _errorMessage
        End Get
        Set(value As String)
            _errorMessage = value
            OnProp(NameOf(ErrorMessage))
            OnProp(NameOf(HasError))
        End Set
    End Property

    Public ReadOnly Property HasError As Boolean
        Get
            Return Not String.IsNullOrEmpty(_errorMessage)
        End Get
    End Property

    Private _isBusy As Boolean = False
    Public Property IsBusy As Boolean
        Get
            Return _isBusy
        End Get
        Set(value As Boolean)
            _isBusy = value
            OnProp(NameOf(IsBusy))
            OnProp(NameOf(IsNotBusy))
            _loginCommand?.RaiseCanExecuteChanged()
        End Set
    End Property

    Public ReadOnly Property IsNotBusy As Boolean
        Get
            Return Not _isBusy
        End Get
    End Property

    ' ── Comandos ─────────────────────────────────────────────────────────

    Private _loginCommand As RelayCommand

    Public ReadOnly Property LoginCommand As ICommand
        Get
            Return _loginCommand
        End Get
    End Property

    ' ── Constructor ───────────────────────────────────────────────────────

    Public Sub New(userService As IDataService(Of Usuario))
        _userService = userService

        _loginCommand = New RelayCommand(
            Async Sub(o) Await LoginAsync(),
            Function(o) Not IsBusy AndAlso
                        Not String.IsNullOrWhiteSpace(Username) AndAlso
                        Not String.IsNullOrWhiteSpace(Password))
    End Sub

    ' ── Lógica de login ───────────────────────────────────────────────────

    Private Async Function LoginAsync() As Task
        IsBusy = True
        ErrorMessage = ""

        Try
            Dim allUsers = Await _userService.GetAll()
            Dim user = allUsers.FirstOrDefault(
                Function(u) u.Usuario.Equals(Username, StringComparison.OrdinalIgnoreCase))

            If user Is Nothing Then
                ErrorMessage = "Usuario no encontrado."
                IsBusy = False
                Return
            End If

            Dim valid = PasswordHashService.VerifyPassword(Password, user.HashContrasena)
            If Not valid Then
                ErrorMessage = "Contraseña incorrecta."
                IsBusy = False
                Return
            End If

            ' Autenticación exitosa
            Await Task.Delay(300)
            RaiseEvent LoginSuccess(Me, EventArgs.Empty)

        Catch ex As Exception
            ErrorMessage = "Error al iniciar sesión: " & ex.Message
            IsBusy = False
        End Try
    End Function

End Class
