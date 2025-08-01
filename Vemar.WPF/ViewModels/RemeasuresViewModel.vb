﻿Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Vemar.Domain
Imports Vemar.EF
Imports Vemar.EF.Services

Public Class RemeasuresViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged
    Private _remeasures As ObservableCollection(Of Remedida)
    Private _dataService As GenericDataService(Of Remedida)
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property Remeasures As ObservableCollection(Of Remedida)
        Get
            Return _remeasures
        End Get
        Set(value As ObservableCollection(Of Remedida))
            _remeasures = value
            OnPropertyChanged()
        End Set
    End Property
    Public Sub New()
        Dim dbContextFactory As New VemarDbContextFactory()
        _dataService = New GenericDataService(Of Remedida)(dbContextFactory)
        LoadDataAsync()
    End Sub

    Private Async Sub LoadDataAsync()
        Try
            Dim remeasuresList As IEnumerable(Of Remedida) = Await _dataService.GetAll()
            Remeasures = New ObservableCollection(Of Remedida)(remeasuresList)
        Catch ex As Exception
            MsgBox("Error loading remeasures: " & ex.Message, MsgBoxStyle.Critical, "Error")
        End Try
    End Sub


    Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

End Class
