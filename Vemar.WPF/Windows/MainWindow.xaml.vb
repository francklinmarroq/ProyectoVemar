Class MainWindow
    Public Sub New(obj As Object)
        InitializeComponent()
        Me.Title = "Constructora Vemar"
        DataContext = obj
    End Sub

End Class
