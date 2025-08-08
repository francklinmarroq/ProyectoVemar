Imports System.Data.Odbc
Imports System.Linq.Expressions

Public Class UpdateCurrentViewModelCommand : Implements ICommand
    Private ReadOnly _navigator As INavigator
    Private ReadOnly _viewModelFactory As IVemarViewModelAbstractFactory

    Public Sub New(navigator As INavigator, viewModelFactory As IVemarViewModelAbstractFactory)
        _navigator = navigator
        _viewModelFactory = viewModelFactory
    End Sub

    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        If TypeOf parameter Is ViewType Then
            Dim vt As ViewType = CType(parameter, ViewType)
            _navigator.CurrentViewModel = _viewModelFactory.CreateViewModel(vt)
        End If
    End Sub

    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        Return True
    End Function
End Class
