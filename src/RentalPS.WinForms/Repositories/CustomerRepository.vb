Imports System.Data
Imports MySqlConnector
Imports RentalPS.WinForms.Infrastructure

Namespace RentalPS.WinForms.Repositories
    Public NotInheritable Class CustomerRepository
        Public Function Search(keyword As String) As DataTable
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "
SELECT id, code, name, phone, address, identity_number, notes, is_active, created_at
FROM customers
WHERE @keyword = ''
   OR code LIKE CONCAT('%', @keyword, '%')
   OR name LIKE CONCAT('%', @keyword, '%')
   OR phone LIKE CONCAT('%', @keyword, '%')
ORDER BY name"

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

        Public Sub Insert(code As String, name As String, phone As String, address As String, identityNumber As String, notes As String)
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "
INSERT INTO customers (code, name, phone, address, identity_number, notes)
VALUES (@code, @name, @phone, @address, @identity_number, @notes)"

                Using command = New MySqlCommand(sql, connection)
                    command.Parameters.AddWithValue("@code", code.Trim())
                    command.Parameters.AddWithValue("@name", name.Trim())
                    command.Parameters.AddWithValue("@phone", NullIfBlank(phone))
                    command.Parameters.AddWithValue("@address", NullIfBlank(address))
                    command.Parameters.AddWithValue("@identity_number", NullIfBlank(identityNumber))
                    command.Parameters.AddWithValue("@notes", NullIfBlank(notes))
                    command.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub Update(id As Long, code As String, name As String, phone As String, address As String, identityNumber As String, notes As String, isActive As Boolean)
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "
UPDATE customers
SET code = @code,
    name = @name,
    phone = @phone,
    address = @address,
    identity_number = @identity_number,
    notes = @notes,
    is_active = @is_active
WHERE id = @id"

                Using command = New MySqlCommand(sql, connection)
                    command.Parameters.AddWithValue("@id", id)
                    command.Parameters.AddWithValue("@code", code.Trim())
                    command.Parameters.AddWithValue("@name", name.Trim())
                    command.Parameters.AddWithValue("@phone", NullIfBlank(phone))
                    command.Parameters.AddWithValue("@address", NullIfBlank(address))
                    command.Parameters.AddWithValue("@identity_number", NullIfBlank(identityNumber))
                    command.Parameters.AddWithValue("@notes", NullIfBlank(notes))
                    command.Parameters.AddWithValue("@is_active", If(isActive, 1, 0))
                    command.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub Delete(id As Long)
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "DELETE FROM customers WHERE id = @id"
                Using command = New MySqlCommand(sql, connection)
                    command.Parameters.AddWithValue("@id", id)
                    command.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Private Shared Function NullIfBlank(value As String) As Object
            If String.IsNullOrWhiteSpace(value) Then
                Return DBNull.Value
            End If

            Return value.Trim()
        End Function
    End Class
End Namespace
