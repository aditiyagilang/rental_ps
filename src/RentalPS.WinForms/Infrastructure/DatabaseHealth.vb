Imports MySqlConnector

Namespace RentalPS.WinForms.Infrastructure
    Public NotInheritable Class DatabaseHealth
        Private Sub New()
        End Sub

        Public Shared Function TestConnection() As Boolean
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Using command = New MySqlCommand("SELECT 1", connection)
                    Return Convert.ToInt32(command.ExecuteScalar()) = 1
                End Using
            End Using
        End Function
    End Class
End Namespace
