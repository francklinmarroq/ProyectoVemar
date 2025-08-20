Public Class RelayCommand : Implements ICommand

    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

    Private ReadOnly _execute As Action
    Private ReadOnly _canExecute As Func(Of Boolean)

    Public Sub New(execute As Action, Optional canExecute As Func(Of Boolean) = Nothing)
        If execute Is Nothing Then Throw New ArgumentNullException(NameOf(execute))
        _execute = execute
        _canExecute = If(canExecute, Function() True)
    End Sub
    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        Return _canExecute()
    End Function
    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        _execute()
    End Sub
    Public Sub RaiseCanExecuteChanged()
        RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
    End Sub

End Class
