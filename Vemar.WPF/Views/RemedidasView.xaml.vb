Public Class RemedidasView
    Public Sub New()
        InitializeComponent()

        Dim remedidasVm As New RemedidasViewModel()
        Me.DataContext = remedidasVm

        listaRemedidas.DataContext = remedidasVm


    End Sub

End Class
