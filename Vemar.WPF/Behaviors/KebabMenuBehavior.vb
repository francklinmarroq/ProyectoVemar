Imports System.Windows
Imports System.Windows.Controls

''' <summary>
''' Attached behavior que abre el ContextMenu de un Button con un clic izquierdo,
''' para usarlo como menú desplegable de acciones (botón de tres puntos).
''' Uso XAML: b:KebabMenuBehavior.OpenOnClick="True"
''' </summary>
Public Class KebabMenuBehavior

    Public Shared Function GetOpenOnClick(obj As DependencyObject) As Boolean
        Return CBool(obj.GetValue(OpenOnClickProperty))
    End Function
    Public Shared Sub SetOpenOnClick(obj As DependencyObject, value As Boolean)
        obj.SetValue(OpenOnClickProperty, value)
    End Sub

    Public Shared ReadOnly OpenOnClickProperty As DependencyProperty =
        DependencyProperty.RegisterAttached(
            "OpenOnClick",
            GetType(Boolean),
            GetType(KebabMenuBehavior),
            New UIPropertyMetadata(False, AddressOf OnOpenOnClickChanged))

    Private Shared Sub OnOpenOnClickChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim btn = TryCast(d, Button)
        If btn Is Nothing Then Return

        If CBool(e.NewValue) Then
            AddHandler btn.Click, AddressOf OnClick
        Else
            RemoveHandler btn.Click, AddressOf OnClick
        End If
    End Sub

    Private Shared Sub OnClick(sender As Object, e As RoutedEventArgs)
        Dim btn = TryCast(sender, Button)
        If btn IsNot Nothing AndAlso btn.ContextMenu IsNot Nothing Then
            btn.ContextMenu.PlacementTarget = btn
            btn.ContextMenu.IsOpen = True
        End If
    End Sub

End Class
