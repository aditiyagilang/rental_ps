Imports MySqlConnector
Imports RentalPS.WinForms.Infrastructure

Namespace RentalPS.WinForms.Repositories
    Public NotInheritable Class UserRepository
        Public Function UsernameExists(username As String) As Boolean
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "SELECT COUNT(*) FROM users WHERE username = @username AND is_active = 1"
                Using command = New MySqlCommand(sql, connection)
                    command.Parameters.AddWithValue("@username", username)
                    Return Convert.ToInt32(command.ExecuteScalar()) > 0
                End Using
            End Using
        End Function
    End Class
End Namespace
