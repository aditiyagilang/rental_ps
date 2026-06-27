Imports System.Collections.Generic
Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms
Imports RentalPS.WinForms.Infrastructure
Imports RentalPS.WinForms.Repositories

Namespace RentalPS.WinForms.UI
    Public Class FrmUsers
        Inherits Form

        Private ReadOnly _repository As New UserRepository()
        Private ReadOnly _grid As New DataGridView()
        Private ReadOnly _searchTextBox As New TextBox()
        Private ReadOnly _idTextBox As New TextBox()
        Private ReadOnly _usernameTextBox As New TextBox()
        Private ReadOnly _passwordTextBox As New TextBox()
        Private ReadOnly _fullNameTextBox As New TextBox()
        Private ReadOnly _activeCheckBox As New CheckBox()
        Private ReadOnly _rolesPanel As New FlowLayoutPanel()
        Private ReadOnly _statusLabel As New Label()
        Private ReadOnly _togglePasswordButton As New Button()

        Public Sub New()
            BackColor = AppTheme.Surface
            Font = New Font("Segoe UI", 9.0F)
            BuildLayout()

            If Not DesignModeHelper.IsDesignMode() Then
                LoadRoles()
                LoadUsers()
            End If
        End Sub

        Private Sub BuildLayout()
            Dim root = New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 1, .BackColor = AppTheme.Surface}
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 64))
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 36))

            Dim listPanel = New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.White, .Padding = New Padding(16), .Margin = New Padding(0, 0, 14, 0)}
            Dim toolbar = New Panel With {.Dock = DockStyle.Top, .Height = 48}
            _searchTextBox.Dock = DockStyle.Fill
            _searchTextBox.PlaceholderText = "Cari username atau nama"
            AddHandler _searchTextBox.TextChanged, Sub(sender, e) LoadUsers()
            toolbar.Controls.Add(_searchTextBox)

            AppTheme.StyleGrid(_grid)
            _grid.Dock = DockStyle.Fill
            AddHandler _grid.SelectionChanged, AddressOf Grid_SelectionChanged
            listPanel.Controls.Add(_grid)
            listPanel.Controls.Add(toolbar)

            Dim formPanel = New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.White, .Padding = New Padding(16)}
            Dim title = New Label With {.Dock = DockStyle.Top, .Height = 34, .Text = "CRUD User", .Font = New Font("Segoe UI", 12.0F, FontStyle.Bold), .ForeColor = AppTheme.TextDark}
            Dim flow = New FlowLayoutPanel With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False, .AutoScroll = True}

            _idTextBox.Visible = False
            flow.Controls.Add(_idTextBox)
            AddText(flow, "Username *", _usernameTextBox)
            AddPassword(flow)
            AddText(flow, "Nama Lengkap *", _fullNameTextBox)

            _activeCheckBox.Text = "Aktif"
            _activeCheckBox.Checked = True
            _activeCheckBox.Width = 320
            flow.Controls.Add(_activeCheckBox)

            Dim roleLabel = New Label With {.Text = "Role *", .Width = 320, .Height = 24, .ForeColor = AppTheme.TextDark, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
            _rolesPanel.Width = 320
            _rolesPanel.Height = 92
            _rolesPanel.FlowDirection = FlowDirection.TopDown
            _rolesPanel.WrapContents = False
            _rolesPanel.AutoScroll = True
            flow.Controls.Add(roleLabel)
            flow.Controls.Add(_rolesPanel)

            Dim saveButton = New Button With {.Text = "Simpan", .Width = 320}
            AppTheme.StylePrimaryButton(saveButton)
            AddHandler saveButton.Click, AddressOf SaveButton_Click

            Dim newButton = New Button With {.Text = "Data Baru", .Width = 320}
            AppTheme.StyleSecondaryButton(newButton)
            AddHandler newButton.Click, Sub(sender, e) ClearForm()

            Dim deleteButton = New Button With {.Text = "Hapus", .Width = 320}
            AppTheme.StyleSecondaryButton(deleteButton)
            deleteButton.ForeColor = AppTheme.Danger
            AddHandler deleteButton.Click, AddressOf DeleteButton_Click

            _statusLabel.Width = 320
            _statusLabel.Height = 58
            _statusLabel.ForeColor = AppTheme.TextMuted

            flow.Controls.Add(saveButton)
            flow.Controls.Add(newButton)
            flow.Controls.Add(deleteButton)
            flow.Controls.Add(_statusLabel)
            formPanel.Controls.Add(flow)
            formPanel.Controls.Add(title)

            root.Controls.Add(listPanel, 0, 0)
            root.Controls.Add(formPanel, 1, 0)
            Controls.Add(root)
        End Sub

        Private Sub AddPassword(parent As FlowLayoutPanel)
            Dim container = New Panel With {.Width = 320, .Height = 68}
            Dim label = New Label With {.Dock = DockStyle.Top, .Height = 24, .Text = "Password *", .ForeColor = AppTheme.TextDark, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
            Dim inputPanel = New Panel With {.Dock = DockStyle.Top, .Height = 30}
            _togglePasswordButton.Dock = DockStyle.Right
            _togglePasswordButton.Width = 42
            _togglePasswordButton.Text = "👁"
            _togglePasswordButton.FlatStyle = FlatStyle.Flat
            _togglePasswordButton.FlatAppearance.BorderColor = AppTheme.Border
            AddHandler _togglePasswordButton.Click, AddressOf TogglePasswordButton_Click
            _passwordTextBox.Dock = DockStyle.Fill
            _passwordTextBox.UseSystemPasswordChar = True
            _passwordTextBox.PlaceholderText = "Isi untuk password baru"
            inputPanel.Controls.Add(_passwordTextBox)
            inputPanel.Controls.Add(_togglePasswordButton)
            container.Controls.Add(inputPanel)
            container.Controls.Add(label)
            parent.Controls.Add(container)
        End Sub

        Private Shared Sub AddText(parent As FlowLayoutPanel, labelText As String, textBox As TextBox)
            Dim container = New Panel With {.Width = 320, .Height = 68}
            Dim label = New Label With {.Dock = DockStyle.Top, .Height = 24, .Text = labelText, .ForeColor = AppTheme.TextDark, .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)}
            textBox.Dock = DockStyle.Top
            container.Controls.Add(textBox)
            container.Controls.Add(label)
            parent.Controls.Add(container)
        End Sub

        Private Sub LoadRoles()
            _rolesPanel.Controls.Clear()
            Dim table = _repository.LoadRoles()
            For Each row As DataRow In table.Rows
                Dim checkBox = New CheckBox With {
                    .Text = row("name").ToString(),
                    .Tag = Convert.ToInt64(row("id")),
                    .Width = 280
                }
                _rolesPanel.Controls.Add(checkBox)
            Next
        End Sub

        Private Sub LoadUsers()
            If DesignModeHelper.IsDesignMode() Then
                Return
            End If

            Try
                Dim table = _repository.Search(_searchTextBox.Text)
                _grid.DataSource = table
                GridHeaderHelper.Apply(_grid)
                If _grid.Columns.Contains("id") Then
                    _grid.Columns("id").Visible = False
                End If
                _statusLabel.ForeColor = AppTheme.TextMuted
                _statusLabel.Text = table.Rows.Count.ToString() & " user."
            Catch ex As Exception
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Gagal load user: " & ex.Message
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

            Dim userId = Convert.ToInt64(rowView("id"))
            _idTextBox.Text = userId.ToString()
            _usernameTextBox.Text = rowView("username").ToString()
            _passwordTextBox.Clear()
            _fullNameTextBox.Text = rowView("full_name").ToString()
            _activeCheckBox.Checked = Convert.ToBoolean(rowView("is_active"))

            Dim userRoles = _repository.LoadUserRoles(userId)
            For Each control As Control In _rolesPanel.Controls
                Dim checkBox = TryCast(control, CheckBox)
                If checkBox IsNot Nothing Then
                    checkBox.Checked = userRoles.Contains(Convert.ToInt64(checkBox.Tag))
                End If
            Next
        End Sub

        Private Sub SaveButton_Click(sender As Object, e As EventArgs)
            If Not AppSession.IsAdmin Then
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Hanya admin yang boleh CRUD user."
                Return
            End If

            If _usernameTextBox.Text.Trim() = "" OrElse _fullNameTextBox.Text.Trim() = "" Then
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Username dan nama lengkap wajib diisi."
                Return
            End If

            If _idTextBox.Text.Trim() = "" AndAlso _passwordTextBox.Text.Trim() = "" Then
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Password wajib diisi untuk user baru."
                Return
            End If

            Dim roleIds = GetSelectedRoleIds()
            If roleIds.Count = 0 Then
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Minimal pilih satu role."
                Return
            End If

            Try
                If _idTextBox.Text.Trim() = "" Then
                    _repository.Insert(_usernameTextBox.Text, _passwordTextBox.Text, _fullNameTextBox.Text, _activeCheckBox.Checked, roleIds)
                    _statusLabel.ForeColor = AppTheme.Success
                    _statusLabel.Text = "User berhasil ditambahkan."
                Else
                    _repository.Update(Long.Parse(_idTextBox.Text), _usernameTextBox.Text, _passwordTextBox.Text, _fullNameTextBox.Text, _activeCheckBox.Checked, roleIds)
                    _statusLabel.ForeColor = AppTheme.Success
                    _statusLabel.Text = "User berhasil diperbarui."
                End If

                ClearForm()
                LoadUsers()
            Catch ex As Exception
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Gagal simpan user: " & ex.Message
            End Try
        End Sub

        Private Sub DeleteButton_Click(sender As Object, e As EventArgs)
            If Not AppSession.IsAdmin Then
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Hanya admin yang boleh hapus user."
                Return
            End If

            If _idTextBox.Text.Trim() = "" Then
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Pilih user yang mau dihapus."
                Return
            End If

            If _usernameTextBox.Text.Trim() = AppSession.Username Then
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "User yang sedang login tidak boleh menghapus dirinya sendiri."
                Return
            End If

            If MessageBox.Show("Nonaktifkan user ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) <> DialogResult.Yes Then
                Return
            End If

            Try
                _repository.Delete(Long.Parse(_idTextBox.Text))
                _statusLabel.ForeColor = AppTheme.Success
                _statusLabel.Text = "User berhasil dinonaktifkan."
                ClearForm()
                LoadUsers()
            Catch ex As Exception
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Gagal hapus user: " & ex.Message
            End Try
        End Sub

        Private Function GetSelectedRoleIds() As List(Of Long)
            Dim roleIds = New List(Of Long)()
            For Each control As Control In _rolesPanel.Controls
                Dim checkBox = TryCast(control, CheckBox)
                If checkBox IsNot Nothing AndAlso checkBox.Checked Then
                    roleIds.Add(Convert.ToInt64(checkBox.Tag))
                End If
            Next
            Return roleIds
        End Function

        Private Sub TogglePasswordButton_Click(sender As Object, e As EventArgs)
            _passwordTextBox.UseSystemPasswordChar = Not _passwordTextBox.UseSystemPasswordChar
            _togglePasswordButton.Text = If(_passwordTextBox.UseSystemPasswordChar, "👁", "×")
        End Sub

        Private Sub ClearForm()
            _idTextBox.Clear()
            _usernameTextBox.Clear()
            _passwordTextBox.Clear()
            _passwordTextBox.UseSystemPasswordChar = True
            _togglePasswordButton.Text = "👁"
            _fullNameTextBox.Clear()
            _activeCheckBox.Checked = True
            For Each control As Control In _rolesPanel.Controls
                Dim checkBox = TryCast(control, CheckBox)
                If checkBox IsNot Nothing Then
                    checkBox.Checked = False
                End If
            Next
        End Sub
    End Class
End Namespace
