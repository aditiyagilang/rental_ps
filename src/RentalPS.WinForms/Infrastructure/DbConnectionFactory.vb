Imports Microsoft.Data.SqlClient

Namespace RentalPS.WinForms.Infrastructure
    Public NotInheritable Class DbConnectionFactory
        Private Sub New()
        End Sub

        Public Shared Function CreateConnection() As SqlConnection
            Return New SqlConnection(GetConnectionString())
        End Function

        Public Shared Function GetConnectionString() As String
            Dim server = GetValue("RENTAL_PS_DB_SERVER", "localhost\SQLEXPRESS")
            Dim database = GetValue("RENTAL_PS_DB_NAME", "rental_ps")
            Dim user = GetValue("RENTAL_PS_DB_USER", "")
            Dim password = GetValue("RENTAL_PS_DB_PASSWORD", "")

            If String.IsNullOrWhiteSpace(user) Then
                Return $"Server={server};Database={database};Trusted_Connection=True;TrustServerCertificate=True;"
            End If

            Return $"Server={server};Database={database};User ID={user};Password={password};TrustServerCertificate=True;"
        End Function

        Private Shared Function GetValue(name As String, fallback As String) As String
            Dim value = Environment.GetEnvironmentVariable(name)
            If String.IsNullOrWhiteSpace(value) Then
                Return fallback
            End If

            Return value
        End Function
    End Class
End Namespace
