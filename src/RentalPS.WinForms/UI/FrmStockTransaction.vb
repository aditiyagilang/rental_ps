Imports System.Collections.Generic
Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms
Imports RentalPS.WinForms.Models
Imports RentalPS.WinForms.Repositories

Namespace RentalPS.WinForms.UI
    Public Class FrmStockTransaction
        Inherits Form

        Private ReadOnly _mode As String
        Private ReadOnly _repository As New StockTransactionRepository()
        Private ReadOnly _items As New List(Of StockTransactionItem)()
        Private ReadOnly _grid As New DataGridView()
        Private ReadOnly _detailGrid As New DataGridView()
        Private ReadOnly _searchTextBox As New TextBox()
        Private ReadOnly _numberTextBox As New TextBox()
        Private ReadOnly _partyComboBox As New ComboBox()
        Private ReadOnly _datePicker As New DateTimePicker()
        Private ReadOnly _itemComboBox As New ComboBox()
        Private ReadOnly _qtyTextBox As New TextBox()
        Private ReadOnly _priceTextBox As New TextBox()
        Private ReadOnly _paidTextBox As New TextBox()
        Private ReadOnly _statusComboBox As New ComboBox()
        Private ReadOnly _notesTextBox As New TextBox()
        Private ReadOnly _totalLabel As New Label()
        Private ReadOnly _statusLabel As New Label()

        Public Sub New(mode As String)
            _mode = mode
            BackColor = AppTheme.Surface
            Font = New Font("Segoe UI", 9.0F)
            BuildLayout()

            If Not DesignModeHelper.IsDesignMode() Then
                LoadLookups()
                LoadData()
                RefreshDetails()
            End If
        End Sub

        Private ReadOnly Property IsPurchase As Boolean
            Get
                Return _mode = "sparepart_purchase"
            End Get
        End Property

        Private ReadOnly Property IsFnb As Boolean
            Get
                Return _mode = "fnb_sale"
            End Get
        End Property

        Private Sub BuildLayout()
            Dim root = New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 1, .BackColor = AppTheme.Surface}
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 54))
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 46))

            Dim listPanel = New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.White, .Padding = New Padding(16), .Margin = New Padding(0, 0, 14, 0)}
            Dim toolbar = New Panel With {.Dock = DockStyle.Top, .Height = 48}
            _searchTextBox.Dock = DockStyle.Fill
            _searchTextBox.PlaceholderText = "Cari nomor transaksi"
            AddHandler _searchTextBox.TextChanged, Sub(sender, e) LoadData()
            toolbar.Controls.Add(_searchTextBox)
            AppTheme.StyleGrid(_grid)
            _grid.Dock = DockStyle.Fill
            listPanel.Controls.Add(_grid)
            listPanel.Controls.Add(toolbar)

            Dim formPanel = New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.White, .Padding = New Padding(16)}
            Dim title = New Label With {.Dock = DockStyle.Top, .Height = 34, .Text = GetTitle(), .Font = New Font("Segoe UI", 12.0F, FontStyle.Bold), .ForeColor = AppTheme.TextDark}
            Dim flow = New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False, .AutoScroll = True}

            ConfigureCombo(_partyComboBox)
            ConfigureCombo(_itemComboBox)
            ConfigureCombo(_statusComboBox)
            _statusComboBox.Items.AddRange(New Object() {"draft", "paid", "void"})
            _statusComboBox.SelectedIndex = If(IsPurchase, 1, 0)
            _datePicker.Format = DateTimePickerFormat.Custom
            _datePicker.CustomFormat = "dd/MM/yyyy HH:mm"
            _datePicker.Width = 320
            _notesTextBox.Multiline = True
            _notesTextBox.Height = 60

            AddControl(flow, If(IsPurchase, "No Pembelian *", "No Penjualan *"), _numberTextBox)
            AddControl(flow, If(IsPurchase, "Supplier", "Pelanggan"), _partyComboBox)
            AddControl(flow, "Tanggal *", _datePicker)
            AddControl(flow, If(IsFnb, "Item FNB", "Sparepart"), _itemComboBox)
            AddControl(flow, "Qty", _qtyTextBox)
            AddControl(flow, If(IsPurchase, "Harga Beli", "Harga Jual"), _priceTextBox)

            Dim addButton = New Button With {.Text = "Tambah Item", .Width = 320}
            AppTheme.StyleSecondaryButton(addButton)
            AddHandler addButton.Click, AddressOf AddItemButton_Click
            flow.Controls.Add(addButton)

            AppTheme.StyleGrid(_detailGrid)
            _detailGrid.Width = 320
            _detailGrid.Height = 160
            flow.Controls.Add(_detailGrid)

            Dim removeButton = New Button With {.Text = "Hapus Item Dipilih", .Width = 320}
            AppTheme.StyleSecondaryButton(removeButton)
            AddHandler removeButton.Click, AddressOf RemoveItemButton_Click
            flow.Controls.Add(removeButton)

            _totalLabel.Width = 320
            _totalLabel.Height = 36
            _totalLabel.Font = New Font("Segoe UI", 14.0F, FontStyle.Bold)
            _totalLabel.ForeColor = AppTheme.TextDark
            flow.Controls.Add(_totalLabel)

            If Not IsPurchase Then
                AddControl(flow, "Dibayar", _paidTextBox)
                AddControl(flow, "Status", _statusComboBox)
            End If

            AddControl(flow, "Catatan", _notesTextBox)

            Dim saveButton = New Button With {.Text = "Simpan Transaksi", .Width = 320}
            AppTheme.StylePrimaryButton(saveButton)
            AddHandler saveButton.Click, AddressOf SaveButton_Click
            Dim newButton = New Button With {.Text = "Data Baru", .Width = 320}
            AppTheme.StyleSecondaryButton(newButton)
            AddHandler newButton.Click, Sub(sender, e) ClearForm()
            _statusLabel.Width = 320
            _statusLabel.Height = 54
            _statusLabel.ForeColor = AppTheme.TextMuted

            flow.Controls.Add(saveButton)
            flow.Controls.Add(newButton)
            flow.Controls.Add(_statusLabel)
            formPanel.Controls.Add(flow)
            formPanel.Controls.Add(title)

            root.Controls.Add(listPanel, 0, 0)
            root.Controls.Add(formPanel, 1, 0)
            Controls.Add(root)
        End Sub

        Private Sub LoadLookups()
            If IsPurchase Then
                BindLookup(_partyComboBox, "SELECT id, name FROM suppliers WHERE is_active = 1 ORDER BY name", False)
                BindLookup(_itemComboBox, "SELECT id, name, purchase_price AS price FROM spareparts WHERE is_active = 1 ORDER BY name", True)
            ElseIf IsFnb Then
                BindLookup(_partyComboBox, "SELECT id, name FROM customers WHERE is_active = 1 ORDER BY name", False)
                BindLookup(_itemComboBox, "SELECT id, name, selling_price AS price FROM fnb_items WHERE is_active = 1 ORDER BY name", True)
            Else
                BindLookup(_partyComboBox, "SELECT id, name FROM customers WHERE is_active = 1 ORDER BY name", False)
                BindLookup(_itemComboBox, "SELECT id, name, selling_price AS price FROM spareparts WHERE is_active = 1 ORDER BY name", True)
            End If

            AddHandler _itemComboBox.SelectedIndexChanged, AddressOf ItemComboBox_SelectedIndexChanged
            ItemComboBox_SelectedIndexChanged(Me, EventArgs.Empty)
        End Sub

        Private Sub BindLookup(combo As ComboBox, sql As String, required As Boolean)
            Dim table = _repository.LoadLookup(sql)
            If Not required Then
                Dim row = table.NewRow()
                row("id") = DBNull.Value
                row("name") = "-"
                table.Rows.InsertAt(row, 0)
            End If
            combo.DataSource = table
            combo.DisplayMember = "name"
            combo.ValueMember = "id"
        End Sub

        Private Sub LoadData()
            If DesignModeHelper.IsDesignMode() Then
                Return
            End If

            Dim table As DataTable
            If IsPurchase Then
                table = _repository.SearchSparepartPurchases(_searchTextBox.Text)
            ElseIf IsFnb Then
                table = _repository.SearchFnbSales(_searchTextBox.Text)
            Else
                table = _repository.SearchSparepartSales(_searchTextBox.Text)
            End If
            _grid.DataSource = table
            GridHeaderHelper.Apply(_grid)
        End Sub

        Private Sub AddItemButton_Click(sender As Object, e As EventArgs)
            If _itemComboBox.SelectedValue Is Nothing OrElse _itemComboBox.SelectedValue Is DBNull.Value Then
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Pilih barang dulu."
                Return
            End If

            Dim qty = CInt(ParseDecimal(_qtyTextBox.Text))
            Dim price = ParseDecimal(_priceTextBox.Text)
            If qty <= 0 OrElse price < 0 Then
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Qty harus lebih dari 0."
                Return
            End If

            _items.Add(New StockTransactionItem With {
                .ItemId = Convert.ToInt64(_itemComboBox.SelectedValue),
                .ItemName = _itemComboBox.Text,
                .Qty = qty,
                .Price = price
            })
            RefreshDetails()
        End Sub

        Private Sub RemoveItemButton_Click(sender As Object, e As EventArgs)
            If _detailGrid.CurrentRow Is Nothing Then
                Return
            End If

            Dim index = _detailGrid.CurrentRow.Index
            If index >= 0 AndAlso index < _items.Count Then
                _items.RemoveAt(index)
                RefreshDetails()
            End If
        End Sub

        Private Sub SaveButton_Click(sender As Object, e As EventArgs)
            Try
                If _numberTextBox.Text.Trim() = "" OrElse _items.Count = 0 Then
                    _statusLabel.ForeColor = AppTheme.Danger
                    _statusLabel.Text = "Nomor transaksi dan minimal 1 item wajib diisi."
                    Return
                End If

                Dim partyId = NullableLong(_partyComboBox.SelectedValue)
                If IsPurchase Then
                    _repository.SaveSparepartPurchase(_numberTextBox.Text, partyId, _datePicker.Value, _notesTextBox.Text, _items)
                ElseIf IsFnb Then
                    _repository.SaveFnbSale(_numberTextBox.Text, partyId, _datePicker.Value, ParseDecimal(_paidTextBox.Text), _statusComboBox.Text, _notesTextBox.Text, _items)
                Else
                    _repository.SaveSparepartSale(_numberTextBox.Text, partyId, _datePicker.Value, ParseDecimal(_paidTextBox.Text), _statusComboBox.Text, _notesTextBox.Text, _items)
                End If

                _statusLabel.ForeColor = AppTheme.Success
                _statusLabel.Text = "Transaksi tersimpan dan stok sudah diperbarui."
                ClearForm()
                LoadData()
            Catch ex As Exception
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Gagal simpan: " & ex.Message
            End Try
        End Sub

        Private Sub RefreshDetails()
            Dim table = New DataTable()
            table.Columns.Add("Barang")
            table.Columns.Add("Qty", GetType(Integer))
            table.Columns.Add("Harga", GetType(Decimal))
            table.Columns.Add("Subtotal", GetType(Decimal))

            Dim total = 0D
            For Each item In _items
                table.Rows.Add(item.ItemName, item.Qty, item.Price, item.Subtotal)
                total += item.Subtotal
            Next
            _detailGrid.DataSource = table
            GridHeaderHelper.Apply(_detailGrid)
            _totalLabel.Text = "Total: " & total.ToString("N0")
        End Sub

        Private Sub ClearForm()
            _numberTextBox.Clear()
            _qtyTextBox.Clear()
            _priceTextBox.Clear()
            _paidTextBox.Clear()
            _notesTextBox.Clear()
            _items.Clear()
            RefreshDetails()
        End Sub

        Private Sub ItemComboBox_SelectedIndexChanged(sender As Object, e As EventArgs)
            Dim rowView = TryCast(_itemComboBox.SelectedItem, DataRowView)
            If rowView IsNot Nothing AndAlso rowView.Row.Table.Columns.Contains("price") AndAlso rowView("price") IsNot DBNull.Value Then
                _priceTextBox.Text = Convert.ToDecimal(rowView("price")).ToString("N0")
            End If
        End Sub

        Private Function GetTitle() As String
            If IsPurchase Then
                Return "Beli Sparepart"
            End If
            If IsFnb Then
                Return "Penjualan FNB"
            End If
            Return "Jual Sparepart"
        End Function

        Private Shared Sub ConfigureCombo(combo As ComboBox)
            combo.DropDownStyle = ComboBoxStyle.DropDownList
            combo.Width = 320
        End Sub

        Private Shared Sub AddControl(parent As FlowLayoutPanel, labelText As String, control As Control)
            Dim container = New Panel With {.Width = 320, .Height = If(TypeOf control Is TextBox AndAlso DirectCast(control, TextBox).Multiline, 98, 68)}
            Dim label = New Label With {.Dock = DockStyle.Top, .Height = 24, .Text = labelText, .ForeColor = AppTheme.TextDark, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
            control.Dock = DockStyle.Top
            container.Controls.Add(control)
            container.Controls.Add(label)
            parent.Controls.Add(container)
        End Sub

        Private Shared Function ParseDecimal(text As String) As Decimal
            Dim value As Decimal
            If Decimal.TryParse(text.Replace(",", ""), value) Then
                Return value
            End If
            Return 0D
        End Function

        Private Shared Function NullableLong(value As Object) As Long?
            If value Is Nothing OrElse value Is DBNull.Value Then
                Return Nothing
            End If
            Return Convert.ToInt64(value)
        End Function
    End Class
End Namespace
