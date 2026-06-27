Imports System.Collections.Generic
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class FrmRoomTypes
        Inherits FrmMasterData

        Public Sub New()
            MyBase.New(New MasterFormConfig(
                "Jenis Ruang",
                "room_types",
                New List(Of MasterField) From {
                    MasterField.Text("code", "Kode", True, "REG"),
                    MasterField.Text("name", "Nama", True, "Regular"),
                    MasterField.DecimalValue("hourly_rate", "Tarif Per Jam", True),
                    MasterField.DecimalValue("overtime_rate", "Tarif Overtime", True),
                    MasterField.Multiline("description", "Deskripsi"),
                    MasterField.BooleanField("is_active", "Aktif")
                },
                New List(Of String) From {"code", "name"},
                "name"
            ))
        End Sub
    End Class
End Namespace
