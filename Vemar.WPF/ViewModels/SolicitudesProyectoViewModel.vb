Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Windows.Data
Imports Vemar.Domain
Public Class SolicitudesProyectoViewModel : Inherits ViewModelBase : Implements INotifyPropertyChanged

    Private ReadOnly _service As IDataService(Of CobroProyecto)
    Private ReadOnly _proyectoFijo As Proyecto
    Private _itemsSource As New ObservableCollection(Of CobroProyecto)
    Private _itemsView As ICollectionView

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event GuardadoExitoso As EventHandler

    Public ReadOnly Property TituloProyecto As String
        Get
            Return $"Solicitudes de Pago — {_proyectoFijo.Nombre}"
        End Get
    End Property

    Public ReadOnly Property ItemsView As ICollectionView
        Get
            Return _itemsView
        End Get
    End Property

    Public ReadOnly Property TotalSolicitado As Decimal
        Get
            Return _itemsSource.Sum(Function(c) c.Monto)
        End Get
    End Property

    Public ReadOnly Property TotalEfectuado As Decimal
        Get
            Return _itemsSource.Where(Function(c) c.EsEfectuado).Sum(Function(c) c.Monto)
        End Get
    End Property

    Public ReadOnly Property TotalPendiente As Decimal
        Get
            Return _itemsSource.Where(Function(c) Not c.EsEfectuado).Sum(Function(c) c.Monto)
        End Get
    End Property

    Public ReadOnly Property ValorProyecto As Decimal
        Get
            Return _proyectoFijo.ValorProyecto
        End Get
    End Property

    Public ReadOnly Property SaldoRestante As Decimal
        Get
            Return _proyectoFijo.ValorProyecto - TotalSolicitado
        End Get
    End Property

    Public ReadOnly Property MarcarEfectuadoCommand As ICommand
    Public ReadOnly Property EliminarCommand As ICommand
    Public ReadOnly Property VerPdfCommand As ICommand

    Public Sub New(service As IDataService(Of CobroProyecto), proyectoFijo As Proyecto)
        _service = service
        _proyectoFijo = proyectoFijo

        MarcarEfectuadoCommand = New RelayCommand(AddressOf ToggleEfectuado)

        EliminarCommand = New RelayCommand(AddressOf Eliminar)

        VerPdfCommand = New RelayCommand(Async Sub(o)
                                             Dim c = TryCast(o, CobroProyecto)
                                             If c Is Nothing Then Return
                                             Try
                                                 Dim todosAnteriores = _itemsSource.Where(
                                                     Function(x) x.Id < c.Id).OrderBy(Function(x) x.Id).ToList()
                                                 Dim rpt As New Vemar.WPF.Reports.SolicitudPagoReport()
                                                 Await rpt.GeneratePdfAsync(_proyectoFijo, _proyectoFijo.ValorProyecto, todosAnteriores, c.Monto)
                                             Catch ex As Exception
                                                 MessageBox.Show("Error al generar PDF: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
                                             End Try
                                         End Sub)

        _itemsView = CollectionViewSource.GetDefaultView(_itemsSource)
        _itemsView.Filter = Function(obj)
                                Dim c = TryCast(obj, CobroProyecto)
                                If c Is Nothing Then Return False
                                Return c.Proyecto?.Id = _proyectoFijo.Id
                            End Function

        Dim view = TryCast(_itemsView, ListCollectionView)
        If view IsNot Nothing Then
            view.SortDescriptions.Add(New SortDescription("Id", ListSortDirection.Ascending))
        End If

        CargarItems()
    End Sub

    Public Async Sub CargarItems()
        Try
            Dim lista = Await _service.GetAll()
            _itemsSource.Clear()
            For Each c In lista
                _itemsSource.Add(c)
            Next
            _itemsView.Refresh()
            NotifyResumen()
        Catch ex As Exception
            MessageBox.Show("Error al cargar solicitudes: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Async Sub ToggleEfectuado(obj As Object)
        Dim c = TryCast(obj, CobroProyecto)
        If c Is Nothing Then Return
        Try
            c.EsEfectuado = Not c.EsEfectuado
            Await _service.Update(c.Id, c)
            _itemsView.Refresh()
            NotifyResumen()
        Catch ex As Exception
            c.EsEfectuado = Not c.EsEfectuado
            MessageBox.Show("Error al actualizar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Public Async Sub Eliminar(obj As Object)
        Dim c = TryCast(obj, CobroProyecto)
        If c Is Nothing Then Return
        Dim resultado = MessageBox.Show(
            $"¿Desea eliminar la solicitud ""{c.Descripcion}"" por L {c.Monto:N2}?",
            "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning)
        If resultado = MessageBoxResult.Yes Then
            Try
                Await _service.Delete(c.Id)
                CargarItems()
            Catch ex As Exception
                MessageBox.Show("Error al eliminar: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
    End Sub

    Private Sub NotifyResumen()
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TotalSolicitado)))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TotalEfectuado)))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TotalPendiente)))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SaldoRestante)))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ItemsView)))
    End Sub

End Class
