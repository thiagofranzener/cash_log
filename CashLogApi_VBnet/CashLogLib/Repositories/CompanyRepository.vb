Imports System.Data
Imports System.Data.SqlClient
Imports System.Text

Public Class CompanyRepository
    Inherits Repository

    Private ReadOnly connection As SqlConnection

    Sub New(connection As SqlConnection)
        Me.connection = connection
    End Sub

    Public Function Add(company As Company) As Integer
        company.Id = GetNextId()
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = GetAddQuery()
        PopulateQuery(command, company)
        command.ExecuteNonQuery()

        Dim expenseCategoryRepository As New ExpenseCategoryRepository(connection)
        expenseCategoryRepository.Add(company.Id, "Hospedagem")
        expenseCategoryRepository.Add(company.Id, "Transporte")
        expenseCategoryRepository.Add(company.Id, "Alimentação")
        expenseCategoryRepository.Add(company.Id, "Outros")

        Return company.Id
    End Function

    Public Function Exist(id As Integer) As Boolean
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = $"SELECT COUNT(company_id) FROM Company WHERE company_id={id}"
        If command.ExecuteScalar() > 0 Then Return True
        Return False
    End Function

    Public Sub Update(company As Company)
        Dim sql As New StringBuilder(GetUpdateQuery(company.Id))
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = sql.ToString()
        PopulateQuery(command, company)
        command.ExecuteNonQuery()
    End Sub

    Public Function FindById(id As Integer) As Company
        Dim sql As New StringBuilder(GetFindQuery())
        sql.AppendFormat("WHERE com.company_id={0}", id)
        Dim command = connection.CreateCommand()
        command.CommandText = sql.ToString()
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                Dim company As New Company()
                Populate(reader, company)
                Return company
            End If
        End Using
        Return Nothing
    End Function

    Public Function FindByUsername(username As String) As Company
        Dim sql As New StringBuilder(GetFindQuery())
        sql.AppendLine("JOIN CompanyUser cus ON")
        sql.AppendLine("com.company_id=cus.company_id")
        sql.AppendLine("WHERE cus.username=@userName")
        Dim command = connection.CreateCommand()
        command.CommandText = sql.ToString()
        command.Parameters.AddWithValue("@userName", SqlDbType.VarChar).Value = username
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                Dim company As New Company()
                Populate(reader, company)
                Return company
            End If
        End Using
        Return Nothing
    End Function

    Public Function FindManager(companyId As Integer) As String
        Dim result As String = Nothing
        Dim command = connection.CreateCommand()
        command.CommandText = GetFindManagerQuery(companyId)
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                result = reader(0)
            End If
        End Using
        Return result
    End Function

    Public Function Find() As HashSet(Of Company)
        Dim result As New HashSet(Of Company)
        Dim command = connection.CreateCommand()
        command.CommandText = GetFindQuery()
        Using reader As IDataReader = command.ExecuteReader()
            While reader.Read()
                Dim company As New Company()
                Populate(reader, company)
                result.Add(company)
            End While
        End Using
        Return result
    End Function

    Public Function FindByToken(token As Guid) As Company
        Dim result As Company = Nothing
        Dim query As New StringBuilder(GetFindQuery())
        query.AppendLine("WHERE token=@token")
        Dim command = connection.CreateCommand()
        command.CommandText = query.ToString()
        command.Parameters.AddWithValue("@token", DbType.Guid).Value = token
        Using reader As IDataReader = command.ExecuteReader()
            If reader.Read() Then
                Dim company As New Company()
                Populate(reader, company)
                result = company
            End If
        End Using
        Return result
    End Function

    Public Sub Delete(id As Integer)
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = $"DELETE FROM User WHERE company_id={id}"
        command.ExecuteNonQuery()

        command.CommandText = $"DELETE FROM Company WHERE company_id={id}"
        command.ExecuteNonQuery()
    End Sub

    Private Function GetNextId() As Integer
        Dim command As SqlCommand = connection.CreateCommand()
        command.CommandText = "SELECT ISNULL(MAX(company_id),0) + 1 FROM Company"
        Return command.ExecuteScalar()
    End Function

    Private Shared Sub PopulateQuery(command As SqlCommand, company As Company)
        command.Parameters.AddWithValue("@company_id", company.Id)
        command.Parameters.AddWithValue("@name", company.Name)
        command.Parameters.AddWithValue("@cnpj", company.Cnpj)
        command.Parameters.AddWithValue("@address", company.Address)
        command.Parameters.AddWithValue("@number", company.Number)
        command.Parameters.AddWithValue("@complement", company.Complement)
        command.Parameters.AddWithValue("@city", company.City)
        command.Parameters.AddWithValue("@state", company.State)
        command.Parameters.AddWithValue("@cep", company.Cep)
    End Sub

    Private Shared Sub Populate(reader As IDataReader, company As Company)
        company.Id = reader("company_id")
        company.Name = reader("name")
        company.Cnpj = reader("cnpj")
        company.State = reader("state")
        company.City = reader("city")
        company.Address = reader("address")
        company.Complement = reader("complement")
        company.Number = reader("number")
        company.Cep = reader("cep")
        company.Token = reader("token")
    End Sub

    Private Shared Function GetAddQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("INSERT INTO Company")
        sql.AppendLine("(company_id,name,cnpj,address,number,complement,city,state,cep,token)")
        sql.AppendLine("VALUES")
        sql.AppendLine("(@company_id,@name,@cnpj,@address,@number,@complement,@city,@state,@cep,NEWID())")
        Return sql.ToString()
    End Function

    Private Shared Function GetFindQuery() As String
        Dim sql As New StringBuilder()
        sql.AppendLine("SELECT")
        sql.AppendLine("com.company_id")
        sql.AppendLine(",com.name")
        sql.AppendLine(",com.cnpj")
        sql.AppendLine(",com.address")
        sql.AppendLine(",com.number")
        sql.AppendLine(",com.complement")
        sql.AppendLine(",com.city")
        sql.AppendLine(",com.state")
        sql.AppendLine(",com.cep")
        sql.AppendLine(",com.token")
        sql.AppendLine("FROM Company com")
        Return sql.ToString()
    End Function

    Private Shared Function GetFindManagerQuery(companyId As Integer) As String
        Dim sql As New StringBuilder()
        sql.AppendLine("SELECT username FROM CompanyUser")
        sql.AppendFormat("WHERE company_id={0}", companyId).AppendLine()
        sql.AppendFormat("AND type={0}", CInt(UserRole.Administrator))
        Return sql.ToString()
    End Function

    Private Shared Function GetUpdateQuery(companyId As Integer) As String
        Dim sql As New StringBuilder()
        sql.AppendLine("UPDATE Company SET")
        sql.AppendLine("company_id=@company_id")
        sql.AppendLine(",name=@name")
        sql.AppendLine(",cnpj=@cnpj")
        sql.AppendLine(",address=@address")
        sql.AppendLine(",number=@number")
        sql.AppendLine(",complement=@complement")
        sql.AppendLine(",city=@city")
        sql.AppendLine(",state=@state")
        sql.AppendLine(",cep=@cep")
        sql.AppendFormat("WHERE company_id={0}", companyId)
        Return sql.ToString()
    End Function

    Public Overrides Sub Dispose()
        connection.Close()
    End Sub

End Class