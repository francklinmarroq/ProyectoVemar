Imports System
Imports Vemar.Domain
Imports Vemar.EF
Imports Vemar.EF.Services


Module Program
    Sub Main(args As String())
        Dim colaboradorService As IDataService(Of Colaborador) = New GenericDataService(Of Colaborador)(New VemarDbContextFactory())
        'colaboradorService.Add(New Colaborador With {
        '    .Dni = "12345678A",
        '    .Nombre = "Joe Doe"
        '}).Wait()
        Dim colaborador As Colaborador = colaboradorService.GetById(1).Result
        Console.WriteLine($"Colaborador: {colaborador.Nombre} ({colaborador.Dni})")
        Console.WriteLine($"Total Colaboradores: {colaboradorService.GetAll.Result.Count}")
    End Sub
End Module
