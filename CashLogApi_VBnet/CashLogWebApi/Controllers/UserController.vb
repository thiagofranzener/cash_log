Imports System.ComponentModel.DataAnnotations
Imports System.Web.Http
Imports LazyDevUtils.Extensions
Imports CashLogLib
Imports System.Net

Namespace Controllers

    <RoutePrefix("api/company/{companyId}/user")>
    Public Class UserController
        Inherits ApiController

        Private ReadOnly Connection As New ApiConnection()

        <HttpPost>
        <Route("~/api/company/user")>
        Public Function Add(<FromBody()> ByVal user As UserSignUpViewModel) As IHttpActionResult
            Try
                Dim errors = New UserSignUpValidator(user).Validate()
                If errors IsNot Nothing Then
                    Connection.Dispose()
                    Return Me.BadRequest(String.Join(", ", errors))
                End If

                Dim companyRepository As New CompanyRepository(Connection.GetConnection())
                Dim company = companyRepository.FindByToken(user.CompanyToken)
                If company Is Nothing Then
                    Connection.Dispose()
                    Return Me.BadRequest("empresa não encontrada")
                End If

                Dim userRepository As New UserRepository(Connection.GetConnection())
                If userRepository.Exist(user.User) Then
                    Connection.Dispose()
                    Return Me.BadRequest("nome de usuário já utilizado")
                End If
                userRepository.Add(CreateModel(user, company))

                Connection.Dispose()
                Return Me.Ok()
            Catch ex As Exception
                Connection.Dispose()
                Return Me.InternalServerError(ex)
            End Try
        End Function


        <HttpPost>
        <Route("~/api/user/auth")>
        Public Function Auth(<FromBody()> ByVal user As UserLoginViewModel) As IHttpActionResult
            Try
                Dim companyRepository As New CompanyRepository(Connection.GetConnection())
                Dim userRepository As New UserRepository(Connection.GetConnection())
                Dim token As Guid = userRepository.Login(user.User, user.Password)
                If token = Guid.Empty Then
                    Connection.Dispose()
                    Return Me.BadRequest("Usuário ou senha inválido")
                End If
                Dim company As CompanyViewModel = CompanyModelToViewModel(companyRepository.FindByUsername(user.User))
                Dim userModel As UserViewModel = ModelToViewModel(userRepository.GetUser(user.User))
                Dim result As New UserAuthenticationResult() With {
                .Token = token,
                .Company = company,
                .User = userModel
                }
                Return Me.Ok(result)
            Catch ex As Exception
                Connection.Dispose()
                Return Me.InternalServerError(ex)
            End Try
        End Function

        <TokenAuthenthicator>
        <HttpGet>
        <Route("~/api/user")>
        Public Function FindUser() As IHttpActionResult
            Dim userRepository As New UserRepository(Connection.GetConnection())
            Dim result As UserViewModel
            Try
                result = ModelToViewModel(userRepository.GetUser(HttpContext.Current.User.Identity.Name))
            Catch ex As Exception
                userRepository.Dispose()
                Return Me.InternalServerError(ex)
            End Try
            userRepository.Dispose()
            If result Is Nothing Then Return Me.BadRequest("Usuário ou senha incorretos!")
            Return Me.Ok(result)
        End Function


        <TokenAuthenthicator>
        <HttpGet>
        <Route("~/api/user/company/{companyId:int}")>
        Public Function FindUsersByCompany(<FromUri> companyId As Integer) As IHttpActionResult
            Try
                Dim companyRepository As New CompanyRepository(Connection.GetConnection())
                Dim companyExists As Boolean = False

                companyExists = companyRepository.Exist(companyId)


                If Not companyExists Then
                    Connection.Dispose()
                    Return Me.BadRequest("empresa inexistente")
                End If

                Dim userRepository As New UserRepository(Connection.GetConnection())
                Dim listOfModels As List(Of User)


                listOfModels = userRepository.GetUsersByCompanyId(companyId)

                companyRepository.Dispose()
                userRepository.Dispose()
                Dim differentCompanyFromUserRequesting As Boolean = True
                Dim result As New List(Of UserViewModel)
                listOfModels.ForEach(Sub(user)
                                         If user.User.Equals(HttpContext.Current.User.Identity.Name) Then
                                             differentCompanyFromUserRequesting = False
                                         End If
                                         result.Add(ModelToViewModel(user))
                                     End Sub)
                If differentCompanyFromUserRequesting Then
                    Return Me.Unauthorized()
                End If
                Return Me.Ok(result)
            Catch ex As Exception
                Connection.Dispose()
                Return Me.InternalServerError(ex)
            End Try
        End Function

        Private Function CreateModel(user As UserSignUpViewModel, company As Company) As User
            Return New User With {
                .User = user.User,
                .Name = user.Name,
                .Email = user.Email,
                .Cpf = user.Cpf,
                .CompanyId = company.Id,
                .Password = user.Password
             }
        End Function

        Private Function ModelToViewModel(user As User) As UserViewModel
            If user Is Nothing Then Return Nothing
            Return New UserViewModel() With {
            .CompanyId = user.CompanyId,
            .Cpf = user.Cpf,
            .Email = user.Email,
            .Name = user.Name,
            .User = user.User,
            .Role = user.Role
            }
        End Function

        Private Function CompanyModelToViewModel(company As Company) As CompanyViewModel
            If User Is Nothing Then Return Nothing
            Return New CompanyViewModel() With {
                .Id = company.Id,
                .Address = company.Address,
                .Cep = company.Cep,
                .City = company.City,
                .Cnpj = company.Cnpj,
                .Complement = company.Complement,
                .Name = company.Name,
                .Number = company.Number,
                .State = company.State,
                .Token = company.Token
            }
        End Function

    End Class

End Namespace