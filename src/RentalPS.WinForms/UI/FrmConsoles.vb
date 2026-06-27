Imports System.Collections.Generic
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class FrmConsoles
        Inherits FrmMasterData

        Public Sub New()
            MyBase.New(New MasterFormConfig(
                "Stok PS",
                "consoles",
                New List(Of MasterField) From {
                    MasterField.Text("code", "Kode", True, "PS001"),
                    MasterField.Text("name", "Nama", True, "PlayStation 4 - 1"),
                    MasterField.Text("console_type", "Tipe Console", True, "PS4"),
                    MasterField.Text("serial_number", "Serial Number"),
                    MasterField.DateField("purchase_date", "Tanggal Beli"),
                    MasterField.ComboField("condition_status", "Kondisi", New String() {"good", "minor_issue", "broken", "service"}, True),
                    MasterField.ComboField("availability_status", "Status Ketersediaan", New String() {"available", "rented", "maintenance", "inactive"}, True),
                    MasterField.Multiline("notes", "Catatan")
                },
                New List(Of String) From {"code", "name", "console_type", "serial_number"},
                "name"
            ))
        End Sub
    End Class
End Namespace
