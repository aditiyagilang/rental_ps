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
                .RowCount = 3,
                .BackColor = AppTheme.Surface
            }
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25))
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25))
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25))
            root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25))
            root.RowStyles.Add(New RowStyle(SizeType.Absolute, 120))
            root.RowStyles.Add(New RowStyle(SizeType.Absolute, 16))
            root.RowStyles.Add(New RowStyle(SizeType.Percent, 100))

            root.Controls.Add(CreateMetricCard("Sewa Berjalan", _runningRentalLabel), 0, 0)
            root.Controls.Add(CreateMetricCard("Booking Hari Ini", _todayBookingLabel), 1, 0)
            root.Controls.Add(CreateMetricCard("Stok Menipis", _lowStockLabel), 2, 0)
            root.Controls.Add(CreateMetricCard("Pendapatan Hari Ini", _revenueLabel), 3, 0)

            Dim activityPanel = New Panel With {
                .Dock = DockStyle.Fill,
                .BackColor = Color.White,
                .Padding = New Padding(18)
            }
            root.SetColumnSpan(activityPanel, 4)

            Dim title = New Label With {
                .Dock = DockStyle.Top,
                .Height = 34,
                .Text = "Ringkasan Operasional",
                .Font = New Font("Segoe UI", 12.0F, FontStyle.Bold),
                .ForeColor = AppTheme.TextDark
            }

            Dim body = New Label With {
                .Dock = DockStyle.Top,
                .Height = 100,
                .Text = "Gunakan menu kiri untuk transaksi sewa, booking, service, isi game, dan master data. Dashboard ini sudah membaca data langsung dari MySQL.",
                .ForeColor = AppTheme.TextMuted
            }

            _statusLabel.Dock = DockStyle.Top
            _statusLabel.Height = 32
            _statusLabel.ForeColor = AppTheme.TextMuted

            Dim refreshButton = New Button With {.Text = "Refresh", .Width = 110, .Dock = DockStyle.Top}
            AppTheme.StyleSecondaryButton(refreshButton)
            AddHandler refreshButton.Click, Sub(sender, e) LoadMetrics()

            activityPanel.Controls.Add(refreshButton)
            activityPanel.Controls.Add(_statusLabel)
            activityPanel.Controls.Add(body)
            activityPanel.Controls.Add(title)
            root.Controls.Add(activityPanel, 0, 2)

            Controls.Add(root)
        End Sub

        Private Function CreateMetricCard(title As String, valueLabel As Label) As Control
            Dim panel = New Panel With {
                .Dock = DockStyle.Fill,
                .BackColor = Color.White,
                .Padding = New Padding(16),
                .Margin = New Padding(0, 0, 12, 0)
            }

            Dim titleLabel = New Label With {
                .Dock = DockStyle.Top,
                .Height = 28,
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
            Return panel
        End Function

        Private Sub LoadMetrics()
            Try
                _runningRentalLabel.Text = _repository.GetMetric("SELECT COUNT(*) FROM rentals WHERE status = 'running'").ToString()
                _todayBookingLabel.Text = _repository.GetMetric("SELECT COUNT(*) FROM bookings WHERE DATE(start_time) = CURRENT_DATE() AND status IN ('booked','checked_in')").ToString()
                _lowStockLabel.Text = _repository.GetMetric("SELECT (SELECT COUNT(*) FROM fnb_items WHERE stock_qty <= minimum_stock) + (SELECT COUNT(*) FROM spareparts WHERE stock_qty <= minimum_stock)").ToString()
                _revenueLabel.Text = _repository.GetRevenueToday().ToString("N0")
                _statusLabel.ForeColor = AppTheme.Success
                _statusLabel.Text = "Database tersambung."
            Catch ex As Exception
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Gagal membaca dashboard: " & ex.Message
            End Try
        End Sub
    End Class
End Namespace
