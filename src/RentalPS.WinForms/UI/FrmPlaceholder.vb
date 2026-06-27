Imports System.Drawing
Imports System.Windows.Forms

Namespace RentalPS.WinForms.UI
    Public Class FrmPlaceholder
        Inherits Form

        Public Sub New(title As String)
            BackColor = AppTheme.Surface

            Dim panel = New Panel With {
                .Dock = DockStyle.Fill,
                .BackColor = Color.White,
                .Padding = New Padding(24)
            }

            Dim titleLabel = New Label With {
                .Dock = DockStyle.Top,
                .Height = 42,
                .Text = title,
                .Font = New Font("Segoe UI", 16.0F, FontStyle.Bold),
                .ForeColor = AppTheme.TextDark
            }

            Dim bodyLabel = New Label With {
                .Dock = DockStyle.Top,
                .Height = 80,
                .Text = "Form ini sudah disiapkan di menu. Implementasi detailnya mengikuti pola form Pelanggan.",
                .ForeColor = AppTheme.TextMuted
            }

            panel.Controls.Add(bodyLabel)
            panel.Controls.Add(titleLabel)
            Controls.Add(panel)
        End Sub
    End Class
End Namespace
