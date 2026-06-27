Imports System.Collections.Generic
Imports System.Data
Imports MySqlConnector
Imports RentalPS.WinForms.Infrastructure
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.Repositories
    Public NotInheritable Class StockTransactionRepository
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

        Public Function SearchFnbSales(keyword As String) As DataTable
            Return Search("fnb_sales", "sale_no", "sale_date", keyword)
        End Function

        Public Function SearchSparepartSales(keyword As String) As DataTable
            Return Search("sparepart_sales", "sale_no", "sale_date", keyword)
        End Function

        Public Function SearchSparepartPurchases(keyword As String) As DataTable
            Return Search("sparepart_purchases", "purchase_no", "purchase_date", keyword)
        End Function

        Public Sub SaveFnbSale(saleNo As String, customerId As Long?, saleDate As DateTime, paidAmount As Decimal, status As String, notes As String, items As List(Of StockTransactionItem))
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()
                Using transaction = connection.BeginTransaction()
                    Dim total = SumItems(items)
                    Dim saleId = InsertFnbSale(connection, transaction, saleNo, customerId, saleDate, total, paidAmount, status, notes)
                    For Each item In items
                        ExecuteNonQuery(connection, transaction, "INSERT INTO fnb_sale_items (fnb_sale_id, fnb_item_id, qty, price, subtotal) VALUES (@ref_id, @item_id, @qty, @price, @subtotal)", saleId, item)
                        MoveStock(connection, transaction, "fnb", "out", item.ItemId, item.Qty, "fnb_sale", saleId, "Penjualan FNB " & saleNo)
                    Next
                    transaction.Commit()
                End Using
            End Using
        End Sub

        Public Sub SaveSparepartSale(saleNo As String, customerId As Long?, saleDate As DateTime, paidAmount As Decimal, status As String, notes As String, items As List(Of StockTransactionItem))
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()
                Using transaction = connection.BeginTransaction()
                    Dim total = SumItems(items)
                    Dim saleId = InsertSparepartSale(connection, transaction, saleNo, customerId, saleDate, total, paidAmount, status, notes)
                    For Each item In items
                        ExecuteNonQuery(connection, transaction, "INSERT INTO sparepart_sale_items (sparepart_sale_id, sparepart_id, qty, price, subtotal) VALUES (@ref_id, @item_id, @qty, @price, @subtotal)", saleId, item)
                        MoveStock(connection, transaction, "sparepart", "out", item.ItemId, item.Qty, "sparepart_sale", saleId, "Penjualan sparepart " & saleNo)
                    Next
                    transaction.Commit()
                End Using
            End Using
        End Sub

        Public Sub SaveSparepartPurchase(purchaseNo As String, supplierId As Long?, purchaseDate As DateTime, notes As String, items As List(Of StockTransactionItem))
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()
                Using transaction = connection.BeginTransaction()
                    Dim total = SumItems(items)
                    Dim purchaseId = InsertSparepartPurchase(connection, transaction, purchaseNo, supplierId, purchaseDate, total, notes)
                    For Each item In items
                        ExecuteNonQuery(connection, transaction, "INSERT INTO sparepart_purchase_items (sparepart_purchase_id, sparepart_id, qty, purchase_price, subtotal) VALUES (@ref_id, @item_id, @qty, @price, @subtotal)", purchaseId, item)
                        MoveStock(connection, transaction, "sparepart", "in", item.ItemId, item.Qty, "sparepart_purchase", purchaseId, "Pembelian sparepart " & purchaseNo)
                    Next
                    transaction.Commit()
                End Using
            End Using
        End Sub

        Public Sub AdjustStock(itemType As String, itemId As Long, movementType As String, qty As Integer, notes As String)
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()
                Using transaction = connection.BeginTransaction()
                    MoveStock(connection, transaction, itemType, movementType, itemId, qty, "stock_adjustment", Nothing, notes)
                    transaction.Commit()
                End Using
            End Using
        End Sub

        Private Shared Function Search(tableName As String, numberColumn As String, dateColumn As String, keyword As String) As DataTable
            Using connection = DbConnectionFactory.CreateConnection()
                connection.Open()
                Dim sql = $"SELECT * FROM `{tableName}` WHERE @keyword = '' OR `{numberColumn}` LIKE CONCAT('%', @keyword, '%') OR status LIKE CONCAT('%', @keyword, '%') ORDER BY `{dateColumn}` DESC"
                If tableName = "sparepart_purchases" Then
                    sql = $"SELECT * FROM `{tableName}` WHERE @keyword = '' OR `{numberColumn}` LIKE CONCAT('%', @keyword, '%') ORDER BY `{dateColumn}` DESC"
                End If

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

        Private Shared Function InsertFnbSale(connection As MySqlConnection, transaction As MySqlTransaction, saleNo As String, customerId As Long?, saleDate As DateTime, total As Decimal, paid As Decimal, status As String, notes As String) As Long
            Const sql = "INSERT INTO fnb_sales (sale_no, customer_id, sale_date, total_amount, paid_amount, status, notes) VALUES (@no, @customer_id, @date, @total, @paid, @status, @notes)"
            Return InsertHeader(connection, transaction, sql, saleNo, customerId, saleDate, total, paid, status, notes)
        End Function

        Private Shared Function InsertSparepartSale(connection As MySqlConnection, transaction As MySqlTransaction, saleNo As String, customerId As Long?, saleDate As DateTime, total As Decimal, paid As Decimal, status As String, notes As String) As Long
            Const sql = "INSERT INTO sparepart_sales (sale_no, customer_id, sale_date, total_amount, paid_amount, status, notes) VALUES (@no, @customer_id, @date, @total, @paid, @status, @notes)"
            Return InsertHeader(connection, transaction, sql, saleNo, customerId, saleDate, total, paid, status, notes)
        End Function

        Private Shared Function InsertSparepartPurchase(connection As MySqlConnection, transaction As MySqlTransaction, purchaseNo As String, supplierId As Long?, purchaseDate As DateTime, total As Decimal, notes As String) As Long
            Const sql = "INSERT INTO sparepart_purchases (purchase_no, supplier_id, purchase_date, total_amount, notes) VALUES (@no, @customer_id, @date, @total, @notes)"
            Return InsertHeader(connection, transaction, sql, purchaseNo, supplierId, purchaseDate, total, 0D, "", notes)
        End Function

        Private Shared Function InsertHeader(connection As MySqlConnection, transaction As MySqlTransaction, sql As String, number As String, partyId As Long?, dateValue As DateTime, total As Decimal, paid As Decimal, status As String, notes As String) As Long
            Using command = New MySqlCommand(sql, connection, transaction)
                command.Parameters.AddWithValue("@no", number.Trim())
                Dim partyValue As Object = DBNull.Value
                If partyId.HasValue Then
                    partyValue = partyId.Value
                End If
                command.Parameters.AddWithValue("@customer_id", partyValue)
                command.Parameters.AddWithValue("@date", dateValue)
                command.Parameters.AddWithValue("@total", total)
                If sql.Contains("@paid") Then
                    command.Parameters.AddWithValue("@paid", paid)
                End If
                If sql.Contains("@status") Then
                    command.Parameters.AddWithValue("@status", status)
                End If
                Dim notesValue As Object = DBNull.Value
                If Not String.IsNullOrWhiteSpace(notes) Then
                    notesValue = notes.Trim()
                End If
                command.Parameters.AddWithValue("@notes", notesValue)
                command.ExecuteNonQuery()
                Return command.LastInsertedId
            End Using
        End Function

        Private Shared Sub ExecuteNonQuery(connection As MySqlConnection, transaction As MySqlTransaction, sql As String, referenceId As Long, item As StockTransactionItem)
            Using command = New MySqlCommand(sql, connection, transaction)
                command.Parameters.AddWithValue("@ref_id", referenceId)
                command.Parameters.AddWithValue("@item_id", item.ItemId)
                command.Parameters.AddWithValue("@qty", item.Qty)
                command.Parameters.AddWithValue("@price", item.Price)
                command.Parameters.AddWithValue("@subtotal", item.Subtotal)
                command.ExecuteNonQuery()
            End Using
        End Sub

        Private Shared Sub MoveStock(connection As MySqlConnection, transaction As MySqlTransaction, itemType As String, movementType As String, itemId As Long, qty As Integer, referenceType As String, referenceId As Long?, notes As String)
            If qty <= 0 Then
                Throw New InvalidOperationException("Qty stok harus lebih dari 0.")
            End If

            Dim tableName = If(itemType = "fnb", "fnb_items", "spareparts")
            Dim delta = If(movementType = "out", -qty, qty)

            Dim updateSql = $"UPDATE `{tableName}` SET stock_qty = stock_qty + @delta WHERE id = @item_id"
            If movementType = "out" Then
                updateSql &= " AND stock_qty >= @qty"
            End If

            Using command = New MySqlCommand(updateSql, connection, transaction)
                command.Parameters.AddWithValue("@delta", delta)
                command.Parameters.AddWithValue("@qty", qty)
                command.Parameters.AddWithValue("@item_id", itemId)
                Dim affected = command.ExecuteNonQuery()
                If affected = 0 Then
                    Throw New InvalidOperationException("Stok tidak cukup atau barang tidak ditemukan.")
                End If
            End Using

            Using command = New MySqlCommand("INSERT INTO stock_movements (item_type, item_id, movement_type, qty, reference_type, reference_id, notes) VALUES (@item_type, @item_id, @movement_type, @qty, @reference_type, @reference_id, @notes)", connection, transaction)
                command.Parameters.AddWithValue("@item_type", itemType)
                command.Parameters.AddWithValue("@item_id", itemId)
                command.Parameters.AddWithValue("@movement_type", movementType)
                command.Parameters.AddWithValue("@qty", qty)
                command.Parameters.AddWithValue("@reference_type", referenceType)
                Dim referenceValue As Object = DBNull.Value
                If referenceId.HasValue Then
                    referenceValue = referenceId.Value
                End If
                command.Parameters.AddWithValue("@reference_id", referenceValue)

                Dim movementNotesValue As Object = DBNull.Value
                If Not String.IsNullOrWhiteSpace(notes) Then
                    movementNotesValue = notes.Trim()
                End If
                command.Parameters.AddWithValue("@notes", movementNotesValue)
                command.ExecuteNonQuery()
            End Using
        End Sub

        Private Shared Function SumItems(items As List(Of StockTransactionItem)) As Decimal
            Dim total = 0D
            For Each item In items
                total += item.Subtotal
            Next
            Return total
        End Function
    End Class
End Namespace
