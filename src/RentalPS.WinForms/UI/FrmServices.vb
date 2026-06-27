Imports System.Collections.Generic
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class FrmServices
        Inherits FrmMasterData

        Public Sub New()
            MyBase.New(New MasterFormConfig(
                "Jasa",
                "services",
                New List(Of MasterField) From {
                    MasterField.Text("code", "Kode", True, "JS001"),
                    MasterField.Text("name", "Nama", True),
                    MasterField.DecimalValue("default_price", "Harga Default", True),
                    MasterField.Multiline("description", "Deskripsi"),
                    MasterField.BooleanField("is_active", "Aktif")
                },
                New List(Of String) From {"code", "name"},
                "name"
            ))
        End Sub
    End Class
End Namespace
