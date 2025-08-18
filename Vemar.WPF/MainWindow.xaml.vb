Class MainWindow
    Public Sub New(dataContext As Object)
        InitializeComponent()
        Me.DataContext = dataContext
        Me.appBar.DataContext = dataContext
    End Sub
End Class
