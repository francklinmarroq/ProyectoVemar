Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input

''' <summary>
''' Attached behavior que restringe un TextBox a 13 dígitos
''' y lo formatea visualmente como XXXX-XXXX-XXXXX.
''' Uso XAML: b:DniTextBoxBehavior.IsEnabled="True"
''' </summary>
Public Class DniTextBoxBehavior

    Public Shared Function GetIsEnabled(obj As DependencyObject) As Boolean
        Return CBool(obj.GetValue(IsEnabledProperty))
    End Function
    Public Shared Sub SetIsEnabled(obj As DependencyObject, value As Boolean)
        obj.SetValue(IsEnabledProperty, value)
    End Sub

    Public Shared ReadOnly IsEnabledProperty As DependencyProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            GetType(Boolean),
            GetType(DniTextBoxBehavior),
            New UIPropertyMetadata(False, AddressOf OnIsEnabledChanged))

    Private Shared Sub OnIsEnabledChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim tb = TryCast(d, TextBox)
        If tb Is Nothing Then Return
        If CBool(e.NewValue) Then
            tb.MaxLength = 15  ' 13 dígitos + 2 guiones
            AddHandler tb.PreviewTextInput, AddressOf OnPreviewTextInput
            AddHandler tb.TextChanged, AddressOf OnTextChanged
            AddHandler tb.PreviewKeyDown, AddressOf OnPreviewKeyDown
            DataObject.AddPastingHandler(tb, AddressOf OnPaste)
        Else
            RemoveHandler tb.PreviewTextInput, AddressOf OnPreviewTextInput
            RemoveHandler tb.TextChanged, AddressOf OnTextChanged
            RemoveHandler tb.PreviewKeyDown, AddressOf OnPreviewKeyDown
            DataObject.RemovePastingHandler(tb, AddressOf OnPaste)
        End If
    End Sub

    ' Solo dígitos, máximo 13
    Private Shared Sub OnPreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        If Not e.Text.All(Function(c) Char.IsDigit(c)) Then
            e.Handled = True
            Return
        End If
        Dim tb = CType(sender, TextBox)
        Dim currentDigits = tb.Text.Replace("-", "").Length -
                            tb.SelectedText.Replace("-", "").Length
        If currentDigits >= 13 Then e.Handled = True
    End Sub

    ' Auto-formato XXXX-XXXX-XXXXX
    Private Shared _isFormatting As Boolean = False

    Private Shared Sub OnTextChanged(sender As Object, e As TextChangedEventArgs)
        If _isFormatting Then Return
        Dim tb = CType(sender, TextBox)
        Dim originalText = tb.Text
        Dim caret = tb.CaretIndex

        ' Contar cuántos dígitos había ANTES del cursor en el texto original
        Dim digitsBeforeCaret = originalText.Take(caret).Count(Function(c) Char.IsDigit(c))

        Dim raw = New String(originalText.Where(Function(c) Char.IsDigit(c)).Take(13).ToArray())
        Dim formatted As String
        If raw.Length <= 4 Then
            formatted = raw
        ElseIf raw.Length <= 8 Then
            formatted = raw.Substring(0, 4) & "-" & raw.Substring(4)
        Else
            formatted = raw.Substring(0, 4) & "-" & raw.Substring(4, 4) & "-" & raw.Substring(8)
        End If

        If originalText = formatted Then Return

        _isFormatting = True
        tb.Text = formatted
        ' Reposicionar cursor después del mismo número de dígitos en el texto formateado
        tb.CaretIndex = CaretAfterDigits(formatted, digitsBeforeCaret)
        _isFormatting = False
    End Sub

    ''' Devuelve la posición en <formatted> justo después del dígito número <n>.
    Private Shared Function CaretAfterDigits(formatted As String, n As Integer) As Integer
        Dim count = 0
        For i = 0 To formatted.Length - 1
            If count = n Then Return i
            If Char.IsDigit(formatted(i)) Then count += 1
        Next
        Return formatted.Length
    End Function

    ' Backspace inteligente en posición de guión
    Private Shared Sub OnPreviewKeyDown(sender As Object, e As KeyEventArgs)
        Dim tb = CType(sender, TextBox)
        If e.Key = Key.Back AndAlso tb.SelectionLength = 0 Then
            If tb.CaretIndex = 5 AndAlso tb.Text.Length >= 5 Then
                tb.Text = tb.Text.Substring(0, 3) & tb.Text.Substring(5)
                tb.CaretIndex = 3
                e.Handled = True
            ElseIf tb.CaretIndex = 10 AndAlso tb.Text.Length >= 10 Then
                tb.Text = tb.Text.Substring(0, 8) & tb.Text.Substring(10)
                tb.CaretIndex = 8
                e.Handled = True
            End If
        End If
    End Sub

    ' Pegar: filtrar solo dígitos
    Private Shared Sub OnPaste(sender As Object, e As DataObjectPastingEventArgs)
        If e.DataObject.GetDataPresent(GetType(String)) Then
            Dim pasted = CStr(e.DataObject.GetData(GetType(String)))
            Dim digits = New String(pasted.Where(Function(c) Char.IsDigit(c)).Take(13).ToArray())
            Dim newObj As New DataObject()
            newObj.SetData(DataFormats.UnicodeText, digits)
            e.DataObject = newObj
        End If
    End Sub

End Class
