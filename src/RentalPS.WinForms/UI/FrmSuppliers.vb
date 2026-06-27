Imports System.Collections.Generic
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class FrmSuppliers
        Inherits FrmMasterData

        Public Sub New()
            MyBase.New(New MasterFormConfig(
                "Supplier",
                "suppliers",
                New List(Of MasterField) From {
                    MasterField.Text("code", "Kode", True, "SUP001"),
                    MasterField.Text("name", "Nama", True),
                    MasterField.Text("phone", "No HP"),
                    MasterField.Multiline("address", "Alamat"),
                    MasterField.Multiline("notes", "Catatan"),
                    MasterField.BooleanField("is_active", "Aktif")
                },
                New List(Of String) From {"code", "name", "phone"},
                "name"
            ))
        End Sub
    End Class
End Namespace
