Public Interface IAuthenticationService
    Function Authenticate(usuario As String, contrasena As String) As Task(Of Usuario)
    Function Register(usuario As String, contrasena As String, confirmaContrasena As String, tipoUsuario As TipoUsuario) As Task(Of Boolean)
End Interface
