Imports System.Data
Imports System.Data.SqlClient
Imports System.Text
Imports LazyDevUtils.Extensions

Public Class UserRepository
    Inherits Repository

    Private ReadOnly connection As SqlConnection
    Sub New(connection As SqlConnection)
        Me.connection = connection
    End Sub

    Public Sub Add(user As User)
        user.Role = UserRole.Employee
        If IsFirstUserInCompany(user.CompanyId) Then user.Role = UserRole.Administrator
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = GetAddQuery()
        PopulateQuery(command, user)
        command.ExecuteNonQuery()
    End Sub

    Public Function Exist(user As String) As Boolean
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = $"SELECT COUNT(username) FROM CompanyUser WHERE username=@username"
        command.Parameters.AddWithValue("@username", SqlDbType.VarChar).Value = user
        If command.ExecuteScalar() > 0 Then Return True
        Return False
    End Function

    Public Function Login(user As String, password As String) As Guid
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = $"SELECT token FROM CompanyUser WHERE username=@username AND password=@password"
        command.Parameters.AddWithValue("@username", SqlDbType.VarChar).Value = user
        command.Parameters.AddWithValue("@password", SqlDbType.VarChar).Value = EncryptString(password)
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                Return reader("token")
            End If
        End Using
        Return Nothing
    End Function

    Public Function GetUsersByCompanyId(companyId As Integer) As List(Of User)
        Dim sql As New StringBuilder(GetFindQuery())
        sql.AppendLine("WHERE cus.company_id=@companyId")
        Dim result As New List(Of User)
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = sql.ToString()
        command.Parameters.AddWithValue("@companyId", SqlDbType.Int).Value = companyId
        Using reader As IDataReader = command.ExecuteReader()
            While reader.Read()
                Dim user As New User()
                PopulateUser(user, reader)
                result.Add(user)
            End While
        End Using
        Return result
    End Function

    Public Function GetUser(username As String) As User
        Dim sql As New StringBuilder(GetFindQuery())
        sql.AppendLine("WHERE cus.username=@username")
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = sql.ToString()
        command.Parameters.AddWithValue("@username", SqlDbType.VarChar).Value = username
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                Dim user As New User()
                PopulateUser(user, reader)
                Return user
            End If
        End Using
        Return Nothing
    End Function

    Private Function IsFirstUserInCompany(companyId As Integer) As Boolean
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = $"SELECT COUNT(username) FROM CompanyUser WHERE company_id=@company_id"
        command.Parameters.AddWithValue("@company_id", SqlDbType.Int).Value = companyId
        If command.ExecuteScalar() > 0 Then Return False
        Return True
    End Function


    Private Shared Function GetFindQuery() As String
        Dim sql As New StringBuilder()

        sql.AppendLine("SELECT")
        sql.AppendLine("cus.username")
        sql.AppendLine(",cus.cpf")
        sql.AppendLine(",cus.name")
        sql.AppendLine(",cus.email")
        sql.AppendLine(",cus.company_id")
        sql.AppendLine(",cus.type")
        sql.AppendLine("FROM CompanyUser cus")

        Return sql.ToString()

    End Function

    Private Shared Sub PopulateUser(user As User, reader As IDataReader)
        user.User = reader("username")
        user.Cpf = reader("cpf")
        user.Name = reader("name")
        user.Email = reader("email")
        user.CompanyId = reader("company_id")
        user.Role = reader("type")
    End Sub

    Private Shared Sub PopulateQuery(command As SqlCommand, user As User)
        command.Parameters.AddWithValue("@username", user.User)
        command.Parameters.AddWithValue("@name", user.Name)
        command.Parameters.AddWithValue("@password", EncryptString(user.Password))
        command.Parameters.AddWithValue("@email", user.Email)
        command.Parameters.AddWithValue("@cpf", user.Cpf)
        command.Parameters.AddWithValue("@company_id", user.CompanyId)
        command.Parameters.AddWithValue("@type", user.Role)
    End Sub

    Private Shared Function GetAddQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("INSERT INTO CompanyUser")
        sql.AppendLine("(username,name,password,email,cpf,company_id,token,type)")
        sql.AppendLine("VALUES")
        sql.AppendLine("(@username,@name,@password,@email,@cpf,@company_id,NEWID(),@type)")
        Return sql.ToString()
    End Function

    Public Overrides Sub Dispose()
        connection.Close()
    End Sub

End Class
