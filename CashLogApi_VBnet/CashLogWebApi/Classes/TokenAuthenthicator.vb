Imports System.Data.SqlClient
Imports System.Net
Imports System.Net.Http
Imports System.Security.Principal
Imports System.Web.Http.Controllers
Imports System.Web.Http.Filters

Public Class TokenAuthenthicator
    Inherits AuthorizationFilterAttribute
    Public Overrides Sub OnAuthorization(actionContext As HttpActionContext)

        Dim user As String = String.Empty
        Dim password As String = String.Empty

        If actionContext.Request.Headers.Authorization.Scheme.Contains("^") Then
            user = actionContext.Request.Headers.Authorization.Scheme.Split("^")(0)
            password = actionContext.Request.Headers.Authorization.Scheme.Split("^")(1)
        End If


        Using apiConnection As New ApiConnection()
            Dim connection As SqlConnection = apiConnection.GetConnection()
            Dim userRepository As New CashLogLib.UserRepository(connection)
            Dim command As SqlCommand = connection.CreateCommand()
            command.CommandText = "SELECT password FROM CompanyUser WHERE username=@username"
            command.Parameters.AddWithValue("@username", user)
            Using dr As IDataReader = command.ExecuteReader()
                If dr.Read Then
                    If password <> dr("password") & "" Then
                        user = String.Empty
                    End If
                End If
            End Using
            connection.Close()
        End Using
        Dim identity As New GenericIdentity(user)
        HttpContext.Current.User = New GenericPrincipal(identity, Nothing)

        'If actionContext.Request.Headers.Authorization Is Nothing OrElse actionContext.Request.Headers.Authorization Is String.Empty Then
        '    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized)
        'Else
        '    Dim scheme = actionContext.Request.Headers.Authorization.Scheme
        '    Dim parameter = actionContext.Request.Headers.Authorization.Parameter
        '    Dim auth As String = ""
        '    If scheme IsNot Nothing Then auth = scheme
        '    If parameter IsNot Nothing Then auth = parameter
        '    If auth.Length <> 36 AndAlso Not auth.Split("-").Length = 4 Then
        '        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized)
        '    Else
        '        Using apiConnection As New ApiConnection()
        '            Dim connection As SqlConnection = apiConnection.GetConnection()
        '            Dim userRepository As New CashLogLib.UserRepository(connection)
        '            Dim authenticationToken As New Guid(auth)
        '            Dim command As SqlCommand = connection.CreateCommand()
        '            command.CommandText = "SELECT username FROM CompanyUser WHERE token=@token"
        '            command.Parameters.AddWithValue("@token", authenticationToken)
        '            Using dr As IDataReader = command.ExecuteReader()
        '                If Not dr.Read Then
        '                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized)
        '                Else
        '                    Dim identity As New GenericIdentity(dr("username"))
        '                    HttpContext.Current.User = New GenericPrincipal(identity, Nothing)
        '                End If
        '            End Using
        '            connection.Close()
        '        End Using
        '    End If
        'End If
        MyBase.OnAuthorization(actionContext)

    End Sub

End Class