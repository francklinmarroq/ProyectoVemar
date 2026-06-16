Imports System.Globalization
Imports System.Windows.Data

''' <summary>
''' Formatea un número de teléfono almacenado (con o sin guión) como XXXX-XXXX.
''' Usado en columnas DataGrid de solo lectura.
''' </summary>
<ValueConversion(GetType(String), GetType(String))>
Public Class PhoneFormatConverter : Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type,
                            parameter As Object, culture As CultureInfo) As Object _
                            Implements IValueConverter.Convert
        If value Is Nothing Then Return ""
        Dim digits = New String(value.ToString().Where(Function(c) Char.IsDigit(c)).Take(8).ToArray())
        If digits.Length = 0 Then Return value.ToString()
        If digits.Length <= 4 Then Return digits
        Return digits.Substring(0, 4) & "-" & digits.Substring(4)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type,
                                parameter As Object, culture As CultureInfo) As Object _
                                Implements IValueConverter.ConvertBack
        ' Se almacena el valor formateado tal como aparece (XXXX-XXXX)
        Return value
    End Function

End Class
