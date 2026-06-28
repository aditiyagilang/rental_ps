Imports System.Drawing
Imports System.Windows.Forms
Imports RentalPS.WinForms.Repositories

Namespace RentalPS.WinForms.UI
    Public Class FrmDashboard
        Inherits Form

        Private ReadOnly _repository As New DashboardRepository()
        Private ReadOnly _runningRentalLabel As New Label()
        Private ReadOnly _todayBookingLabel As New Label()
        Private ReadOnly _lowStockLabel As New Label()
        Private ReadOnly _revenueLabel As New Label()
        Private ReadOnly _statusLabel As New Label()
        Private ReadOnly _revenueChart As New SimpleBarChart()
        Private ReadOnly _transactionChart As New SimpleDonutChart()
        Private ReadOnly _stockChart As New SimpleBarChart()

        Public Sub New()
            BackColor = AppTheme.Surface
            Font = New Font("Segoe UI", 9.0F)
            BuildLayout()

            If DesignModeHelper.IsDesignMode() Then
                _statusLabel.ForeColor = AppTheme.TextMuted
                _statusLabel.Text = "Preview designer. Data dashboard muncul saat aplikasi dijalankan."
            Else
                LoadMetrics()
            End If
        End Sub

        Private Sub BuildLayout()
            Dim root = New TableLayoutPanel With {
                .Dock = DockStyle.Fill,
                .ColumnCount = 4,
                .RowCount = 4,
                .BackColor = AppTheme.Surface
            }
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25))
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25))
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25))
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25))
            root.RowStyles.Add(New RowStyle(SizeType.Absolute, 112))
            root.RowStyles.Add(New RowStyle(SizeType.Absolute, 16))
            root.RowStyles.Add(New RowStyle(SizeType.Percent, 56))
            root.RowStyles.Add(New RowStyle(SizeType.Percent, 44))

            root.Controls.Add(CreateMetricCard("Sewa Berjalan", _runningRentalLabel, AppTheme.Accent), 0, 0)
            root.Controls.Add(CreateMetricCard("Booking Hari Ini", _todayBookingLabel, AppTheme.Success), 1, 0)
            root.Controls.Add(CreateMetricCard("Stok Menipis", _lowStockLabel, Color.FromArgb(234, 88, 12)), 2, 0)
            root.Controls.Add(CreateMetricCard("Pendapatan Hari Ini", _revenueLabel, Color.FromArgb(147, 51, 234)), 3, 0)

            _revenueChart.Dock = DockStyle.Fill
            _revenueChart.Title = "Pendapatan 7 Hari"
            _revenueChart.BarColor = AppTheme.Accent
            _revenueChart.Margin = New Padding(0, 0, 12, 12)
            root.SetColumnSpan(_revenueChart, 2)
            root.Controls.Add(_revenueChart, 0, 2)

            _transactionChart.Dock = DockStyle.Fill
            _transactionChart.Title = "Komposisi Transaksi Hari Ini"
            _transactionChart.Margin = New Padding(0, 0, 0, 12)
            root.SetColumnSpan(_transactionChart, 2)
            root.Controls.Add(_transactionChart, 2, 2)

            _stockChart.Dock = DockStyle.Fill
            _stockChart.Title = "Risiko Operasional"
            _stockChart.BarColor = Color.FromArgb(234, 88, 12)
            _stockChart.Margin = New Padding(0, 0, 12, 0)
            root.SetColumnSpan(_stockChart, 2)
            root.Controls.Add(_stockChart, 0, 3)

            Dim summaryPanel = New Panel With {
                .Dock = DockStyle.Fill,
                .BackColor = Color.White,
                .Padding = New Padding(18),
                .Margin = New Padding(0)
            }
            root.SetColumnSpan(summaryPanel, 2)

            Dim title = New Label With {
                .Dock = DockStyle.Top,
                .Height = 34,
                .Text = "Kontrol Cepat",
                .Font = New Font("Segoe UI", 12.0F, FontStyle.Bold),
                .ForeColor = AppTheme.TextDark
            }

            _statusLabel.Dock = DockStyle.Top
            _statusLabel.Height = 34
            _statusLabel.ForeColor = AppTheme.TextMuted

            Dim refreshButton = New Button With {.Text = "Refresh Dashboard", .Width = 160, .Dock = DockStyle.Top}
            AppTheme.StylePrimaryButton(refreshButton)
            AddHandler refreshButton.Click, Sub(sender, e) LoadMetrics()

            Dim body = New Label With {
                .Dock = DockStyle.Top,
                .Height = 82,
                .Text = "Pantau transaksi berjalan, booking hari ini, pendapatan, dan stok bermasalah dari satu layar.",
                .ForeColor = AppTheme.TextMuted
            }

            summaryPanel.Controls.Add(refreshButton)
            summaryPanel.Controls.Add(_statusLabel)
            summaryPanel.Controls.Add(body)
            summaryPanel.Controls.Add(title)
            root.Controls.Add(summaryPanel, 2, 3)

            Controls.Add(root)
        End Sub

        Private Function CreateMetricCard(title As String, valueLabel As Label, accentColor As Color) As Control
            Dim panel = New Panel With {
                .Dock = DockStyle.Fill,
                .BackColor = Color.White,
                .Padding = New Padding(16),
                .Margin = New Padding(0, 0, 12, 0)
            }

            Dim accent = New Panel With {.Dock = DockStyle.Left, .Width = 5, .BackColor = accentColor}

            Dim titleLabel = New Label With {
                .Dock = DockStyle.Top,
                .Height = 26,
                .Text = title,
                .ForeColor = AppTheme.TextMuted
            }

            valueLabel.Dock = DockStyle.Fill
            valueLabel.Text = "-"
            valueLabel.Font = New Font("Segoe UI", 22.0F, FontStyle.Bold)
            valueLabel.ForeColor = AppTheme.TextDark
            valueLabel.TextAlign = ContentAlignment.MiddleLeft

            panel.Controls.Add(valueLabel)
            panel.Controls.Add(titleLabel)
            panel.Controls.Add(accent)
            Return panel
        End Function

        Private Sub LoadMetrics()
            Try
                _runningRentalLabel.Text = _repository.GetMetric("SELECT COUNT(*) FROM rentals WHERE status = 'running'").ToString()
                _todayBookingLabel.Text = _repository.GetMetric("SELECT COUNT(*) FROM bookings WHERE CAST(start_time AS date) = CAST(GETDATE() AS date) AND status IN ('booked','checked_in')").ToString()
                _lowStockLabel.Text = _repository.GetMetric("SELECT (SELECT COUNT(*) FROM fnb_items WHERE stock_qty <= minimum_stock) + (SELECT COUNT(*) FROM spareparts WHERE stock_qty <= minimum_stock)").ToString()
                _revenueLabel.Text = _repository.GetRevenueToday().ToString("N0")

                _revenueChart.Items = _repository.GetRevenueLast7Days()
                _transactionChart.Items = _repository.GetTransactionMixToday()
                _stockChart.Items = _repository.GetStockRisk()
                _revenueChart.Invalidate()
                _transactionChart.Invalidate()
                _stockChart.Invalidate()

                _statusLabel.ForeColor = AppTheme.Success
                _statusLabel.Text = "Database tersambung. Dashboard diperbarui."
            Catch ex As Exception
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Gagal membaca dashboard: " & ex.Message
            End Try
        End Sub
    End Class
End Namespace
