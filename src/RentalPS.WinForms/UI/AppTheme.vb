Imports System.Drawing
Imports System.Windows.Forms

Namespace RentalPS.WinForms.UI
    Friend NotInheritable Class AppTheme
        Private Sub New()
        End Sub

        Public Shared ReadOnly SidebarBack As Color = Color.FromArgb(26, 32, 44)
        Public Shared ReadOnly SidebarActive As Color = Color.FromArgb(45, 55, 72)
        Public Shared ReadOnly Accent As Color = Color.FromArgb(37, 99, 235)
        Public Shared ReadOnly Success As Color = Color.FromArgb(22, 163, 74)
        Public Shared ReadOnly Danger As Color = Color.FromArgb(220, 38, 38)
        Public Shared ReadOnly Surface As Color = Color.FromArgb(248, 250, 252)
        Public Shared ReadOnly Border As Color = Color.FromArgb(226, 232, 240)
        Public Shared ReadOnly TextDark As Color = Color.FromArgb(15, 23, 42)
        Public Shared ReadOnly TextMuted As Color = Color.FromArgb(100, 116, 139)

        Public Shared Sub StylePrimaryButton(button As Button)
            button.BackColor = Accent
            button.ForeColor = Color.White
            button.FlatStyle = FlatStyle.Flat
            button.FlatAppearance.BorderSize = 0
            button.Height = 38
            button.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        End Sub

        Public Shared Sub StyleSecondaryButton(button As Button)
            button.BackColor = Color.White
            button.ForeColor = TextDark
            button.FlatStyle = FlatStyle.Flat
            button.FlatAppearance.BorderColor = Border
            button.Height = 38
            button.Font = New Font("Segoe UI", 9.0F, FontStyle.Regular)
        End Sub

        Public Shared Sub StyleGrid(grid As DataGridView)
            grid.AllowUserToAddRows = False
            grid.AllowUserToDeleteRows = False
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            grid.BackgroundColor = Color.White
            grid.BorderStyle = BorderStyle.None
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249)
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextDark
            grid.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
            grid.ColumnHeadersHeight = 38
            grid.DefaultCellStyle.Font = New Font("Segoe UI", 9.0F)
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254)
            grid.DefaultCellStyle.SelectionForeColor = TextDark
            grid.EnableHeadersVisualStyles = False
            grid.GridColor = Border
            grid.ReadOnly = True
            grid.RowHeadersVisible = False
            grid.RowTemplate.Height = 34
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        End Sub
    End Class
End Namespace
