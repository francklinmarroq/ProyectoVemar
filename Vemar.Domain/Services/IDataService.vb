Public Interface IDataService(Of T)
    Function GetAll() As Task(Of IEnumerable(Of T))
    Function GetById(id As Integer) As Task(Of T)
    Function Add(entity As T) As Task(Of T)
    Function Update(id As Integer, entity As T) As Task(Of T)
    Function Delete(id As Integer) As Task(Of Boolean)

End Interface
