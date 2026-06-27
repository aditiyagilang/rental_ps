Imports System.Collections.Generic
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class FrmSparepartPurchase
        Inherits FrmMasterData

        Public Sub New()
            MyBase.New(New MasterFormConfig(
                "Beli Sparepart",
                "sparepart_purchases",
                New List(Of MasterField) From {
                    MasterField.Text("purchase_no", "No Pembelian", True, "PB-20260627-001"),
                    MasterField.LookupField("supplier_id", "Supplier", "SELECT id, name FROM suppliers WHERE is_active = 1 ORDER BY name"),
                    MasterField.DateTimeField("purchase_date", "Tanggal", True),
                    MasterField.DecimalValue("total_amount", "Total", True),
                    MasterField.Multiline("notes", "Catatan")
                },
                New List(Of String) From {"purchase_no"},
                "purchase_no"
            ))
        End Sub
    End Class
End Namespace
