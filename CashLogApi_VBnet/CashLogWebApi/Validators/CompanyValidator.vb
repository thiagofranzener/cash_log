Imports LazyDevUtils.Extensions
Imports LazyDevUtils.Validators

Public Class CompanyValidator

    Private ReadOnly company As CompanyViewModel
    Sub New(company As CompanyViewModel)
        Me.company = company
    End Sub

    Public Function Validate() As List(Of String)
        Dim errors As New List(Of String)
        If ObjectSerializationIsInvalid(Me.company, errors) Then Return errors
        ValidateFields(errors)
        If errors.Count > 0 Then Return errors
        Return Nothing
    End Function

    Private Sub ValidateFields(errors As List(Of String))
        Dim validator As New Validators()
        If Not validator.ValidateCnpj(company.Cnpj) Then errors.Add("cnpj inválido")
        If Not validator.ValidateCep(company.Cep) Then errors.Add("cep inválido")
        If String.IsNullOrWhiteSpace(company.Name) Then errors.Add("nome inválido")
        If String.IsNullOrWhiteSpace(company.Address) Then errors.Add("endereço inválido")
        If String.IsNullOrWhiteSpace(company.State) Then errors.Add("estado inválido")
        If String.IsNullOrWhiteSpace(company.City) Then errors.Add("cidade inválida")
        If company.Name.Length > 140 Then errors.Add("nome muito longo")
        If company.Address.Length > 140 Then errors.Add("endereço muito longo")
        If company.State.Length > 140 Then errors.Add("estado muito longo")
        If company.City.Length > 140 Then errors.Add("cidade muito longa")
    End Sub

End Class
