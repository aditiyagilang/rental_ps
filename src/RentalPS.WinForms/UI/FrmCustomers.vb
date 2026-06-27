Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms
Imports RentalPS.WinForms.Repositories

Namespace RentalPS.WinForms.UI
    Public Class FrmCustomers
        Inherits Form

        Private ReadOnly _repository As New CustomerRepository()
        Private ReadOnly _grid As New DataGridView()
        Private ReadOnly _searchTextBox As New TextBox()
        Private ReadOnly _idTextBox As New TextBox()
        Private ReadOnly _codeTextBox As New TextBox()
        Private ReadOnly _nameTextBox As New TextBox()
        Private ReadOnly _phoneTextBox As New TextBox()
        Private ReadOnly _identityTextBox As New TextBox()
        Private ReadOnly _addressTextBox As New TextBox()
        Private ReadOnly _notesTextBox As New TextBox()
        Private ReadOnly _activeCheckBox As New CheckBox()
        Private ReadOnly _statusLabel As New Label()

        Public Sub New()
            BackColor = AppTheme.Surface
            Font = New Font("Segoe UI", 9.0F)
            BuildLayout()

            If DesignModeHelper.IsDesignMode() Then
                _statusLabel.ForeColor = AppTheme.TextMuted
                _statusLabel.Text = "Preview designer. Data pelanggan muncul saat aplikasi dijalankan."
            Else
                LoadCustomers()
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
            _searchTextBox.PlaceholderText = "Cari kode, nama, atau no HP"
            AddHandler _searchTextBox.TextChanged, Sub(sender, e) LoadCustomers()

            Dim refreshButton = New Button With {.Dock = DockStyle.Right, .Width = 98, .Text = "Refresh"}
            AppTheme.StyleSecondaryButton(refreshButton)
            AddHandler refreshButton.Click, Sub(sender, e) LoadCustomers()

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
                .Text = "Data Pelanggan",
                .Font = New Font("Segoe UI", 12.0F, FontStyle.Bold),
                .ForeColor = AppTheme.TextDark
            }

            _idTextBox.Visible = False
            _codeTextBox.PlaceholderText = "CUST001"
            _nameTextBox.PlaceholderText = "Nama pelanggan"
            _phoneTextBox.PlaceholderText = "08xxxxxxxxxx"
            _identityTextBox.PlaceholderText = "No KTP/SIM"
            _addressTextBox.Multiline = True
            _addressTextBox.Height = 58
            _notesTextBox.Multiline = True
            _notesTextBox.Height = 58
            _activeCheckBox.Text = "Aktif"
            _activeCheckBox.Checked = True
            _activeCheckBox.Dock = DockStyle.Top
            _activeCheckBox.Height = 30

            Dim saveButton = New Button With {.Text = "Simpan", .Dock = DockStyle.Top}
            AppTheme.StylePrimaryButton(saveButton)
            AddHandler saveButton.Click, AddressOf SaveButton_Click

            Dim newButton = New Button With {.Text = "Data Baru", .Dock = DockStyle.Top}
            AppTheme.StyleSecondaryButton(newButton)
            AddHandler newButton.Click, Sub(sender, e) ClearForm()

            _statusLabel.Dock = DockStyle.Top
            _statusLabel.Height = 48
            _statusLabel.ForeColor = AppTheme.TextMuted

            formPanel.Controls.Add(_statusLabel)
            formPanel.Controls.Add(newButton)
            formPanel.Controls.Add(CreateSpacer(8))
            formPanel.Controls.Add(saveButton)
            formPanel.Controls.Add(CreateSpacer(12))
            formPanel.Controls.Add(_activeCheckBox)
            AddTextBox(formPanel, "Catatan", _notesTextBox)
            AddTextBox(formPanel, "Alamat", _addressTextBox)
            AddTextBox(formPanel, "No Identitas", _identityTextBox)
            AddTextBox(formPanel, "No HP", _phoneTextBox)
            AddTextBox(formPanel, "Nama", _nameTextBox)
            AddTextBox(formPanel, "Kode", _codeTextBox)
            formPanel.Controls.Add(_idTextBox)
            formPanel.Controls.Add(title)

            root.Controls.Add(listPanel, 0, 0)
            root.Controls.Add(formPanel, 1, 0)
            Controls.Add(root)
        End Sub

        Private Sub AddTextBox(parent As Control, labelText As String, textBox As TextBox)
            textBox.Dock = DockStyle.Top
            textBox.Margin = New Padding(0)
            parent.Controls.Add(textBox)
            parent.Controls.Add(CreateFieldLabel(labelText))
            parent.Controls.Add(CreateSpacer(8))
        End Sub

        Private Shared Function CreateFieldLabel(text As String) As Control
            Return New Label With {
                .Dock = DockStyle.Top,
                .Height = 24,
                .Text = text,
                .ForeColor = AppTheme.TextDark,
                .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold),
                .TextAlign = ContentAlignment.BottomLeft
            }
        End Function

        Private Shared Function CreateSpacer(height As Integer) As Control
            Return New Panel With {.Dock = DockStyle.Top, .Height = height}
        End Function

        Private Sub LoadCustomers()
            If DesignModeHelper.IsDesignMode() Then
                Return
            End If

            Try
                Dim table = _repository.Search(_searchTextBox.Text)
                _grid.DataSource = table
                GridHeaderHelper.Apply(_grid)
                _statusLabel.ForeColor = AppTheme.TextMuted
                _statusLabel.Text = table.Rows.Count.ToString() & " data pelanggan."
            Catch ex As Exception
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Gagal load pelanggan: " & ex.Message
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
            _codeTextBox.Text = rowView("code").ToString()
            _nameTextBox.Text = rowView("name").ToString()
            _phoneTextBox.Text = rowView("phone").ToString()
            _identityTextBox.Text = rowView("identity_number").ToString()
            _addressTextBox.Text = rowView("address").ToString()
            _notesTextBox.Text = rowView("notes").ToString()
            _activeCheckBox.Checked = Convert.ToBoolean(rowView("is_active"))
        End Sub

        Private Sub SaveButton_Click(sender As Object, e As EventArgs)
            Try
                If _codeTextBox.Text.Trim() = "" OrElse _nameTextBox.Text.Trim() = "" Then
                    _statusLabel.ForeColor = AppTheme.Danger
                    _statusLabel.Text = "Kode dan nama wajib diisi."
                    Return
                End If

                If _idTextBox.Text.Trim() = "" Then
                    _repository.Insert(_codeTextBox.Text, _nameTextBox.Text, _phoneTextBox.Text, _addressTextBox.Text, _identityTextBox.Text, _notesTextBox.Text)
                    _statusLabel.ForeColor = AppTheme.Success
                    _statusLabel.Text = "Pelanggan berhasil ditambahkan."
                Else
                    _repository.Update(Long.Parse(_idTextBox.Text), _codeTextBox.Text, _nameTextBox.Text, _phoneTextBox.Text, _addressTextBox.Text, _identityTextBox.Text, _notesTextBox.Text, _activeCheckBox.Checked)
                    _statusLabel.ForeColor = AppTheme.Success
                    _statusLabel.Text = "Pelanggan berhasil diperbarui."
                End If

                LoadCustomers()
            Catch ex As Exception
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Gagal simpan: " & ex.Message
            End Try
        End Sub

        Private Sub ClearForm()
            _idTextBox.Clear()
            _codeTextBox.Clear()
            _nameTextBox.Clear()
            _phoneTextBox.Clear()
            _identityTextBox.Clear()
            _addressTextBox.Clear()
            _notesTextBox.Clear()
            _activeCheckBox.Checked = True
            _codeTextBox.Focus()
        End Sub
    End Class
End Namespace
