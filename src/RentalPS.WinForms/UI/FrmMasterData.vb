Imports System.Collections.Generic
Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms
Imports RentalPS.WinForms.Models
Imports RentalPS.WinForms.Repositories

Namespace RentalPS.WinForms.UI
    Public Class FrmMasterData
        Inherits Form

        Private ReadOnly _config As MasterFormConfig
        Private ReadOnly _repository As New MasterDataRepository()
        Private ReadOnly _grid As New DataGridView()
        Private ReadOnly _searchTextBox As New TextBox()
        Private ReadOnly _statusLabel As New Label()
        Private ReadOnly _idTextBox As New TextBox()
        Private ReadOnly _inputs As New Dictionary(Of String, Control)()

        Public Sub New(config As MasterFormConfig)
            _config = config
            BackColor = AppTheme.Surface
            Font = New Font("Segoe UI", 9.0F)

            BuildLayout()

            If DesignModeHelper.IsDesignMode() Then
                _statusLabel.Text = "Preview designer. Data muncul saat aplikasi dijalankan."
            Else
                LoadData()
            End If
        End Sub

        Private Sub BuildLayout()
            Dim root = New TableLayoutPanel With {
                .Dock = DockStyle.Fill,
                .ColumnCount = 2,
                .RowCount = 1,
                .BackColor = AppTheme.Surface
            }
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 64))
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 36))

            Dim listPanel = New Panel With {
                .Dock = DockStyle.Fill,
                .BackColor = Color.White,
                .Padding = New Padding(16),
                .Margin = New Padding(0, 0, 14, 0)
            }

            Dim toolbar = New Panel With {.Dock = DockStyle.Top, .Height = 48}
            _searchTextBox.Dock = DockStyle.Fill
            _searchTextBox.PlaceholderText = "Cari data"
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

            Dim formPanel = New Panel With {
                .Dock = DockStyle.Fill,
                .BackColor = Color.White,
                .Padding = New Padding(16)
            }

            Dim title = New Label With {
                .Dock = DockStyle.Top,
                .Height = 34,
                .Text = _config.Title,
                .Font = New Font("Segoe UI", 12.0F, FontStyle.Bold),
                .ForeColor = AppTheme.TextDark
            }

            Dim formFlow = New FlowLayoutPanel With {
                .Dock = DockStyle.Fill,
                .FlowDirection = FlowDirection.TopDown,
                .WrapContents = False,
                .AutoScroll = True
            }

            _idTextBox.Visible = False
            formFlow.Controls.Add(_idTextBox)

            For Each field In _config.Fields
                AddField(formFlow, field)
            Next

            Dim saveButton = New Button With {.Text = "Simpan", .Width = 320}
            AppTheme.StylePrimaryButton(saveButton)
            AddHandler saveButton.Click, AddressOf SaveButton_Click

            Dim newButton = New Button With {.Text = "Data Baru", .Width = 320}
            AppTheme.StyleSecondaryButton(newButton)
            AddHandler newButton.Click, Sub(sender, e) ClearForm()

            _statusLabel.Width = 320
            _statusLabel.Height = 48
            _statusLabel.ForeColor = AppTheme.TextMuted

            formFlow.Controls.Add(saveButton)
            formFlow.Controls.Add(newButton)
            formFlow.Controls.Add(_statusLabel)

            formPanel.Controls.Add(formFlow)
            formPanel.Controls.Add(title)

            root.Controls.Add(listPanel, 0, 0)
            root.Controls.Add(formPanel, 1, 0)
            Controls.Add(root)
        End Sub

        Private Sub AddField(parent As FlowLayoutPanel, field As MasterField)
            Dim container = New Panel With {.Width = 320, .Height = If(field.Kind = MasterFieldKind.MultilineText, 98, 68)}

            Dim label = New Label With {
                .Dock = DockStyle.Top,
                .Height = 24,
                .Text = If(field.Required, field.LabelText & " *", field.LabelText),
                .ForeColor = AppTheme.TextDark,
                .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
            }

            Dim input = CreateInput(field)
            input.Dock = DockStyle.Top
            container.Controls.Add(input)
            container.Controls.Add(label)
            parent.Controls.Add(container)
            _inputs.Add(field.ColumnName, input)
        End Sub

        Private Function CreateInput(field As MasterField) As Control
            Select Case field.Kind
                Case MasterFieldKind.MultilineText
                    Return New TextBox With {.Height = 62, .Multiline = True, .PlaceholderText = field.Placeholder}
                Case MasterFieldKind.IntegerNumber, MasterFieldKind.DecimalNumber, MasterFieldKind.Text
                    Return New TextBox With {.Height = 28, .PlaceholderText = field.Placeholder}
                Case MasterFieldKind.DateValue
                    Return New DateTimePicker With {.Height = 28, .Format = DateTimePickerFormat.Short, .ShowCheckBox = Not field.Required}
                Case MasterFieldKind.DateTimeValue
                    Return New DateTimePicker With {.Height = 28, .Format = DateTimePickerFormat.Custom, .CustomFormat = "dd/MM/yyyy HH:mm", .ShowCheckBox = Not field.Required}
                Case MasterFieldKind.BooleanValue
                    Return New CheckBox With {.Height = 28, .Text = "Ya", .Checked = True}
                Case MasterFieldKind.Combo
                    Dim combo = New ComboBox With {.Height = 28, .DropDownStyle = ComboBoxStyle.DropDownList}
                    combo.Items.AddRange(field.Options.ToArray())
                    If combo.Items.Count > 0 Then
                        combo.SelectedIndex = 0
                    End If
                    Return combo
                Case MasterFieldKind.Lookup
                    Dim combo = New ComboBox With {.Height = 28, .DropDownStyle = ComboBoxStyle.DropDownList}
                    If Not DesignModeHelper.IsDesignMode() Then
                        Dim table = _repository.LoadLookup(field.LookupSql)
                        If Not field.Required Then
                            Dim row = table.NewRow()
                            row(field.LookupValueMember) = DBNull.Value
                            row(field.LookupDisplayMember) = "-"
                            table.Rows.InsertAt(row, 0)
                        End If

                        combo.DataSource = table
                        combo.DisplayMember = field.LookupDisplayMember
                        combo.ValueMember = field.LookupValueMember
                    End If
                    Return combo
                Case Else
                    Return New TextBox With {.Height = 28, .PlaceholderText = field.Placeholder}
            End Select
        End Function

        Private Sub LoadData()
            If DesignModeHelper.IsDesignMode() Then
                Return
            End If

            Try
                Dim table = _repository.Search(_config, _searchTextBox.Text)
                _grid.DataSource = table
                _statusLabel.ForeColor = AppTheme.TextMuted
                _statusLabel.Text = table.Rows.Count.ToString() & " data."
            Catch ex As Exception
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Gagal load: " & ex.Message
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

            _idTextBox.Text = rowView("id").ToString()

            For Each field In _config.Fields
                If rowView.Row.Table.Columns.Contains(field.ColumnName) Then
                    SetInputValue(field, rowView(field.ColumnName))
                End If
            Next
        End Sub

        Private Sub SaveButton_Click(sender As Object, e As EventArgs)
            Try
                Dim values = CollectValues()
                If values Is Nothing Then
                    Return
                End If

                If _idTextBox.Text.Trim() = "" Then
                    _repository.Insert(_config, values)
                    _statusLabel.ForeColor = AppTheme.Success
                    _statusLabel.Text = "Data berhasil ditambahkan."
                Else
                    _repository.Update(_config, Long.Parse(_idTextBox.Text), values)
                    _statusLabel.ForeColor = AppTheme.Success
                    _statusLabel.Text = "Data berhasil diperbarui."
                End If

                LoadData()
            Catch ex As Exception
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Gagal simpan: " & ex.Message
            End Try
        End Sub

        Private Function CollectValues() As Dictionary(Of String, Object)
            Dim values = New Dictionary(Of String, Object)()

            For Each field In _config.Fields
                Dim value = GetInputValue(field)
                If field.Required AndAlso (value Is DBNull.Value OrElse value Is Nothing OrElse value.ToString().Trim() = "") Then
                    _statusLabel.ForeColor = AppTheme.Danger
                    _statusLabel.Text = field.LabelText & " wajib diisi."
                    Return Nothing
                End If

                values.Add(field.ColumnName, value)
            Next

            Return values
        End Function

        Private Function GetInputValue(field As MasterField) As Object
            Dim input = _inputs(field.ColumnName)

            Select Case field.Kind
                Case MasterFieldKind.BooleanValue
                    Return If(DirectCast(input, CheckBox).Checked, 1, 0)
                Case MasterFieldKind.Combo
                    Dim combo = DirectCast(input, ComboBox)
                    If combo.SelectedItem Is Nothing Then
                        Return DBNull.Value
                    End If
                    Return combo.SelectedItem.ToString()
                Case MasterFieldKind.Lookup
                    Dim combo = DirectCast(input, ComboBox)
                    If combo.SelectedValue Is Nothing OrElse combo.SelectedValue Is DBNull.Value Then
                        Return DBNull.Value
                    End If
                    Return combo.SelectedValue
                Case MasterFieldKind.DateValue
                    Dim picker = DirectCast(input, DateTimePicker)
                    If picker.ShowCheckBox AndAlso Not picker.Checked Then
                        Return DBNull.Value
                    End If
                    Return picker.Value.Date
                Case MasterFieldKind.DateTimeValue
                    Dim picker = DirectCast(input, DateTimePicker)
                    If picker.ShowCheckBox AndAlso Not picker.Checked Then
                        Return DBNull.Value
                    End If
                    Return picker.Value
                Case MasterFieldKind.IntegerNumber
                    Dim text = DirectCast(input, TextBox).Text.Trim()
                    If text = "" Then
                        Return 0
                    End If
                    Return Integer.Parse(text)
                Case MasterFieldKind.DecimalNumber
                    Dim text = DirectCast(input, TextBox).Text.Trim()
                    If text = "" Then
                        Return 0D
                    End If
                    Return Decimal.Parse(text)
                Case Else
                    Dim text = DirectCast(input, TextBox).Text.Trim()
                    If text = "" AndAlso Not field.Required Then
                        Return DBNull.Value
                    End If
                    Return text
            End Select
        End Function

        Private Sub SetInputValue(field As MasterField, value As Object)
            Dim input = _inputs(field.ColumnName)

            If value Is DBNull.Value Then
                ClearInput(field)
                Return
            End If

            Select Case field.Kind
                Case MasterFieldKind.BooleanValue
                    DirectCast(input, CheckBox).Checked = Convert.ToBoolean(value)
                Case MasterFieldKind.Combo
                    DirectCast(input, ComboBox).SelectedItem = value.ToString()
                Case MasterFieldKind.Lookup
                    DirectCast(input, ComboBox).SelectedValue = value
                Case MasterFieldKind.DateValue, MasterFieldKind.DateTimeValue
                    Dim picker = DirectCast(input, DateTimePicker)
                    picker.Value = Convert.ToDateTime(value)
                    If picker.ShowCheckBox Then
                        picker.Checked = True
                    End If
                Case Else
                    DirectCast(input, TextBox).Text = value.ToString()
            End Select
        End Sub

        Private Sub ClearForm()
            _idTextBox.Clear()

            For Each field In _config.Fields
                ClearInput(field)
            Next
        End Sub

        Private Sub ClearInput(field As MasterField)
            Dim input = _inputs(field.ColumnName)

            Select Case field.Kind
                Case MasterFieldKind.BooleanValue
                    DirectCast(input, CheckBox).Checked = True
                Case MasterFieldKind.Combo
                    Dim combo = DirectCast(input, ComboBox)
                    If combo.Items.Count > 0 Then
                        combo.SelectedIndex = 0
                    End If
                Case MasterFieldKind.Lookup
                    Dim combo = DirectCast(input, ComboBox)
                    If combo.Items.Count > 0 Then
                        combo.SelectedIndex = 0
                    End If
                Case MasterFieldKind.DateValue, MasterFieldKind.DateTimeValue
                    Dim picker = DirectCast(input, DateTimePicker)
                    picker.Value = Date.Today
                    If picker.ShowCheckBox Then
                        picker.Checked = False
                    End If
                Case Else
                    DirectCast(input, TextBox).Clear()
            End Select
        End Sub
    End Class
End Namespace
