Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms
Imports RentalPS.WinForms.Repositories

Namespace RentalPS.WinForms.UI
    Public Class FrmStockAdjustment
        Inherits Form

        Private ReadOnly _repository As New StockTransactionRepository()
        Private ReadOnly _itemTypeComboBox As New ComboBox()
        Private ReadOnly _itemComboBox As New ComboBox()
        Private ReadOnly _movementComboBox As New ComboBox()
        Private ReadOnly _qtyTextBox As New TextBox()
        Private ReadOnly _notesTextBox As New TextBox()
        Private ReadOnly _statusLabel As New Label()

        Public Sub New()
            BackColor = AppTheme.Surface
            Font = New Font("Segoe UI", 9.0F)
            BuildLayout()

            If Not DesignModeHelper.IsDesignMode() Then
                LoadItems()
            End If
        End Sub

        Private Sub BuildLayout()
            Dim panel = New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.White, .Padding = New Padding(24)}
            Dim title = New Label With {.Dock = DockStyle.Top, .Height = 42, .Text = "Koreksi Stok", .Font = New Font("Segoe UI", 16.0F, FontStyle.Bold), .ForeColor = AppTheme.TextDark}
            Dim flow = New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False, .AutoScroll = True}

            ConfigureCombo(_itemTypeComboBox)
            _itemTypeComboBox.Items.AddRange(New Object() {"fnb", "sparepart"})
            _itemTypeComboBox.SelectedIndex = 0
            AddHandler _itemTypeComboBox.SelectedIndexChanged, Sub(sender, e) LoadItems()

            ConfigureCombo(_itemComboBox)
            ConfigureCombo(_movementComboBox)
            _movementComboBox.Items.AddRange(New Object() {"in", "out", "adjustment"})
            _movementComboBox.SelectedIndex = 0

            _notesTextBox.Multiline = True
            _notesTextBox.Height = 80

            AddControl(flow, "Jenis Item", _itemTypeComboBox)
            AddControl(flow, "Barang", _itemComboBox)
            AddControl(flow, "Jenis Mutasi", _movementComboBox)
            AddControl(flow, "Qty", _qtyTextBox)
            AddControl(flow, "Catatan", _notesTextBox)

            Dim saveButton = New Button With {.Text = "Simpan Koreksi", .Width = 340}
            AppTheme.StylePrimaryButton(saveButton)
            AddHandler saveButton.Click, AddressOf SaveButton_Click
            _statusLabel.Width = 340
            _statusLabel.Height = 60
            _statusLabel.ForeColor = AppTheme.TextMuted

            flow.Controls.Add(saveButton)
            flow.Controls.Add(_statusLabel)
            panel.Controls.Add(flow)
            panel.Controls.Add(title)
            Controls.Add(panel)
        End Sub

        Private Sub LoadItems()
            If DesignModeHelper.IsDesignMode() OrElse _itemTypeComboBox.SelectedItem Is Nothing Then
                Return
            End If

            Dim sql = If(_itemTypeComboBox.SelectedItem.ToString() = "fnb",
                         "SELECT id, name + ' | Stok: ' + CAST(stock_qty AS varchar(20)) AS name FROM fnb_items WHERE is_active = 1 ORDER BY name",
                         "SELECT id, name + ' | Stok: ' + CAST(stock_qty AS varchar(20)) AS name FROM spareparts WHERE is_active = 1 ORDER BY name")
            _itemComboBox.DataSource = _repository.LoadLookup(sql)
            _itemComboBox.DisplayMember = "name"
            _itemComboBox.ValueMember = "id"
        End Sub

        Private Sub SaveButton_Click(sender As Object, e As EventArgs)
            Try
                If _itemComboBox.SelectedValue Is Nothing Then
                    _statusLabel.ForeColor = AppTheme.Danger
                    _statusLabel.Text = "Pilih barang dulu."
                    Return
                End If

                Dim qty As Integer
                If Not Integer.TryParse(_qtyTextBox.Text.Trim(), qty) OrElse qty <= 0 Then
                    _statusLabel.ForeColor = AppTheme.Danger
                    _statusLabel.Text = "Qty harus angka lebih dari 0."
                    Return
                End If

                _repository.AdjustStock(_itemTypeComboBox.Text, Convert.ToInt64(_itemComboBox.SelectedValue), _movementComboBox.Text, qty, _notesTextBox.Text)
                _statusLabel.ForeColor = AppTheme.Success
                _statusLabel.Text = "Stok berhasil dikoreksi dan dicatat di riwayat mutasi."
                _qtyTextBox.Clear()
                _notesTextBox.Clear()
                LoadItems()
            Catch ex As Exception
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Gagal koreksi stok: " & ex.Message
            End Try
        End Sub

        Private Shared Sub ConfigureCombo(combo As ComboBox)
            combo.DropDownStyle = ComboBoxStyle.DropDownList
            combo.Width = 340
        End Sub

        Private Shared Sub AddControl(parent As FlowLayoutPanel, labelText As String, control As Control)
            Dim container = New Panel With {.Width = 340, .Height = If(TypeOf control Is TextBox AndAlso DirectCast(control, TextBox).Multiline, 118, 72)}
            Dim label = New Label With {.Dock = DockStyle.Top, .Height = 26, .Text = labelText, .ForeColor = AppTheme.TextDark, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
            control.Dock = DockStyle.Top
            container.Controls.Add(control)
            container.Controls.Add(label)
            parent.Controls.Add(container)
        End Sub
    End Class
End Namespace
