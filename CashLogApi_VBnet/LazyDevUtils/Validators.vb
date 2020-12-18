Imports LazyDevUtils.Extensions

Namespace Validators

    Public Class Validators

        Sub New()
        End Sub

        Public Function ValidateCpf(value As String) As Boolean
            If value Is Nothing OrElse value.Length <> 11 OrElse Not Decimal.TryParse(value, Nothing) Then Return False
            Return True
        End Function

        Public Function ValidateEmail(value As String) As Boolean
            If value Is Nothing OrElse Not value.Contains("@") OrElse Not value.Contains(".") Then Return False
            Return True
        End Function
        Public Function ValidateCnpj(value As String) As Boolean
            If value Is Nothing OrElse value.Length <> 14 OrElse Not Decimal.TryParse(value, Nothing) Then Return False
            Return True
        End Function
        Public Function ValidateCep(value As String) As Boolean
            If value Is Nothing OrElse value.Length <> 8 OrElse Not IsNumber(value) Then Return False
            Return True
        End Function

    End Class

End Namespace