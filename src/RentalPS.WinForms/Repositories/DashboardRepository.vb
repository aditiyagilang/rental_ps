Imports MySqlConnector
Imports RentalPS.WinForms.Infrastructure

Namespace RentalPS.WinForms.Repositories
    Public NotInheritable Class DashboardRepository
        Public Function GetMetric(sql As String) As Integer
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Using command = New MySqlCommand(sql, connection)
                    Return Convert.ToInt32(command.ExecuteScalar())
                End Using
            End Using
        End Function

        Public Function GetRevenueToday() As Decimal
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "SELECT COALESCE(SUM(amount), 0) FROM payments WHERE DATE(paid_at) = CURRENT_DATE()"
                Using command = New MySqlCommand(sql, connection)
                    Return Convert.ToDecimal(command.ExecuteScalar())
                End Using
            End Using
        End Function
    End Class
End Namespace
