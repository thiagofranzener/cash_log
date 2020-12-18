Public Class ApiConnection
    Implements System.IDisposable

    Private ReadOnly connection As SqlClient.SqlConnection = New SqlClient.SqlConnection("")
    'Server=localhost;Database=CashLog;User Id=movtech;Password=mvt;Connection Timeout=30

    Sub New()
        connection.Open()
    End Sub

    Function GetConnection() As SqlClient.SqlConnection
        Return connection
    End Function

    Public Function GetCommand() As SqlClient.SqlCommand
        Dim command As SqlClient.SqlCommand = connection.CreateCommand()
        command.Connection = connection
        Return command
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        connection.Close()
        connection.Dispose()
    End Sub

End Class
