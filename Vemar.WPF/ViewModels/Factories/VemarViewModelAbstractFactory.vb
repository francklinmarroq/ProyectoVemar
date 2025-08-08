Public Class VemarViewModelAbstractFactory : Implements IVemarViewModelAbstractFactory
    Private ReadOnly _inicioViewModelFactory As IVemarViewModelFactory(Of InicioViewModel)
    Private ReadOnly _remedidasViewModelFactory As IVemarViewModelFactory(Of RemedidasViewModel)

    Public Sub New(homeViewModelFactory As IVemarViewModelFactory(Of InicioViewModel), remedidasViewModelFactory As IVemarViewModelFactory(Of RemedidasViewModel))
        _inicioViewModelFactory = homeViewModelFactory
        _remedidasViewModelFactory = remedidasViewModelFactory
    End Sub

    Public Function CreateViewModel(vt As ViewType) As ViewModelBase Implements IVemarViewModelAbstractFactory.CreateViewModel
        Select Case vt
            Case vt.Inicio
                Return _inicioViewModelFactory.CreateViewModel()
            Case vt.Remedidas
                Return _remedidasViewModelFactory.CreateViewModel()
            Case Else
                Throw New ArgumentException("Tipo de vista no soportado", NameOf(vt))
        End Select
    End Function
End Class
