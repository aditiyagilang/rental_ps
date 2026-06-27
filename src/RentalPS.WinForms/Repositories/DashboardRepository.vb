Imports MySqlConnector
Imports RentalPS.WinForms.Infrastructure
Imports RentalPS.WinForms.Models
Imports System.Collections.Generic

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

                Const sql = "
SELECT COALESCE(SUM(total), 0)
FROM (
  SELECT paid_amount AS total FROM rentals WHERE DATE(created_at) = CURRENT_DATE() AND status <> 'cancelled'
  UNION ALL SELECT deposit_amount FROM bookings WHERE DATE(created_at) = CURRENT_DATE() AND status <> 'cancelled'
  UNION ALL SELECT amount FROM fines WHERE DATE(created_at) = CURRENT_DATE() AND status = 'paid'
  UNION ALL SELECT total_amount FROM game_installs WHERE DATE(received_at) = CURRENT_DATE() AND status <> 'cancelled'
  UNION ALL SELECT total_amount FROM service_jobs WHERE DATE(received_at) = CURRENT_DATE() AND status <> 'cancelled'
  UNION ALL SELECT paid_amount FROM fnb_sales WHERE DATE(sale_date) = CURRENT_DATE() AND status <> 'void'
  UNION ALL SELECT paid_amount FROM sparepart_sales WHERE DATE(sale_date) = CURRENT_DATE() AND status <> 'void'
  UNION ALL SELECT amount FROM payments WHERE DATE(paid_at) = CURRENT_DATE()
) revenue"
                Using command = New MySqlCommand(sql, connection)
                    Return Convert.ToDecimal(command.ExecuteScalar())
                End Using
            End Using
        End Function

        Public Function GetRevenueLast7Days() As List(Of ChartItem)
            Dim items = New List(Of ChartItem)()
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "
SELECT revenue_date, COALESCE(SUM(total), 0) AS total
FROM (
  SELECT DATE(created_at) AS revenue_date, paid_amount AS total FROM rentals WHERE created_at >= DATE_SUB(CURRENT_DATE(), INTERVAL 6 DAY) AND status <> 'cancelled'
  UNION ALL SELECT DATE(created_at), deposit_amount FROM bookings WHERE created_at >= DATE_SUB(CURRENT_DATE(), INTERVAL 6 DAY) AND status <> 'cancelled'
  UNION ALL SELECT DATE(created_at), amount FROM fines WHERE created_at >= DATE_SUB(CURRENT_DATE(), INTERVAL 6 DAY) AND status = 'paid'
  UNION ALL SELECT DATE(received_at), total_amount FROM game_installs WHERE received_at >= DATE_SUB(CURRENT_DATE(), INTERVAL 6 DAY) AND status <> 'cancelled'
  UNION ALL SELECT DATE(received_at), total_amount FROM service_jobs WHERE received_at >= DATE_SUB(CURRENT_DATE(), INTERVAL 6 DAY) AND status <> 'cancelled'
  UNION ALL SELECT DATE(sale_date), paid_amount FROM fnb_sales WHERE sale_date >= DATE_SUB(CURRENT_DATE(), INTERVAL 6 DAY) AND status <> 'void'
  UNION ALL SELECT DATE(sale_date), paid_amount FROM sparepart_sales WHERE sale_date >= DATE_SUB(CURRENT_DATE(), INTERVAL 6 DAY) AND status <> 'void'
  UNION ALL SELECT DATE(paid_at), amount FROM payments WHERE paid_at >= DATE_SUB(CURRENT_DATE(), INTERVAL 6 DAY)
) revenue
GROUP BY revenue_date
ORDER BY revenue_date"

                Using command = New MySqlCommand(sql, connection)
                    Using reader = command.ExecuteReader()
                        While reader.Read()
                            items.Add(New ChartItem(Convert.ToDateTime(reader("revenue_date")).ToString("dd/MM"), Convert.ToDecimal(reader("total"))))
                        End While
                    End Using
                End Using
            End Using

            Return items
        End Function

        Public Function GetTransactionMixToday() As List(Of ChartItem)
            Dim items = New List(Of ChartItem)()
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "
SELECT 'Sewa' AS label, COUNT(*) AS total FROM rentals WHERE DATE(created_at) = CURRENT_DATE()
UNION ALL SELECT 'Booking', COUNT(*) FROM bookings WHERE DATE(created_at) = CURRENT_DATE()
UNION ALL SELECT 'Service', COUNT(*) FROM service_jobs WHERE DATE(received_at) = CURRENT_DATE()
UNION ALL SELECT 'Isi Game', COUNT(*) FROM game_installs WHERE DATE(received_at) = CURRENT_DATE()
UNION ALL SELECT 'Denda', COUNT(*) FROM fines WHERE DATE(created_at) = CURRENT_DATE()"

                Using command = New MySqlCommand(sql, connection)
                    Using reader = command.ExecuteReader()
                        While reader.Read()
                            items.Add(New ChartItem(reader("label").ToString(), Convert.ToDecimal(reader("total"))))
                        End While
                    End Using
                End Using
            End Using

            Return items
        End Function

        Public Function GetStockRisk() As List(Of ChartItem)
            Dim items = New List(Of ChartItem)()
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "
SELECT 'FNB Menipis' AS label, COUNT(*) AS total FROM fnb_items WHERE stock_qty <= minimum_stock
UNION ALL SELECT 'Sparepart Menipis', COUNT(*) FROM spareparts WHERE stock_qty <= minimum_stock
UNION ALL SELECT 'PS Maintenance', COUNT(*) FROM consoles WHERE availability_status = 'maintenance'
UNION ALL SELECT 'Ruang Maintenance', COUNT(*) FROM rooms WHERE status = 'maintenance'"

                Using command = New MySqlCommand(sql, connection)
                    Using reader = command.ExecuteReader()
                        While reader.Read()
                            items.Add(New ChartItem(reader("label").ToString(), Convert.ToDecimal(reader("total"))))
                        End While
                    End Using
                End Using
            End Using

            Return items
        End Function
    End Class
End Namespace
