Public Class InicioView
    Public Sub New()
        InitializeComponent()
        Dim inicioVm As New InicioViewModel()
        Me.DataContext = inicioVm
        remedidasCard.DataContext = inicioVm.RemedidasViewModel
    End Sub

End Class
