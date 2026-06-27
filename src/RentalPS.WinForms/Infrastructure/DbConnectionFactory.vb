Imports MySqlConnector

Namespace RentalPS.WinForms.Infrastructure
    Public NotInheritable Class DbConnectionFactory
        Private Sub New()
        End Sub

        Public Shared Function CreateConnection() As MySqlConnection
            Return New MySqlConnection(GetConnectionString())
        End Function

        Public Shared Function GetConnectionString() As String
            Dim host = GetValue("RENTAL_PS_DB_HOST", "localhost")
            Dim port = GetValue("RENTAL_PS_DB_PORT", "3306")
            Dim database = GetValue("RENTAL_PS_DB_NAME", "rental_ps")
            Dim user = GetValue("RENTAL_PS_DB_USER", "root")
            Dim password = GetValue("RENTAL_PS_DB_PASSWORD", "")

            Return $"Server={host};Port={port};Database={database};User ID={user};Password={password};SslMode=Preferred;Allow User Variables=True;"
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
