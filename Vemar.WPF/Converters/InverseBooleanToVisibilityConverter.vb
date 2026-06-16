Imports System.Globalization
Imports System.Windows.Data

<ValueConversion(GetType(Boolean), GetType(Visibility))>
Public Class InverseBooleanToVisibilityConverter : Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If TypeOf value Is Boolean AndAlso CBool(value) Then
            Return Visibility.Collapsed
        End If
        Return Visibility.Visible
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class
