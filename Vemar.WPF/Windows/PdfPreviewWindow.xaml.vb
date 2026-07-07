Imports Microsoft.Win32

Public Class PdfPreviewWindow
    Private _tempFilePath As String
    Private _suggestedFileName As String

    Public Sub New(tempFilePath As String, Optional title As String = Nothing, Optional suggestedFileName As String = Nothing)
        InitializeComponent()
        _tempFilePath = tempFilePath
        _suggestedFileName = If(suggestedFileName, IO.Path.GetFileName(tempFilePath))
        If title IsNot Nothing Then TxtTitulo.Text = title
    End Sub

    Private Async Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Await WebView.EnsureCoreWebView2Async()
        WebView.CoreWebView2.Navigate("file:///" & _tempFilePath.Replace("\", "/"))
    End Sub

    Private Sub BtnDescargar_Click(sender As Object, e As RoutedEventArgs)
        Dim dlg As New SaveFileDialog() With {
            .Title = "Guardar PDF",
            .Filter = "Archivo PDF (*.pdf)|*.pdf",
            .FileName = _suggestedFileName,
            .DefaultExt = ".pdf"
        }
        If dlg.ShowDialog() = True Then
            IO.File.Copy(_tempFilePath, dlg.FileName, overwrite:=True)
        End If
    End Sub

    Private Sub BtnCerrar_Click(sender As Object, e As RoutedEventArgs)
        Close()
    End Sub

    Protected Overrides Sub OnClosed(e As EventArgs)
        MyBase.OnClosed(e)
        Try
            If IO.File.Exists(_tempFilePath) Then IO.File.Delete(_tempFilePath)
        Catch
        End Try
    End Sub
End Class
