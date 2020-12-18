Imports LazyDevUtils.Extensions
Imports LazyDevUtils.Validators

Public Class ExpenseCategoryValidator

    Private ReadOnly expenseCategory As ExpenseCategoryViewModel
    Sub New(exenseCategory As ExpenseCategoryViewModel)
        Me.expenseCategory = exenseCategory
    End Sub

    Public Function Validate() As List(Of String)
        Dim errors As New List(Of String)
        If ObjectSerializationIsInvalid(Me.expenseCategory, errors) Then Return errors
        ValidateFields(errors)
        If errors.Count > 0 Then Return errors
        Return Nothing
    End Function

    Private Sub ValidateFields(errors As List(Of String))
        If String.IsNullOrWhiteSpace(expenseCategory.Description) Then errors.Add("descrição inválida")
        If expenseCategory.Description.Length > 140 Then errors.Add("descrição muito longa")
    End Sub

End Class
