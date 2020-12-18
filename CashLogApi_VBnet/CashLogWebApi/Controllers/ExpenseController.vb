Imports System.ComponentModel.DataAnnotations
Imports System.Web.Http
Imports LazyDevUtils.Extensions
Imports CashLogLib
Imports System.Net

Namespace Controllers

    <RoutePrefix("api/company/{companyId:int}/expense")>
    Public Class ExpenseController
        Inherits ApiController

        Private ReadOnly Connection As New ApiConnection()

        <TokenAuthenthicator>
        <HttpPost>
        <Route()>
        Public Function Add(<FromUri()> <Range(1, Integer.MaxValue)> companyId As Integer, <FromBody()> ByVal expense As ExpenseEmployeeRequestViewModel) As IHttpActionResult
            expense.CompanyId = companyId
            Dim user As String = HttpContext.Current.User.Identity.Name
            Dim errors = New ExpenseEmployeeRequestValidator(expense).Validate()
            If errors IsNot Nothing Then
                Connection.Dispose()
                Return Me.BadRequest(String.Join(", ", errors))
            End If

            Dim companyRepository As New CompanyRepository(Connection.GetConnection())

            If Not companyRepository.Exist(companyId) Then
                Connection.Dispose()
                Return Me.BadRequest("Empresa inexistente")
            End If

            Dim userCompany As Company = companyRepository.FindByUsername(user)
            If userCompany Is Nothing OrElse userCompany.Id <> companyId Then
                Connection.Dispose()
                Return Me.BadRequest($"Empresa inválido para o usuário {user}")
            End If

            Dim expenseCategoryRepository As New ExpenseCategoryRepository(Connection.GetConnection())
            If Not expenseCategoryRepository.Exist(companyId, expense.TypeId) Then
                Connection.Dispose()
                Return Me.BadRequest("Categoria inexistente")
            End If


            Dim manager As String = companyRepository.FindManager(companyId)
            If manager.Length = 0 Then
                Connection.Dispose()
                Return Me.BadRequest("Administrador da empresa não encontrado")
            ElseIf manager = user Then
                Connection.Dispose()
                Return Me.BadRequest("Usuário de requisição e administrador são os mesmos")
            End If

            Try
                Dim expenseRepository As New ExpenseRepository(Connection.GetConnection())
                Dim nextId As Integer = expenseRepository.AddEmployeeRequest(CreateEmployeExpenseModel(expense, user, manager))
                Connection.Dispose()
                Return Me.Ok(nextId)
            Catch ex As Exception
                Connection.Dispose()
                Return Me.InternalServerError(ex)
            End Try
        End Function



        <TokenAuthenthicator>
        <HttpPut>
        <Route("~/api/company/{companyId:int}/expense/{expenseId:int}")>
        Public Function Update(<FromUri()> <Range(1, Integer.MaxValue)> companyId As Integer, <FromUri()> <Range(1, Integer.MaxValue)> expenseId As Integer, <FromBody()> ByVal expense As ExpenseAdministratorApprovationViewModel) As IHttpActionResult
            expense.CompanyId = companyId
            expense.ExpenseId = expenseId
            Dim user As String = HttpContext.Current.User.Identity.Name
            Dim errors = New ExpenseAdministratorApprovationValidator(expense).Validate()
            If errors IsNot Nothing Then
                Connection.Dispose()
                Return Me.BadRequest(String.Join(", ", errors))
            End If

            Dim companyRepository As New CompanyRepository(Connection.GetConnection())
            If Not companyRepository.Exist(companyId) Then
                Connection.Dispose()
                Return Me.BadRequest("Empresa inexistente")
            End If

            Dim userCompany As Company = companyRepository.FindByUsername(user)
            If userCompany Is Nothing OrElse userCompany.Id <> companyId Then
                Connection.Dispose()
                Return Me.BadRequest($"Empresa inválido para o usuário {user}")
            End If

            Dim manager As String = companyRepository.FindManager(companyId)
            If manager <> user Then
                Connection.Dispose()
                Return Me.BadRequest("Usuário não é administrador da empresa")
            End If

            Dim expenseRepository As New ExpenseRepository(Connection.GetConnection())
            If Not expenseRepository.Exist(expense.CompanyId, expense.ExpenseId) Then
                Connection.Dispose()
                Return Me.BadRequest("Despesa não encontrada")
            End If

            Try
                expenseRepository.UpdateExpense(CreateAdministratorApprovationModel(expense))
                Connection.Dispose()
                Return Me.StatusCode(204)
            Catch ex As Exception
                Connection.Dispose()
                Return Me.InternalServerError(ex)
            End Try
        End Function


        <TokenAuthenthicator>
        <HttpGet>
        <Route("~/api/company/{companyId:int}/expense/{expenseId:int}")>
        Public Function FindAll(<FromUri()> <Range(1, Integer.MaxValue)> companyId As Integer, <FromUri()> <Range(1, Integer.MaxValue)> expenseId As Integer)
            Dim companyRepository As New CompanyRepository(Connection.GetConnection())
            If Not companyRepository.Exist(companyId) Then
                Connection.Dispose()
                Return Me.BadRequest("Empresa inexistente")
            End If

            Dim userCompany As Company = companyRepository.FindByUsername(HttpContext.Current.User.Identity.Name)
            If userCompany Is Nothing OrElse userCompany.Id <> companyId Then
                Connection.Dispose()
                Return Me.BadRequest($"Empresa inválido para o usuário {HttpContext.Current.User.Identity.Name}")
            End If

            Dim expenseRepository As New ExpenseRepository(Connection.GetConnection())
            Dim userRepository As New UserRepository(Connection.GetConnection())
            Dim user = userRepository.GetUser(HttpContext.Current.User.Identity.Name)
            Try
                Dim result = expenseRepository.GetById(companyId, expenseId, user)
                Connection.Dispose()
                If result Is Nothing Then Return Me.StatusCode(404)
                Return Me.Ok(result)
            Catch ex As Exception
                Connection.Dispose()
                Return Me.InternalServerError(ex)
            End Try
        End Function

        <TokenAuthenthicator>
        <HttpGet>
        <Route()>
        Public Function FindById(<FromUri()> <Range(1, Integer.MaxValue)> companyId As Integer)
            Dim companyRepository As New CompanyRepository(Connection.GetConnection())
            If Not companyRepository.Exist(companyId) Then
                Connection.Dispose()
                Return Me.BadRequest("Empresa inexistente")
            End If

            Dim userCompany As Company = companyRepository.FindByUsername(HttpContext.Current.User.Identity.Name)
            If userCompany Is Nothing OrElse userCompany.Id <> companyId Then
                Connection.Dispose()
                Return Me.BadRequest($"Empresa inválido para o usuário {HttpContext.Current.User.Identity.Name}")
            End If

            Dim expenseRepository As New ExpenseRepository(Connection.GetConnection())
            Dim userRepository As New UserRepository(Connection.GetConnection())
            Dim user = userRepository.GetUser(HttpContext.Current.User.Identity.Name)
            Try
                Dim result = expenseRepository.GetAll(companyId, user)
                Connection.Dispose()
                Return Me.Ok(result)
            Catch ex As Exception
                Connection.Dispose()
                Return Me.InternalServerError(ex)
            End Try
        End Function

        Private Function CreateEmployeExpenseModel(expense As ExpenseEmployeeRequestViewModel, employee As String, manager As String) As ExpenseEmployeeRequest
            Return New ExpenseEmployeeRequest() With {
            .CompanyId = expense.CompanyId,
            .RequestUser = employee,
            .ManagerUser = manager,
            .Cost = expense.Cost,
            .[Date] = expense.Date,
            .Description = expense.Description,
            .Picture = expense.Picture,
            .Situation = ExpenseSituation.WaitingApprovation,
            .TypeId = expense.TypeId
            }
        End Function


        Private Function CreateAdministratorApprovationModel(approvation As ExpenseAdministratorApprovationViewModel) As ExpenseAdministratorApprovation
            Return New ExpenseAdministratorApprovation() With {
            .CompanyId = approvation.CompanyId,
            .ExpenseId = approvation.ExpenseId,
            .ApprovedCost = approvation.ApprovedCost,
            .Motive = approvation.Motive,
            .Situation = approvation.Situation
            }
        End Function

    End Class

End Namespace