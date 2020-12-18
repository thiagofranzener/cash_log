Public Class ExpenseCategory
    Implements IEquatable(Of ExpenseCategory)

    Public Property CompanyId As Integer

    Public Property CategoryId As Integer

    Public Property Description As String

    Public Overrides Function Equals(obj As Object) As Boolean
        Return Equals(TryCast(obj, ExpenseCategory))
    End Function

    Public Overloads Function Equals(other As ExpenseCategory) As Boolean Implements IEquatable(Of ExpenseCategory).Equals
        Return other IsNot Nothing AndAlso
               CompanyId = other.CompanyId AndAlso
               CategoryId = other.CategoryId
    End Function

End Class