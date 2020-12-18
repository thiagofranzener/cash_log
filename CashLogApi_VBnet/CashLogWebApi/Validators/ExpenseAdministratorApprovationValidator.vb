Imports LazyDevUtils.Extensions
Imports LazyDevUtils.Validators

Public Class ExpenseAdministratorApprovationValidator

    Private ReadOnly approvation As ExpenseAdministratorApprovationViewModel
    Sub New(approvation As ExpenseAdministratorApprovationViewModel)
        Me.approvation = approvation
    End Sub

    Public Function Validate() As List(Of String)
        Dim errors As New List(Of String)
        If ObjectSerializationIsInvalid(Me.approvation, errors) Then Return errors
        ValidateFields(errors)
        If errors.Count > 0 Then Return errors
        Return Nothing
    End Function

    Private Sub ValidateFields(errors As List(Of String))
        Dim validator As New Validators()
        If approvation.ApprovedCost <> Nothing AndAlso approvation.ApprovedCost < 0 Then errors.Add("custo aprovado inválido")
        If approvation.Situation <= CInt(CashLogLib.ExpenseSituation.WaitingApprovation) Then errors.Add("situação inválida, deve ser aprovada [1] ou reprovada [2]")
        If approvation.Motive.Length > 500 Then errors.Add("Tamanho do motivo não deve exceder 500 caracteres")
    End Sub

End Class
