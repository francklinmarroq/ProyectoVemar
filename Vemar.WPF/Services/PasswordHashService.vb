Imports System.Security.Cryptography

''' <summary>
''' Genera y verifica hashes de contraseñas usando PBKDF2-SHA256.
''' Formato almacenado: Base64(salt[16] + hash[32])
''' </summary>
Public Class PasswordHashService

    Private Const SaltSize As Integer = 16
    Private Const HashSize As Integer = 32
    Private Const Iterations As Integer = 100_000

    Public Shared Function HashPassword(password As String) As String
        Dim salt(SaltSize - 1) As Byte
        RandomNumberGenerator.Fill(salt)

        Using pbkdf2 As New Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256)
            Dim hash = pbkdf2.GetBytes(HashSize)
            Dim combined(SaltSize + HashSize - 1) As Byte
            Array.Copy(salt, 0, combined, 0, SaltSize)
            Array.Copy(hash, 0, combined, SaltSize, HashSize)
            Return Convert.ToBase64String(combined)
        End Using
    End Function

    Public Shared Function VerifyPassword(password As String, storedHash As String) As Boolean
        Try
            Dim combined = Convert.FromBase64String(storedHash)
            If combined.Length < SaltSize + HashSize Then Return False

            Dim salt(SaltSize - 1) As Byte
            Array.Copy(combined, 0, salt, 0, SaltSize)

            Using pbkdf2 As New Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256)
                Dim hash = pbkdf2.GetBytes(HashSize)
                ' Comparación en tiempo constante para evitar timing attacks
                Dim diff As Byte = 0
                For i = 0 To HashSize - 1
                    diff = diff Or (combined(SaltSize + i) Xor hash(i))
                Next
                Return diff = 0
            End Using
        Catch
            Return False
        End Try
    End Function

End Class
