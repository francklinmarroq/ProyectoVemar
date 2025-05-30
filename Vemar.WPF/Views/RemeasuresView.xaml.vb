Public Class RemeasuresView
    ''Assign a data context to the view
    Public Sub New()
        InitializeComponent()

        Dim remeasuresVm As New RemeasuresViewModel()
        Me.DataContext = remeasuresVm

        remeasuresList.DataContext = remeasuresVm
        Dim x = remeasuresVm.Remeasures
    End Sub

End Class
