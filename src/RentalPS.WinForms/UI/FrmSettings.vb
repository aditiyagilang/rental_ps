Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports RentalPS.WinForms.Infrastructure
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class FrmSettings
        Inherits Form

        Public Sub New()
            BackColor = AppTheme.Surface
            Font = New Font("Segoe UI", 9.0F)
            BuildLayout()
        End Sub

        Private Sub BuildLayout()
            Dim tabs = New TabControl With {.Dock = DockStyle.Fill}
            tabs.TabPages.Add(CreatePage("Profil Toko", CreateSettingsForm()))
            tabs.TabPages.Add(CreatePage("Metode Pembayaran", CreatePaymentMethodForm()))
            If AppSession.IsAdmin Then
                tabs.TabPages.Add(CreatePage("User", New FrmUsers()))
            End If
            Controls.Add(tabs)
        End Sub

        Private Shared Function CreatePage(title As String, child As Form) As TabPage
            Dim page = New TabPage(title)
            child.TopLevel = False
            child.FormBorderStyle = FormBorderStyle.None
            child.Dock = DockStyle.Fill
            page.Controls.Add(child)
            child.Show()
            Return page
        End Function

        Private Shared Function CreateSettingsForm() As Form
            Return New FrmMasterData(New MasterFormConfig(
                "Profil Toko / Setting",
                "settings",
                New List(Of MasterField) From {
                    MasterField.Text("setting_key", "Kode Setting", True),
                    MasterField.Multiline("setting_value", "Nilai"),
                    MasterField.Multiline("description", "Deskripsi")
                },
                New List(Of String) From {"setting_key", "setting_value"},
                "setting_key"
            ))
        End Function

        Private Shared Function CreatePaymentMethodForm() As Form
            Return New FrmMasterData(New MasterFormConfig(
                "Metode Pembayaran",
                "payment_methods",
                New List(Of MasterField) From {
                    MasterField.Text("code", "Kode", True, "CASH"),
                    MasterField.Text("name", "Nama", True),
                    MasterField.BooleanField("is_active", "Aktif")
                },
                New List(Of String) From {"code", "name"},
                "name"
            ))
        End Function

    End Class
End Namespace
