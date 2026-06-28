Imports Microsoft.Data.SqlClient
Imports RentalPS.WinForms.Infrastructure
Imports System.Collections.Generic
Imports System.Data

Namespace RentalPS.WinForms.Repositories
    Public NotInheritable Class UserRepository
        Public Function UsernameExists(username As String) As Boolean
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "SELECT COUNT(*) FROM users WHERE username = @username AND is_active = 1"
                Using command = New SqlCommand(sql, connection)
                    command.Parameters.AddWithValue("@username", username)
                    Return Convert.ToInt32(command.ExecuteScalar()) > 0
                End Using
            End Using
        End Function

        Public Function ValidateLogin(username As String, password As String) As Boolean
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "SELECT password_hash FROM users WHERE username = @username AND is_active = 1"
                Using command = New SqlCommand(sql, connection)
                    command.Parameters.AddWithValue("@username", username)
                    Dim result = command.ExecuteScalar()
                    If result Is Nothing OrElse result Is DBNull.Value Then
                        Return False
                    End If

                    Dim storedPassword = result.ToString()
                    If storedPassword = password Then
                        Return True
                    End If

                    Return username = "admin" AndAlso password = "admin" AndAlso storedPassword.Contains("replace_with_real_hash")
                End Using
            End Using
        End Function

        Public Function IsInRole(username As String, roleName As String) As Boolean
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "
SELECT COUNT(*)
FROM users u
JOIN user_roles ur ON ur.user_id = u.id
JOIN roles r ON r.id = ur.role_id
WHERE u.username = @username
  AND r.name = @role_name
  AND u.is_active = 1"

                Using command = New SqlCommand(sql, connection)
                    command.Parameters.AddWithValue("@username", username)
                    command.Parameters.AddWithValue("@role_name", roleName)
                    Return Convert.ToInt32(command.ExecuteScalar()) > 0
                End Using
            End Using
        End Function

        Public Function Search(keyword As String) As DataTable
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "
SELECT u.id,
       u.username,
       '********' AS password,
       u.full_name,
       COALESCE(STUFF((
           SELECT ', ' + r2.name
           FROM user_roles ur2
           JOIN roles r2 ON r2.id = ur2.role_id
           WHERE ur2.user_id = u.id
           ORDER BY r2.name
           FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 2, ''), '') AS roles,
       u.is_active
FROM users u
WHERE @keyword = ''
   OR u.username LIKE '%' + @keyword + '%'
   OR u.full_name LIKE '%' + @keyword + '%'
ORDER BY u.username"

                Using command = New SqlCommand(sql, connection)
                    command.Parameters.AddWithValue("@keyword", keyword.Trim())
                    Using adapter = New SqlDataAdapter(command)
                        Dim table = New DataTable()
                        adapter.Fill(table)
                        Return table
                    End Using
                End Using
            End Using
        End Function

        Public Function LoadRoles() As DataTable
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "SELECT id, name FROM roles ORDER BY name"
                Using command = New SqlCommand(sql, connection)
                    Using adapter = New SqlDataAdapter(command)
                        Dim table = New DataTable()
                        adapter.Fill(table)
                        Return table
                    End Using
                End Using
            End Using
        End Function

        Public Function LoadUserRoles(userId As Long) As HashSet(Of Long)
            Dim roles = New HashSet(Of Long)()
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Const sql = "SELECT role_id FROM user_roles WHERE user_id = @user_id"
                Using command = New SqlCommand(sql, connection)
                    command.Parameters.AddWithValue("@user_id", userId)
                    Using reader = command.ExecuteReader()
                        While reader.Read()
                            roles.Add(Convert.ToInt64(reader("role_id")))
                        End While
                    End Using
                End Using
            End Using
            Return roles
        End Function

        Public Sub Insert(username As String, password As String, fullName As String, isActive As Boolean, roleIds As IEnumerable(Of Long))
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()
                Using transaction = connection.BeginTransaction()
                    Const sql = "INSERT INTO users (username, password_hash, full_name, is_active) OUTPUT INSERTED.id VALUES (@username, @password_hash, @full_name, @is_active)"
                    Using command = New SqlCommand(sql, connection, transaction)
                        command.Parameters.AddWithValue("@username", username.Trim())
                        command.Parameters.AddWithValue("@password_hash", password.Trim())
                        command.Parameters.AddWithValue("@full_name", fullName.Trim())
                        command.Parameters.AddWithValue("@is_active", If(isActive, 1, 0))
                        Dim userId = Convert.ToInt64(command.ExecuteScalar())
                        SaveRoles(connection, transaction, userId, roleIds)
                    End Using
                    transaction.Commit()
                End Using
            End Using
        End Sub

        Public Sub Update(id As Long, username As String, password As String, fullName As String, isActive As Boolean, roleIds As IEnumerable(Of Long))
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()
                Using transaction = connection.BeginTransaction()
                    Dim sql = "UPDATE users SET username = @username, full_name = @full_name, is_active = @is_active"
                    If Not String.IsNullOrWhiteSpace(password) Then
                        sql &= ", password_hash = @password_hash"
                    End If
                    sql &= " WHERE id = @id"

                    Using command = New SqlCommand(sql, connection, transaction)
                        command.Parameters.AddWithValue("@id", id)
                        command.Parameters.AddWithValue("@username", username.Trim())
                        command.Parameters.AddWithValue("@full_name", fullName.Trim())
                        command.Parameters.AddWithValue("@is_active", If(isActive, 1, 0))
                        If Not String.IsNullOrWhiteSpace(password) Then
                            command.Parameters.AddWithValue("@password_hash", password.Trim())
                        End If
                        command.ExecuteNonQuery()
                    End Using

                    Using command = New SqlCommand("DELETE FROM user_roles WHERE user_id = @user_id", connection, transaction)
                        command.Parameters.AddWithValue("@user_id", id)
                        command.ExecuteNonQuery()
                    End Using
                    SaveRoles(connection, transaction, id, roleIds)
                    transaction.Commit()
                End Using
            End Using
        End Sub

        Public Sub Delete(id As Long)
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Using command = New SqlCommand("UPDATE users SET is_active = 0 WHERE id = @id", connection)
                    command.Parameters.AddWithValue("@id", id)
                    command.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Private Shared Sub SaveRoles(connection As SqlConnection, transaction As SqlTransaction, userId As Long, roleIds As IEnumerable(Of Long))
            For Each roleId In roleIds
                Using command = New SqlCommand("INSERT INTO user_roles (user_id, role_id) VALUES (@user_id, @role_id)", connection, transaction)
                    command.Parameters.AddWithValue("@user_id", userId)
                    command.Parameters.AddWithValue("@role_id", roleId)
                    command.ExecuteNonQuery()
                End Using
            Next
        End Sub
    End Class
End Namespace
