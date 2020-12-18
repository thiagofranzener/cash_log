Imports LazyDevUtils.Extensions
Imports LazyDevUtils.Validators

Public Class ExpenseEmployeeRequestValidator

    Private ReadOnly request As ExpenseEmployeeRequestViewModel
    Sub New(request As ExpenseEmployeeRequestViewModel)
        Me.request = request
    End Sub

    Public Function Validate() As List(Of String)
        Dim errors As New List(Of String)
        If ObjectSerializationIsInvalid(Me.request, errors) Then Return errors
        ValidateFields(errors)
        If errors.Count > 0 Then Return errors
        Return Nothing
    End Function

    Private Sub ValidateFields(errors As List(Of String))
        Dim validator As New Validators()
        If request.TypeId <= 0 Then errors.Add("tipo inválido")
        If request.CompanyId <= 0 Then errors.Add("empresa inválida")
        If request.Cost <= 0 Then errors.Add("custo inválido")
        If String.IsNullOrWhiteSpace(request.Description) Then errors.Add("descrição inválida")
        If request.Description.Length > 495 Then errors.Add("descrição muito longa")
        If Not IsDateValid(request.Date) Then errors.Add("data inválida")
        If request.Picture Is Nothing Then errors.Add("imagem inválida")
    End Sub

End Class
