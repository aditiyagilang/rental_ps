Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms
Imports RentalPS.WinForms.Repositories

Namespace RentalPS.WinForms.UI
    Public Class FrmReports
        Inherits Form

        Private ReadOnly _repository As New ReportRepository()
        Private ReadOnly _reportComboBox As New ComboBox()
        Private ReadOnly _startPicker As New DateTimePicker()
        Private ReadOnly _endPicker As New DateTimePicker()
        Private ReadOnly _grid As New DataGridView()
        Private ReadOnly _summaryLabel As New Label()

        Public Sub New()
            BackColor = AppTheme.Surface
            Font = New Font("Segoe UI", 9.0F)
            BuildLayout()

            If Not DesignModeHelper.IsDesignMode() Then
                LoadReport()
            End If
        End Sub

        Private Sub BuildLayout()
            Dim root = New Panel With {.Dock = DockStyle.Fill, .BackColor = Color.White, .Padding = New Padding(16)}
            Dim toolbar = New FlowLayoutPanel With {.Dock = DockStyle.Top, .Height = 52, .FlowDirection = FlowDirection.LeftToRight, .WrapContents = False}

            _reportComboBox.DropDownStyle = ComboBoxStyle.DropDownList
            _reportComboBox.Width = 180
            _reportComboBox.Items.AddRange(New Object() {"Pendapatan", "Sewa", "Booking", "Service", "Stok Menipis", "Pembayaran"})
            _reportComboBox.SelectedIndex = 0

            _startPicker.Format = DateTimePickerFormat.Short
            _startPicker.Width = 120
            _startPicker.Value = Date.Today
            _endPicker.Format = DateTimePickerFormat.Short
            _endPicker.Width = 120
            _endPicker.Value = Date.Today

            Dim showButton = New Button With {.Text = "Tampilkan", .Width = 120}
            AppTheme.StylePrimaryButton(showButton)
            AddHandler showButton.Click, Sub(sender, e) LoadReport()

            toolbar.Controls.Add(CreateLabel("Laporan"))
            toolbar.Controls.Add(_reportComboBox)
            toolbar.Controls.Add(CreateLabel("Dari"))
            toolbar.Controls.Add(_startPicker)
            toolbar.Controls.Add(CreateLabel("Sampai"))
            toolbar.Controls.Add(_endPicker)
            toolbar.Controls.Add(showButton)

            _summaryLabel.Dock = DockStyle.Bottom
            _summaryLabel.Height = 40
            _summaryLabel.ForeColor = AppTheme.TextDark
            _summaryLabel.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)

            AppTheme.StyleGrid(_grid)
            _grid.Dock = DockStyle.Fill

            root.Controls.Add(_grid)
            root.Controls.Add(_summaryLabel)
            root.Controls.Add(toolbar)
            Controls.Add(root)
        End Sub

        Private Shared Function CreateLabel(text As String) As Label
            Return New Label With {.Text = text, .Width = 62, .Height = 34, .TextAlign = ContentAlignment.MiddleLeft, .ForeColor = AppTheme.TextMuted}
        End Function

        Private Sub LoadReport()
            Try
                Dim table = _repository.LoadReport(_reportComboBox.Text, _startPicker.Value.Date, _endPicker.Value.Date)
                _grid.DataSource = table
                GridHeaderHelper.Apply(_grid)
                _summaryLabel.Text = BuildSummary(table)
            Catch ex As Exception
                _summaryLabel.ForeColor = AppTheme.Danger
                _summaryLabel.Text = "Gagal load laporan: " & ex.Message
            End Try
        End Sub

        Private Shared Function BuildSummary(table As DataTable) As String
            Dim total As Decimal = 0D
            For Each columnName In New String() {"amount", "total_amount", "paid_amount", "deposit_amount"}
                If table.Columns.Contains(columnName) Then
                    For Each row As DataRow In table.Rows
                        If row(columnName) IsNot DBNull.Value Then
                            total += Convert.ToDecimal(row(columnName))
                        End If
                    Next
                    Return table.Rows.Count.ToString() & " baris | Total: " & total.ToString("N0")
                End If
            Next

            Return table.Rows.Count.ToString() & " baris"
        End Function
    End Class
End Namespace
