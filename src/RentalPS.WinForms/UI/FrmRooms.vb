Imports System.Collections.Generic
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class FrmRooms
        Inherits FrmMasterData

        Public Sub New()
            MyBase.New(New MasterFormConfig(
                "Ruang",
                "rooms",
                New List(Of MasterField) From {
                    MasterField.LookupField("room_type_id", "Jenis Ruang", "SELECT id, name FROM room_types WHERE is_active = 1 ORDER BY name", True),
                    MasterField.Text("code", "Kode", True, "R001"),
                    MasterField.Text("name", "Nama", True, "Ruang 1"),
                    MasterField.IntegerValue("capacity", "Kapasitas", True),
                    MasterField.ComboField("status", "Status", New String() {"available", "occupied", "maintenance", "inactive"}, True),
                    MasterField.Multiline("notes", "Catatan")
                },
                New List(Of String) From {"code", "name", "status"},
                "name"
            ))
        End Sub
    End Class
End Namespace
