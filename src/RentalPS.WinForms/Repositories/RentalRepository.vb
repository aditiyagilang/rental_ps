Imports System.Data
Imports MySqlConnector
Imports RentalPS.WinForms.Infrastructure

Namespace RentalPS.WinForms.Repositories
    Public NotInheritable Class RentalRepository
        Public Function Search(keyword As String) As DataTable
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "
SELECT r.id,
       r.rental_no,
       r.booking_id,
       r.customer_id,
       c.name AS customer,
       r.room_id,
       rm.name AS room,
       r.console_id,
       co.name AS console,
       r.start_time,
       r.planned_end_time,
       r.actual_end_time,
       r.hourly_rate,
       r.duration_minutes,
       r.rental_amount,
       r.discount_amount,
       r.total_amount,
       r.paid_amount,
       r.status
FROM rentals r
JOIN customers c ON c.id = r.customer_id
JOIN rooms rm ON rm.id = r.room_id
JOIN consoles co ON co.id = r.console_id
WHERE @keyword = ''
   OR r.rental_no LIKE CONCAT('%', @keyword, '%')
   OR c.name LIKE CONCAT('%', @keyword, '%')
   OR rm.name LIKE CONCAT('%', @keyword, '%')
   OR co.name LIKE CONCAT('%', @keyword, '%')
ORDER BY r.start_time DESC"

                Using command = New MySqlCommand(sql, connection)
                    command.Parameters.AddWithValue("@keyword", keyword.Trim())
                    Using adapter = New MySqlDataAdapter(command)
                        Dim table = New DataTable()
                        adapter.Fill(table)
                        Return table
                    End Using
                End Using
            End Using
        End Function

        Public Function LoadLookup(sql As String) As DataTable
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Using command = New MySqlCommand(sql, connection)
                    Using adapter = New MySqlDataAdapter(command)
                        Dim table = New DataTable()
                        adapter.Fill(table)
                        Return table
                    End Using
                End Using
            End Using
        End Function

        Public Sub Insert(values As RentalValues)
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "
INSERT INTO rentals (
  rental_no, booking_id, customer_id, room_id, console_id, start_time,
  planned_end_time, actual_end_time, hourly_rate, duration_minutes,
  rental_amount, discount_amount, total_amount, paid_amount, status, notes
) VALUES (
  @rental_no, @booking_id, @customer_id, @room_id, @console_id, @start_time,
  @planned_end_time, @actual_end_time, @hourly_rate, @duration_minutes,
  @rental_amount, @discount_amount, @total_amount, @paid_amount, @status, @notes
)"

                Using command = New MySqlCommand(sql, connection)
                    AddParameters(command, values)
                    command.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub Update(id As Long, values As RentalValues)
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "
UPDATE rentals
SET rental_no = @rental_no,
    booking_id = @booking_id,
    customer_id = @customer_id,
    room_id = @room_id,
    console_id = @console_id,
    start_time = @start_time,
    planned_end_time = @planned_end_time,
    actual_end_time = @actual_end_time,
    hourly_rate = @hourly_rate,
    duration_minutes = @duration_minutes,
    rental_amount = @rental_amount,
    discount_amount = @discount_amount,
    total_amount = @total_amount,
    paid_amount = @paid_amount,
    status = @status,
    notes = @notes
WHERE id = @id"

                Using command = New MySqlCommand(sql, connection)
                    command.Parameters.AddWithValue("@id", id)
                    AddParameters(command, values)
                    command.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Private Shared Sub AddParameters(command As MySqlCommand, values As RentalValues)
            command.Parameters.AddWithValue("@rental_no", values.RentalNo)
            command.Parameters.AddWithValue("@booking_id", NullIfZero(values.BookingId))
            command.Parameters.AddWithValue("@customer_id", values.CustomerId)
            command.Parameters.AddWithValue("@room_id", values.RoomId)
            command.Parameters.AddWithValue("@console_id", values.ConsoleId)
            command.Parameters.AddWithValue("@start_time", values.StartTime)
            command.Parameters.AddWithValue("@planned_end_time", NullIfNothing(values.PlannedEndTime))
            command.Parameters.AddWithValue("@actual_end_time", NullIfNothing(values.ActualEndTime))
            command.Parameters.AddWithValue("@hourly_rate", values.HourlyRate)
            command.Parameters.AddWithValue("@duration_minutes", values.DurationMinutes)
            command.Parameters.AddWithValue("@rental_amount", values.RentalAmount)
            command.Parameters.AddWithValue("@discount_amount", values.DiscountAmount)
            command.Parameters.AddWithValue("@total_amount", values.TotalAmount)
            command.Parameters.AddWithValue("@paid_amount", values.PaidAmount)
            command.Parameters.AddWithValue("@status", values.Status)
            Dim notesValue As Object = DBNull.Value
            If Not String.IsNullOrWhiteSpace(values.Notes) Then
                notesValue = values.Notes.Trim()
            End If
            command.Parameters.AddWithValue("@notes", notesValue)
        End Sub

        Private Shared Function NullIfZero(value As Long?) As Object
            If Not value.HasValue OrElse value.Value <= 0 Then
                Return DBNull.Value
            End If

            Return value.Value
        End Function

        Private Shared Function NullIfNothing(value As DateTime?) As Object
            If Not value.HasValue Then
                Return DBNull.Value
            End If

            Return value.Value
        End Function
    End Class

    Public Class RentalValues
        Public Property RentalNo As String
        Public Property BookingId As Long?
        Public Property CustomerId As Long
        Public Property RoomId As Long
        Public Property ConsoleId As Long
        Public Property StartTime As DateTime
        Public Property PlannedEndTime As DateTime?
        Public Property ActualEndTime As DateTime?
        Public Property HourlyRate As Decimal
        Public Property DurationMinutes As Integer
        Public Property RentalAmount As Decimal
        Public Property DiscountAmount As Decimal
        Public Property TotalAmount As Decimal
        Public Property PaidAmount As Decimal
        Public Property Status As String
        Public Property Notes As String
    End Class
End Namespace
