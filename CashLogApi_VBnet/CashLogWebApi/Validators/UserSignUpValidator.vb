Imports LazyDevUtils.Extensions
Imports LazyDevUtils.Validators

Public Class UserSignUpValidator

    Private ReadOnly user As UserSignUpViewModel

    Sub New(user As UserSignUpViewModel)
        Me.user = user
    End Sub

    Public Function Validate() As List(Of String)
        Dim errors As New List(Of String)
        If ObjectSerializationIsInvalid(Me.user, errors) Then Return errors
        ValidateFields(errors)
        If errors.Count > 0 Then Return errors
        Return Nothing
    End Function

    Private Sub ValidateFields(errors As List(Of String))
        Dim validator As New Validators()
        If Not validator.ValidateCpf(user.Cpf) Then errors.Add("Cpf Inválido")
        If Not validator.ValidateEmail(user.Email) Then errors.Add("E-mail Inválido")
        If String.IsNullOrWhiteSpace(user.Password) Then errors.Add("Senha Inválida")
        If String.IsNullOrWhiteSpace(user.Name) Then errors.Add("Nome Inválido")
        If String.IsNullOrWhiteSpace(user.User) Then errors.Add("Usuário Inválido")
        If user.Email.Length > 99 Then errors.Add("E-mail muito longo")
        If user.Name.Length > 100 Then errors.Add("Nome muito longo")
        If user.User.Length > 100 Then errors.Add("Usuário muito longo")
    End Sub

End Class