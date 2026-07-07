Imports System.IO
Imports System.Windows

Namespace Vemar.WPF.Reports

    Public Module PdfPreviewHelper

        ''' Guarda los bytes en un archivo temporal y abre la ventana de preview.
        ''' suggestedFileName: nombre sugerido al descargar (sin ruta, sin extensión).
        Public Sub ShowPreview(pdfBytes As Byte(), title As String, suggestedFileName As String)
            Dim tempPath = Path.Combine(Path.GetTempPath(), suggestedFileName & "_preview.pdf")
            File.WriteAllBytes(tempPath, pdfBytes)

            Application.Current.Dispatcher.Invoke(Sub()
                Dim win As New PdfPreviewWindow(tempPath, title, suggestedFileName & ".pdf")
                win.Owner = Application.Current.MainWindow
                win.ShowDialog()
            End Sub)
        End Sub

    End Module

End Namespace
