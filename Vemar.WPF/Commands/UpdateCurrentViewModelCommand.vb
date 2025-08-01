﻿Imports System.Data.Odbc
Imports System.Linq.Expressions

Public Class UpdateCurrentViewModelCommand : Implements ICommand
    Private _navigator As INavigator

    Public Sub New(navigator As INavigator)
        _navigator = navigator
    End Sub

    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        If TypeOf parameter Is ViewType Then
            Dim vt As ViewType = CType(parameter, ViewType)
            Select Case vt
                Case vt.Home
                    _navigator.CurrentViewModel = New HomeViewModel
                Case vt.Remeasures
                    _navigator.CurrentViewModel = New RemeasuresViewModel
            End Select
        End If
    End Sub

    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        Return True
    End Function
End Class
