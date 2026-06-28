Imports Microsoft.Data.SqlClient
Imports RentalPS.WinForms.Infrastructure
Imports RentalPS.WinForms.Models
Imports System.Collections.Generic

Namespace RentalPS.WinForms.Repositories
    Public NotInheritable Class DashboardRepository
        Public Function GetMetric(sql As String) As Integer
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Using command = New SqlCommand(sql, connection)
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
  SELECT paid_amount AS total FROM rentals WHERE CAST(created_at AS date) = CAST(GETDATE() AS date) AND status <> 'cancelled'
  UNION ALL SELECT deposit_amount FROM bookings WHERE CAST(created_at AS date) = CAST(GETDATE() AS date) AND status <> 'cancelled'
  UNION ALL SELECT amount FROM fines WHERE CAST(created_at AS date) = CAST(GETDATE() AS date) AND status = 'paid'
  UNION ALL SELECT total_amount FROM game_installs WHERE CAST(received_at AS date) = CAST(GETDATE() AS date) AND status <> 'cancelled'
  UNION ALL SELECT total_amount FROM service_jobs WHERE CAST(received_at AS date) = CAST(GETDATE() AS date) AND status <> 'cancelled'
  UNION ALL SELECT paid_amount FROM fnb_sales WHERE CAST(sale_date AS date) = CAST(GETDATE() AS date) AND status <> 'void'
  UNION ALL SELECT paid_amount FROM sparepart_sales WHERE CAST(sale_date AS date) = CAST(GETDATE() AS date) AND status <> 'void'
  UNION ALL SELECT amount FROM payments WHERE CAST(paid_at AS date) = CAST(GETDATE() AS date)
) revenue"
                Using command = New SqlCommand(sql, connection)
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
  SELECT CAST(created_at AS date) AS revenue_date, paid_amount AS total FROM rentals WHERE created_at >= DATEADD(day, -6, CAST(GETDATE() AS date)) AND status <> 'cancelled'
  UNION ALL SELECT CAST(created_at AS date), deposit_amount FROM bookings WHERE created_at >= DATEADD(day, -6, CAST(GETDATE() AS date)) AND status <> 'cancelled'
  UNION ALL SELECT CAST(created_at AS date), amount FROM fines WHERE created_at >= DATEADD(day, -6, CAST(GETDATE() AS date)) AND status = 'paid'
  UNION ALL SELECT CAST(received_at AS date), total_amount FROM game_installs WHERE received_at >= DATEADD(day, -6, CAST(GETDATE() AS date)) AND status <> 'cancelled'
  UNION ALL SELECT CAST(received_at AS date), total_amount FROM service_jobs WHERE received_at >= DATEADD(day, -6, CAST(GETDATE() AS date)) AND status <> 'cancelled'
  UNION ALL SELECT CAST(sale_date AS date), paid_amount FROM fnb_sales WHERE sale_date >= DATEADD(day, -6, CAST(GETDATE() AS date)) AND status <> 'void'
  UNION ALL SELECT CAST(sale_date AS date), paid_amount FROM sparepart_sales WHERE sale_date >= DATEADD(day, -6, CAST(GETDATE() AS date)) AND status <> 'void'
  UNION ALL SELECT CAST(paid_at AS date), amount FROM payments WHERE paid_at >= DATEADD(day, -6, CAST(GETDATE() AS date))
) revenue
GROUP BY revenue_date
ORDER BY revenue_date"

                Using command = New SqlCommand(sql, connection)
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
SELECT 'Sewa' AS label, COUNT(*) AS total FROM rentals WHERE CAST(created_at AS date) = CAST(GETDATE() AS date)
UNION ALL SELECT 'Booking', COUNT(*) FROM bookings WHERE CAST(created_at AS date) = CAST(GETDATE() AS date)
UNION ALL SELECT 'Service', COUNT(*) FROM service_jobs WHERE CAST(received_at AS date) = CAST(GETDATE() AS date)
UNION ALL SELECT 'Isi Game', COUNT(*) FROM game_installs WHERE CAST(received_at AS date) = CAST(GETDATE() AS date)
UNION ALL SELECT 'Denda', COUNT(*) FROM fines WHERE CAST(created_at AS date) = CAST(GETDATE() AS date)"

                Using command = New SqlCommand(sql, connection)
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

                Using command = New SqlCommand(sql, connection)
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
