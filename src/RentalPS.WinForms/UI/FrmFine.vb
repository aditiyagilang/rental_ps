Imports System.Collections.Generic
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class FrmFine
        Inherits FrmMasterData

        Public Sub New()
            MyBase.New(New MasterFormConfig(
                "Transaksi Denda",
                "fines",
                New List(Of MasterField) From {
                    MasterField.Text("fine_no", "No Denda", True, "DN-20260627-001"),
                    MasterField.LookupField("rental_id", "Sewa", "SELECT id, rental_no AS name FROM rentals ORDER BY start_time DESC"),
                    MasterField.LookupField("customer_id", "Pelanggan", "SELECT id, name FROM customers WHERE is_active = 1 ORDER BY name", True),
                    MasterField.ComboField("fine_type", "Jenis Denda", New String() {"late", "damage", "lost_item", "manual"}, True),
                    MasterField.Multiline("description", "Deskripsi", True),
                    MasterField.DecimalValue("amount", "Nominal", True),
                    MasterField.ComboField("status", "Status", New String() {"unpaid", "paid", "void"}, True)
                },
                New List(Of String) From {"fine_no", "fine_type", "status"},
                "fine_no"
            ))
        End Sub
    End Class
End Namespace
