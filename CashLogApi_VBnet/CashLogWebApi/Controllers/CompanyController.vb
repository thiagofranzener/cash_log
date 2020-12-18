Imports System.ComponentModel.DataAnnotations
Imports System.Web.Http
Imports CashLogLib

Namespace Controllers

    <RoutePrefix("api/company")>
    Public Class CompanyController
        Inherits ApiController

        Private ReadOnly Connection As New ApiConnection()

        <HttpPost>
        Public Function Add(<FromBody()> ByVal company As CompanyViewModel) As IHttpActionResult
            Dim errors = New CompanyValidator(company).Validate()
            If errors IsNot Nothing Then
                Connection.Dispose()
                Return Me.BadRequest(String.Join(", ", errors))
            End If
            Dim companyRepository As New CompanyRepository(Connection.GetConnection())
            Dim result As Integer

            Try
                result = companyRepository.Add(CreateModel(company))
            Catch ex As Exception
                companyRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try

            companyRepository.Dispose()
            Return Me.Ok(result)
        End Function


        <HttpGet>
        Public Function Find() As IHttpActionResult
            Dim companyRepository As New CompanyRepository(Connection.GetConnection())
            Dim result As HashSet(Of Company) = companyRepository.Find()
            companyRepository.Dispose()
            Return Me.Ok(result)
        End Function

        <TokenAuthenthicator>
        <HttpPut>
        <Route("{companyId:int}")>
        Public Function Update(<FromBody()> ByVal company As CompanyViewModel, <FromUri()> <Range(1, Integer.MaxValue)> companyId As Integer) As IHttpActionResult
            Dim companyRepository As New CompanyRepository(Connection.GetConnection())
            If Not companyRepository.Exist(companyId) Then
                companyRepository.Dispose()
                Return Me.NotFound()
            End If

            company.Id = companyId
            Dim errors = New CompanyValidator(company).Validate()
            If errors IsNot Nothing Then
                Connection.Dispose()
                Return Me.BadRequest(String.Join(", ", errors))
            End If

            Try
                companyRepository.Update(CreateModel(company))
            Catch ex As Exception
                companyRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try

            companyRepository.Dispose()
            Return Me.StatusCode(204)
        End Function

        <TokenAuthenthicator>
        <HttpDelete>
        <Route("{companyId:int}")>
        Public Function Delete(<FromUri()> <Range(1, Integer.MaxValue)> companyId As Integer) As IHttpActionResult
            Dim companyRepository As New CompanyRepository(Connection.GetConnection())
            If Not companyRepository.Exist(companyId) Then
                companyRepository.Dispose()
                Return Me.NotFound()
            End If

            Try
                companyRepository.Delete(companyId)
            Catch ex As Exception
                companyRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try

            companyRepository.Dispose()
            Return Me.Ok()
        End Function

        <HttpGet>
        <Route("{companyId:int}")>
        Public Function FindById(<FromUri()> <Range(1, Integer.MaxValue)> companyId As Integer) As IHttpActionResult
            Dim companyRepository As New CompanyRepository(Connection.GetConnection())
            If Not companyRepository.Exist(companyId) Then Return Me.NotFound()

            Dim result As Company
            Try
                result = companyRepository.FindById(companyId)
            Catch ex As Exception
                companyRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try

            companyRepository.Dispose()
            Return Me.Ok(result)
        End Function


        <TokenAuthenthicator>
        <HttpGet>
        <Route("~/api/company/user/{username}")>
        Public Function FindByUser(<FromUri()> <Required> username As String) As IHttpActionResult
            Dim userRepository As New UserRepository(Connection.GetConnection())
            username = HttpContext.Current.User.Identity.Name
            If Not userRepository.Exist(username) Then
                userRepository.Dispose()
                Return Me.BadRequest("usuário inexistente")
            End If

            Dim companyRepository As New CompanyRepository(Connection.GetConnection())
            Dim result As Company
            Try
                result = companyRepository.FindByUsername(username)
            Catch ex As Exception
                companyRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try

            companyRepository.Dispose()
            Return Me.Ok(result)
        End Function


        Private Function CreateModel(company As CompanyViewModel) As Company
            Return New Company With {
                .Id = company.Id,
                .Name = company.Name,
                .Cnpj = company.Cnpj,
                .State = company.State,
                .City = company.City,
                .Complement = company.Complement,
                .Address = company.Address,
                .Cep = company.Cep,
                .Number = company.Number,
                .Token = company.Token
             }
        End Function

    End Class

End Namespace