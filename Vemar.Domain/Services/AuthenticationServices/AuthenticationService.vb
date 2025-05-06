Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNetCore.Identity

Public Class AuthenticationService : Implements IAuthenticationService
    Private ReadOnly _userService As IDataService(Of Usuario)

    Public Sub New(userService As IDataService(Of Usuario))
        _userService = userService
    End Sub

    Public Function Authenticate(usuario As String, contrasena As String) As Task(Of Usuario) Implements IAuthenticationService.Authenticate
        Throw New NotImplementedException()
    End Function

    Public Async Function Register(usuario As String, contrasena As String, confirmaContrasena As String, tipoUsuario As TipoUsuario) As Task(Of Boolean) Implements IAuthenticationService.Register

        Dim success As Boolean = False
        If String.Equals(contrasena, confirmaContrasena) = True Then
            Dim hasher As New PasswordHasher()
            Dim hashed As String = hasher.HashPassword(contrasena)
            Dim usr As New Usuario() With {
            .Usuario = usuario,
            .HashContrasena = hashed,
            .TipoUsuario = tipoUsuario}

            Await _userService.Add(usr)


        End If
        Return success

    End Function
End Class
