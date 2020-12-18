Public Class Company
    Implements IEquatable(Of Company)

    Public Property Id As Integer

    Public Property Name As String

    Public Property Cnpj As String

    Public Property Address As String

    Public Property Number As Integer

    Public Property Complement As String

    Public Property City As String

    Public Property State As String

    Public Property Cep As String

    Public Property Token As Guid

    Public Overrides Function Equals(obj As Object) As Boolean
        Return Equals(TryCast(obj, Company))
    End Function

    Public Overloads Function Equals(other As Company) As Boolean Implements IEquatable(Of Company).Equals
        Return other IsNot Nothing AndAlso
               Id = other.Id
    End Function

End Class