Imports System.Collections.Generic
Imports System.Data
Imports System.Text
Imports Microsoft.Data.SqlClient
Imports RentalPS.WinForms.Infrastructure
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.Repositories
    Public NotInheritable Class MasterDataRepository
        Public Function Search(config As MasterFormConfig, keyword As String) As DataTable
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Dim sql = BuildSearchSql(config)
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

        Public Function LoadLookup(sql As String) As DataTable
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Using command = New SqlCommand(sql, connection)
                    Using adapter = New SqlDataAdapter(command)
                        Dim table = New DataTable()
                        adapter.Fill(table)
                        Return table
                    End Using
                End Using
            End Using
        End Function

        Public Sub Insert(config As MasterFormConfig, values As Dictionary(Of String, Object))
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Dim columns = New List(Of String)()
                Dim parameters = New List(Of String)()

                For Each field In config.Fields
                    columns.Add(QuoteName(field.ColumnName))
                    parameters.Add("@" & field.ColumnName)
                Next

                Dim sql = $"INSERT INTO {QuoteName(config.TableName)} ({String.Join(", ", columns)}) VALUES ({String.Join(", ", parameters)})"
                Using command = New SqlCommand(sql, connection)
                    AddParameters(command, config, values)
                    command.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub Update(config As MasterFormConfig, id As Long, values As Dictionary(Of String, Object))
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Dim assignments = New List(Of String)()
                For Each field In config.Fields
                    assignments.Add($"{QuoteName(field.ColumnName)} = @{field.ColumnName}")
                Next

                Dim sql = $"UPDATE {QuoteName(config.TableName)} SET {String.Join(", ", assignments)} WHERE id = @id"
                Using command = New SqlCommand(sql, connection)
                    command.Parameters.AddWithValue("@id", id)
                    AddParameters(command, config, values)
                    command.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub Delete(config As MasterFormConfig, id As Long)
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()

                Dim sql = $"DELETE FROM {QuoteName(config.TableName)} WHERE id = @id"
                Using command = New SqlCommand(sql, connection)
                    command.Parameters.AddWithValue("@id", id)
                    command.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Private Shared Sub AddParameters(command As SqlCommand, config As MasterFormConfig, values As Dictionary(Of String, Object))
            For Each field In config.Fields
                If values.ContainsKey(field.ColumnName) Then
                    command.Parameters.AddWithValue("@" & field.ColumnName, values(field.ColumnName))
                Else
                    command.Parameters.AddWithValue("@" & field.ColumnName, DBNull.Value)
                End If
            Next
        End Sub

        Private Shared Function BuildSearchSql(config As MasterFormConfig) As String
            Dim selectColumns = New List(Of String) From {QuoteName("id")}
            For Each field In config.Fields
                selectColumns.Add(QuoteName(field.ColumnName))
            Next

            Dim builder = New StringBuilder()
            builder.Append("SELECT ")
            builder.Append(String.Join(", ", selectColumns))
            builder.Append(" FROM ")
            builder.Append(QuoteName(config.TableName))

            If config.SearchColumns.Count > 0 Then
                builder.Append(" WHERE @keyword = ''")
                For Each column In config.SearchColumns
                    builder.Append(" OR ")
                    builder.Append(QuoteName(column))
                    builder.Append(" LIKE '%' + @keyword + '%'")
                Next
            End If

            If Not String.IsNullOrWhiteSpace(config.OrderBy) Then
                builder.Append(" ORDER BY ")
                builder.Append(QuoteName(config.OrderBy))
            End If

            Return builder.ToString()
        End Function

        Private Shared Function QuoteName(name As String) As String
            Return "[" & name.Replace("]", "]]") & "]"
        End Function
    End Class
End Namespace
