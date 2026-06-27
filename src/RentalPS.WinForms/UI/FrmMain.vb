Imports System.Drawing
Imports System.Windows.Forms

Namespace RentalPS.WinForms.UI
    Public Class FrmMain
        Inherits Form

        Private ReadOnly _contentPanel As New Panel()
        Private ReadOnly _titleLabel As New Label()
        Private ReadOnly _username As String

        Public Sub New(username As String)
            _username = username

            Text = "Rental PS"
            WindowState = FormWindowState.Maximized
            MinimumSize = New Size(1100, 720)
            BackColor = AppTheme.Surface
            Font = New Font("Segoe UI", 9.0F)

            BuildLayout()
            ShowPage("Dashboard", New FrmDashboard())
        End Sub

        Private Sub BuildLayout()
            Dim sidebar = New Panel With {
                .Dock = DockStyle.Left,
                .Width = 230,
                .BackColor = AppTheme.SidebarBack,
                .Padding = New Padding(14)
            }

            Dim brand = New Label With {
                .Dock = DockStyle.Top,
                .Height = 70,
                .Text = "Rental PS",
                .ForeColor = Color.White,
                .Font = New Font("Segoe UI", 18.0F, FontStyle.Bold),
                .TextAlign = ContentAlignment.MiddleLeft
            }

            sidebar.Controls.Add(CreateMenuButton("Pengaturan", Sub() ShowPage("Pengaturan", New FrmPlaceholder("Pengaturan"))))
            sidebar.Controls.Add(CreateMenuButton("Laporan", Sub() ShowPage("Laporan", New FrmPlaceholder("Laporan"))))
            sidebar.Controls.Add(CreateMenuButton("Supplier", Sub() ShowPage("Supplier", New FrmSuppliers())))
            sidebar.Controls.Add(CreateMenuButton("Sparepart", Sub() ShowPage("Sparepart", New FrmSpareparts())))
            sidebar.Controls.Add(CreateMenuButton("Jasa", Sub() ShowPage("Jasa", New FrmServices())))
            sidebar.Controls.Add(CreateMenuButton("Game", Sub() ShowPage("Game", New FrmGames())))
            sidebar.Controls.Add(CreateMenuButton("Stok FNB", Sub() ShowPage("Stok FNB", New FrmFnbItems())))
            sidebar.Controls.Add(CreateMenuButton("Ruang", Sub() ShowPage("Ruang", New FrmRooms())))
            sidebar.Controls.Add(CreateMenuButton("Jenis Ruang", Sub() ShowPage("Jenis Ruang", New FrmRoomTypes())))
            sidebar.Controls.Add(CreateMenuButton("Stok PS", Sub() ShowPage("Stok PS", New FrmConsoles())))
            sidebar.Controls.Add(CreateMenuButton("Pelanggan", Sub() ShowPage("Pelanggan", New FrmCustomers())))
            sidebar.Controls.Add(CreateMenuButton("Service", Sub() ShowPage("Service", New FrmPlaceholder("Transaksi Service"))))
            sidebar.Controls.Add(CreateMenuButton("Isi Game", Sub() ShowPage("Isi Game", New FrmPlaceholder("Transaksi Isi Game"))))
            sidebar.Controls.Add(CreateMenuButton("Denda", Sub() ShowPage("Denda", New FrmPlaceholder("Transaksi Denda"))))
            sidebar.Controls.Add(CreateMenuButton("Booking", Sub() ShowPage("Booking", New FrmPlaceholder("Transaksi Booking"))))
            sidebar.Controls.Add(CreateMenuButton("Sewa", Sub() ShowPage("Sewa", New FrmPlaceholder("Transaksi Sewa"))))
            sidebar.Controls.Add(CreateMenuButton("Dashboard", Sub() ShowPage("Dashboard", New FrmDashboard())))
            sidebar.Controls.Add(brand)

            Dim header = New Panel With {
                .Dock = DockStyle.Top,
                .Height = 64,
                .BackColor = Color.White,
                .Padding = New Padding(24, 0, 24, 0)
            }

            _titleLabel.Dock = DockStyle.Left
            _titleLabel.Width = 420
            _titleLabel.Font = New Font("Segoe UI", 16.0F, FontStyle.Bold)
            _titleLabel.ForeColor = AppTheme.TextDark
            _titleLabel.TextAlign = ContentAlignment.MiddleLeft

            Dim userLabel = New Label With {
                .Dock = DockStyle.Right,
                .Width = 220,
                .Text = "User: " & _username,
                .TextAlign = ContentAlignment.MiddleRight,
                .ForeColor = AppTheme.TextMuted
            }

            header.Controls.Add(userLabel)
            header.Controls.Add(_titleLabel)

            _contentPanel.Dock = DockStyle.Fill
            _contentPanel.Padding = New Padding(24)
            _contentPanel.BackColor = AppTheme.Surface

            Controls.Add(_contentPanel)
            Controls.Add(header)
            Controls.Add(sidebar)
        End Sub

        Private Function CreateMenuButton(text As String, action As Action) As Button
            Dim button = New Button With {
                .Dock = DockStyle.Top,
                .Height = 38,
                .Text = text,
                .TextAlign = ContentAlignment.MiddleLeft,
                .FlatStyle = FlatStyle.Flat,
                .BackColor = AppTheme.SidebarBack,
                .ForeColor = Color.White,
                .Font = New Font("Segoe UI", 9.0F),
                .Padding = New Padding(10, 0, 0, 0)
            }

            button.FlatAppearance.BorderSize = 0
            button.FlatAppearance.MouseOverBackColor = AppTheme.SidebarActive
            AddHandler button.Click, Sub(sender, e) action.Invoke()
            Return button
        End Function

        Private Sub ShowPage(title As String, page As Form)
            _titleLabel.Text = title
            _contentPanel.Controls.Clear()

            page.TopLevel = False
            page.FormBorderStyle = FormBorderStyle.None
            page.Dock = DockStyle.Fill

            _contentPanel.Controls.Add(page)
            page.Show()
        End Sub
    End Class
End Namespace
