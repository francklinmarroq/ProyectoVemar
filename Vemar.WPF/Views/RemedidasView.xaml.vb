Public Class RemedidasView
    Private Sub BtnReportes_Click(sender As Object, e As RoutedEventArgs)
        Dim btn = DirectCast(sender, Button)
        If btn.ContextMenu IsNot Nothing Then
            btn.ContextMenu.PlacementTarget = btn
            btn.ContextMenu.IsOpen = True
        End If
    End Sub
End Class
