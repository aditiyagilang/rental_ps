Imports System.Data
Imports MySqlConnector
Imports RentalPS.WinForms.Infrastructure

Namespace RentalPS.WinForms.Repositories
    Public NotInheritable Class ReportRepository
        Public Function LoadReport(reportName As String, startDate As Date, endDate As Date) As DataTable
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Dim sql = GetSql(reportName)
                Using command = New MySqlCommand(sql, connection)
                    command.Parameters.AddWithValue("@start_date", startDate)
                    command.Parameters.AddWithValue("@end_date", endDate.AddDays(1))

                    Using adapter = New MySqlDataAdapter(command)
                        Dim table = New DataTable()
                        adapter.Fill(table)
                        Return table
                    End Using
                End Using
            End Using
        End Function

        Private Shared Function GetSql(reportName As String) As String
            Select Case reportName
                Case "Pendapatan"
                    Return "
SELECT revenue_date, source, reference_no, customer, amount
FROM (
  SELECT DATE(r.created_at) AS revenue_date, 'Sewa' AS source, r.rental_no AS reference_no, c.name AS customer, r.paid_amount AS amount
  FROM rentals r JOIN customers c ON c.id = r.customer_id
  WHERE r.created_at >= @start_date AND r.created_at < @end_date AND r.status <> 'cancelled'
  UNION ALL
  SELECT DATE(b.created_at), 'Booking', b.booking_no, c.name, b.deposit_amount
  FROM bookings b JOIN customers c ON c.id = b.customer_id
  WHERE b.created_at >= @start_date AND b.created_at < @end_date AND b.status <> 'cancelled'
  UNION ALL
  SELECT DATE(f.created_at), 'Denda', f.fine_no, c.name, f.amount
  FROM fines f JOIN customers c ON c.id = f.customer_id
  WHERE f.created_at >= @start_date AND f.created_at < @end_date AND f.status = 'paid'
  UNION ALL
  SELECT DATE(g.received_at), 'Isi Game', g.install_no, c.name, g.total_amount
  FROM game_installs g JOIN customers c ON c.id = g.customer_id
  WHERE g.received_at >= @start_date AND g.received_at < @end_date AND g.status <> 'cancelled'
  UNION ALL
  SELECT DATE(s.received_at), 'Service', s.service_no, c.name, s.total_amount
  FROM service_jobs s JOIN customers c ON c.id = s.customer_id
  WHERE s.received_at >= @start_date AND s.received_at < @end_date AND s.status <> 'cancelled'
) report
ORDER BY revenue_date DESC, source"
                Case "Sewa"
                    Return "
SELECT r.rental_no, c.name AS customer, rm.name AS room, co.name AS console, r.start_time, r.actual_end_time, r.total_amount, r.paid_amount, r.status
FROM rentals r
JOIN customers c ON c.id = r.customer_id
JOIN rooms rm ON rm.id = r.room_id
JOIN consoles co ON co.id = r.console_id
WHERE r.start_time >= @start_date AND r.start_time < @end_date
ORDER BY r.start_time DESC"
                Case "Booking"
                    Return "
SELECT b.booking_no, c.name AS customer, b.start_time, b.end_time, b.deposit_amount, b.status
FROM bookings b
JOIN customers c ON c.id = b.customer_id
WHERE b.start_time >= @start_date AND b.start_time < @end_date
ORDER BY b.start_time DESC"
                Case "Service"
                    Return "
SELECT s.service_no, c.name AS customer, s.item_name, s.problem_description, s.total_amount, s.status, s.received_at
FROM service_jobs s
JOIN customers c ON c.id = s.customer_id
WHERE s.received_at >= @start_date AND s.received_at < @end_date
ORDER BY s.received_at DESC"
                Case "Stok Menipis"
                    Return "
SELECT 'FNB' AS item_type, code, name, stock_qty, minimum_stock, unit
FROM fnb_items
WHERE stock_qty <= minimum_stock
UNION ALL
SELECT 'Sparepart', code, name, stock_qty, minimum_stock, unit
FROM spareparts
WHERE stock_qty <= minimum_stock
ORDER BY item_type, name"
                Case "Mutasi Stok"
                    Return "
SELECT sm.created_at,
       sm.item_type,
       COALESCE(f.name, s.name) AS item_name,
       sm.movement_type,
       sm.qty,
       sm.reference_type,
       sm.reference_id,
       sm.notes
FROM stock_movements sm
LEFT JOIN fnb_items f ON sm.item_type = 'fnb' AND f.id = sm.item_id
LEFT JOIN spareparts s ON sm.item_type = 'sparepart' AND s.id = sm.item_id
WHERE sm.created_at >= @start_date AND sm.created_at < @end_date
ORDER BY sm.created_at DESC"
                Case Else
                    Return "
SELECT payment_no, reference_type, reference_id, amount, paid_at
FROM payments
WHERE paid_at >= @start_date AND paid_at < @end_date
ORDER BY paid_at DESC"
            End Select
        End Function
    End Class
End Namespace
