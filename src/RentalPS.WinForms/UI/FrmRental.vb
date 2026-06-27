Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms
Imports RentalPS.WinForms.Repositories

Namespace RentalPS.WinForms.UI
    Public Class FrmRental
        Inherits Form

        Private ReadOnly _repository As New RentalRepository()
        Private ReadOnly _grid As New DataGridView()
        Private ReadOnly _searchTextBox As New TextBox()
        Private ReadOnly _idTextBox As New TextBox()
        Private ReadOnly _rentalNoTextBox As New TextBox()
        Private ReadOnly _bookingComboBox As New ComboBox()
        Private ReadOnly _customerComboBox As New ComboBox()
        Private ReadOnly _roomComboBox As New ComboBox()
        Private ReadOnly _consoleComboBox As New ComboBox()
        Private ReadOnly _startPicker As New DateTimePicker()
        Private ReadOnly _plannedEndPicker As New DateTimePicker()
        Private ReadOnly _actualEndPicker As New DateTimePicker()
        Private ReadOnly _hourlyRateTextBox As New TextBox()
        Private ReadOnly _durationTextBox As New TextBox()
        Private ReadOnly _rentalAmountTextBox As New TextBox()
        Private ReadOnly _discountPercentTextBox As New TextBox()
        Private ReadOnly _discountAmountTextBox As New TextBox()
        Private ReadOnly _totalTextBox As New TextBox()
        Private ReadOnly _paidTextBox As New TextBox()
        Private ReadOnly _statusComboBox As New ComboBox()
        Private ReadOnly _notesTextBox As New TextBox()
        Private ReadOnly _statusLabel As New Label()
        Private _isLoadingSelection As Boolean

        Public Sub New()
            BackColor = AppTheme.Surface
            Font = New Font("Segoe UI", 9.0F)
            BuildLayout()

            If Not DesignModeHelper.IsDesignMode() Then
                LoadLookups()
                LoadData()
                Recalculate()
            End If
        End Sub

        Private Sub BuildLayout()
            Dim root = New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 1, .BackColor = AppTheme.Surface}
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 62))
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 38))

            Dim listPanel = New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.White, .Padding = New Padding(16), .Margin = New Padding(0, 0, 14, 0)}
            Dim toolbar = New Panel With {.Dock = DockStyle.Top, .Height = 48}
            _searchTextBox.Dock = DockStyle.Fill
            _searchTextBox.PlaceholderText = "Cari no sewa, pelanggan, ruang, atau PS"
            AddHandler _searchTextBox.TextChanged, Sub(sender, e) LoadData()
            Dim refreshButton = New Button With {.Dock = DockStyle.Right, .Width = 98, .Text = "Refresh"}
            AppTheme.StyleSecondaryButton(refreshButton)
            AddHandler refreshButton.Click, Sub(sender, e) LoadData()
            toolbar.Controls.Add(_searchTextBox)
            toolbar.Controls.Add(refreshButton)

            AppTheme.StyleGrid(_grid)
            _grid.Dock = DockStyle.Fill
            AddHandler _grid.SelectionChanged, AddressOf Grid_SelectionChanged
            listPanel.Controls.Add(_grid)
            listPanel.Controls.Add(toolbar)

            Dim formPanel = New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.White, .Padding = New Padding(16)}
            Dim title = New Label With {.Dock = DockStyle.Top, .Height = 34, .Text = "Transaksi Sewa", .Font = New Font("Segoe UI", 12.0F, FontStyle.Bold), .ForeColor = AppTheme.TextDark}
            Dim flow = New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False, .AutoScroll = True}

            _idTextBox.Visible = False
            flow.Controls.Add(_idTextBox)

            ConfigureCombo(_bookingComboBox)
            ConfigureCombo(_customerComboBox)
            ConfigureCombo(_roomComboBox)
            ConfigureCombo(_consoleComboBox)
            ConfigureCombo(_statusComboBox)
            _statusComboBox.Items.AddRange(New Object() {"running", "completed", "cancelled"})
            _statusComboBox.SelectedIndex = 0

            ConfigurePicker(_startPicker, False)
            ConfigurePicker(_plannedEndPicker, True)
            ConfigurePicker(_actualEndPicker, True)
            _plannedEndPicker.Value = DateTime.Now.AddHours(1)
            _plannedEndPicker.Checked = True

            MakeReadOnly(_durationTextBox)
            MakeReadOnly(_rentalAmountTextBox)
            MakeReadOnly(_discountAmountTextBox)
            MakeReadOnly(_totalTextBox)
            _notesTextBox.Multiline = True
            _notesTextBox.Height = 60

            AddText(flow, "No Sewa *", _rentalNoTextBox)
            AddControl(flow, "Booking", _bookingComboBox)
            AddControl(flow, "Pelanggan *", _customerComboBox)
            AddControl(flow, "Ruang *", _roomComboBox)
            AddControl(flow, "PS *", _consoleComboBox)
            AddControl(flow, "Mulai *", _startPicker)
            AddControl(flow, "Rencana Selesai", _plannedEndPicker)
            AddControl(flow, "Selesai Aktual", _actualEndPicker)
            AddText(flow, "Tarif/Jam *", _hourlyRateTextBox)
            AddText(flow, "Durasi Menit", _durationTextBox)
            AddText(flow, "Biaya Sewa", _rentalAmountTextBox)
            AddText(flow, "Diskon (%)", _discountPercentTextBox)
            AddText(flow, "Nominal Diskon", _discountAmountTextBox)
            AddText(flow, "Total", _totalTextBox)
            AddText(flow, "Dibayar", _paidTextBox)
            AddControl(flow, "Status *", _statusComboBox)
            AddText(flow, "Catatan", _notesTextBox)

            Dim saveButton = New Button With {.Text = "Simpan", .Width = 320}
            AppTheme.StylePrimaryButton(saveButton)
            AddHandler saveButton.Click, AddressOf SaveButton_Click
            Dim newButton = New Button With {.Text = "Data Baru", .Width = 320}
            AppTheme.StyleSecondaryButton(newButton)
            AddHandler newButton.Click, Sub(sender, e) ClearForm()
            _statusLabel.Width = 320
            _statusLabel.Height = 48
            _statusLabel.ForeColor = AppTheme.TextMuted

            flow.Controls.Add(saveButton)
            flow.Controls.Add(newButton)
            flow.Controls.Add(_statusLabel)
            formPanel.Controls.Add(flow)
            formPanel.Controls.Add(title)

            root.Controls.Add(listPanel, 0, 0)
            root.Controls.Add(formPanel, 1, 0)
            Controls.Add(root)

            AddHandler _startPicker.ValueChanged, Sub(sender, e) Recalculate()
            AddHandler _plannedEndPicker.ValueChanged, Sub(sender, e) Recalculate()
            AddHandler _actualEndPicker.ValueChanged, Sub(sender, e) Recalculate()
            AddHandler _hourlyRateTextBox.TextChanged, Sub(sender, e) Recalculate()
            AddHandler _discountPercentTextBox.TextChanged, Sub(sender, e) Recalculate()
        End Sub

        Private Sub LoadLookups()
            BindLookup(_bookingComboBox, "SELECT id, booking_no AS name FROM bookings WHERE status IN ('booked','checked_in') ORDER BY start_time DESC", False)
            BindLookup(_customerComboBox, "SELECT id, name FROM customers WHERE is_active = 1 ORDER BY name", True)
            BindLookup(_roomComboBox, "SELECT id, name FROM rooms WHERE status <> 'inactive' ORDER BY name", True)
            BindLookup(_consoleComboBox, "SELECT id, name FROM consoles WHERE availability_status <> 'inactive' ORDER BY name", True)
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

            Try
                Dim table = _repository.Search(_searchTextBox.Text)
                _grid.DataSource = table
                GridHeaderHelper.Apply(_grid)
                HideInternalColumns()
                _statusLabel.ForeColor = AppTheme.TextMuted
                _statusLabel.Text = table.Rows.Count.ToString() & " data sewa."
            Catch ex As Exception
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Gagal load sewa: " & ex.Message
            End Try
        End Sub

        Private Sub Grid_SelectionChanged(sender As Object, e As EventArgs)
            If _grid.CurrentRow Is Nothing OrElse _grid.CurrentRow.DataBoundItem Is Nothing Then
                Return
            End If

            Dim rowView = TryCast(_grid.CurrentRow.DataBoundItem, DataRowView)
            If rowView Is Nothing Then
                Return
            End If

            _isLoadingSelection = True
            Try
                _idTextBox.Text = rowView("id").ToString()
                _rentalNoTextBox.Text = rowView("rental_no").ToString()
                SetComboValue(_bookingComboBox, rowView("booking_id"))
                SetComboValue(_customerComboBox, rowView("customer_id"))
                SetComboValue(_roomComboBox, rowView("room_id"))
                SetComboValue(_consoleComboBox, rowView("console_id"))
                _hourlyRateTextBox.Text = Convert.ToDecimal(rowView("hourly_rate")).ToString("N0")
                _durationTextBox.Text = rowView("duration_minutes").ToString()
                _rentalAmountTextBox.Text = Convert.ToDecimal(rowView("rental_amount")).ToString("N0")
                _discountAmountTextBox.Text = Convert.ToDecimal(rowView("discount_amount")).ToString("N0")
                _totalTextBox.Text = Convert.ToDecimal(rowView("total_amount")).ToString("N0")
                _paidTextBox.Text = Convert.ToDecimal(rowView("paid_amount")).ToString("N0")
                _statusComboBox.SelectedItem = rowView("status").ToString()

                Dim rentalAmount = Convert.ToDecimal(rowView("rental_amount"))
                Dim discountAmount = Convert.ToDecimal(rowView("discount_amount"))
                _discountPercentTextBox.Text = If(rentalAmount <= 0D, "0", Math.Round(discountAmount / rentalAmount * 100D, 2).ToString())

                If rowView("start_time") IsNot DBNull.Value Then
                    _startPicker.Value = Convert.ToDateTime(rowView("start_time"))
                End If
                SetNullablePicker(_plannedEndPicker, rowView("planned_end_time"))
                SetNullablePicker(_actualEndPicker, rowView("actual_end_time"))
            Finally
                _isLoadingSelection = False
            End Try

            Recalculate()
        End Sub

        Private Sub SaveButton_Click(sender As Object, e As EventArgs)
            Try
                Dim values = CollectValues()
                If values Is Nothing Then
                    Return
                End If

                If _idTextBox.Text.Trim() = "" Then
                    _repository.Insert(values)
                    _statusLabel.ForeColor = AppTheme.Success
                    _statusLabel.Text = "Sewa berhasil ditambahkan."
                Else
                    _repository.Update(Long.Parse(_idTextBox.Text), values)
                    _statusLabel.ForeColor = AppTheme.Success
                    _statusLabel.Text = "Sewa berhasil diperbarui."
                End If

                LoadData()
            Catch ex As Exception
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Gagal simpan sewa: " & ex.Message
            End Try
        End Sub

        Private Function CollectValues() As RentalValues
            If _rentalNoTextBox.Text.Trim() = "" OrElse _customerComboBox.SelectedValue Is Nothing OrElse _roomComboBox.SelectedValue Is Nothing OrElse _consoleComboBox.SelectedValue Is Nothing Then
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "No sewa, pelanggan, ruang, dan PS wajib diisi."
                Return Nothing
            End If

            Recalculate()
            Return New RentalValues With {
                .RentalNo = _rentalNoTextBox.Text.Trim(),
                .BookingId = NullableLong(_bookingComboBox.SelectedValue),
                .CustomerId = Convert.ToInt64(_customerComboBox.SelectedValue),
                .RoomId = Convert.ToInt64(_roomComboBox.SelectedValue),
                .ConsoleId = Convert.ToInt64(_consoleComboBox.SelectedValue),
                .StartTime = _startPicker.Value,
                .PlannedEndTime = NullableDate(_plannedEndPicker),
                .ActualEndTime = NullableDate(_actualEndPicker),
                .HourlyRate = ParseDecimal(_hourlyRateTextBox.Text),
                .DurationMinutes = CInt(ParseDecimal(_durationTextBox.Text)),
                .RentalAmount = ParseDecimal(_rentalAmountTextBox.Text),
                .DiscountAmount = ParseDecimal(_discountAmountTextBox.Text),
                .TotalAmount = ParseDecimal(_totalTextBox.Text),
                .PaidAmount = ParseDecimal(_paidTextBox.Text),
                .Status = If(_statusComboBox.SelectedItem Is Nothing, "running", _statusComboBox.SelectedItem.ToString()),
                .Notes = _notesTextBox.Text
            }
        End Function

        Private Sub Recalculate()
            If _isLoadingSelection Then
                Return
            End If

            Dim endTime = If(_actualEndPicker.Checked, _actualEndPicker.Value, If(_plannedEndPicker.Checked, _plannedEndPicker.Value, DateTime.Now))
            Dim minutes = Math.Max(0, CInt((endTime - _startPicker.Value).TotalMinutes))
            Dim hourlyRate = ParseDecimal(_hourlyRateTextBox.Text)
            Dim rentalAmount = Math.Round((hourlyRate / 60D) * minutes, 0)
            Dim discountPercent = Math.Min(100D, Math.Max(0D, ParseDecimal(_discountPercentTextBox.Text)))
            Dim discountAmount = Math.Round(rentalAmount * discountPercent / 100D, 0)
            Dim total = Math.Max(0D, rentalAmount - discountAmount)

            _durationTextBox.Text = minutes.ToString()
            _rentalAmountTextBox.Text = rentalAmount.ToString("N0")
            _discountAmountTextBox.Text = discountAmount.ToString("N0")
            _totalTextBox.Text = total.ToString("N0")
        End Sub

        Private Sub ClearForm()
            _idTextBox.Clear()
            _rentalNoTextBox.Clear()
            _startPicker.Value = DateTime.Now
            _plannedEndPicker.Value = DateTime.Now.AddHours(1)
            _plannedEndPicker.Checked = True
            _actualEndPicker.Checked = False
            _hourlyRateTextBox.Text = "0"
            _discountPercentTextBox.Text = "0"
            _paidTextBox.Text = "0"
            _notesTextBox.Clear()
            _statusComboBox.SelectedIndex = 0
            Recalculate()
        End Sub

        Private Sub HideInternalColumns()
            For Each name In New String() {"id", "booking_id", "customer_id", "room_id", "console_id"}
                If _grid.Columns.Contains(name) Then
                    _grid.Columns(name).Visible = False
                End If
            Next
        End Sub

        Private Shared Sub ConfigureCombo(combo As ComboBox)
            combo.DropDownStyle = ComboBoxStyle.DropDownList
            combo.Height = 28
        End Sub

        Private Shared Sub ConfigurePicker(picker As DateTimePicker, nullable As Boolean)
            picker.Height = 28
            picker.Format = DateTimePickerFormat.Custom
            picker.CustomFormat = "dd/MM/yyyy HH:mm"
            picker.ShowCheckBox = nullable
            picker.Checked = Not nullable
        End Sub

        Private Shared Sub MakeReadOnly(textBox As TextBox)
            textBox.ReadOnly = True
            textBox.BackColor = Color.FromArgb(248, 250, 252)
        End Sub

        Private Shared Sub AddText(parent As FlowLayoutPanel, labelText As String, textBox As TextBox)
            AddControl(parent, labelText, textBox)
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
            If String.IsNullOrWhiteSpace(text) Then
                Return 0D
            End If

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

        Private Shared Function NullableDate(picker As DateTimePicker) As DateTime?
            If picker.ShowCheckBox AndAlso Not picker.Checked Then
                Return Nothing
            End If

            Return picker.Value
        End Function

        Private Shared Sub SetNullablePicker(picker As DateTimePicker, value As Object)
            If value Is DBNull.Value Then
                picker.Checked = False
                Return
            End If

            picker.Value = Convert.ToDateTime(value)
            picker.Checked = True
        End Sub

        Private Shared Sub SetComboValue(combo As ComboBox, value As Object)
            If value Is DBNull.Value Then
                combo.SelectedIndex = 0
                Return
            End If

            combo.SelectedValue = value
        End Sub
    End Class
End Namespace
