Imports System.Collections.Generic
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class FrmFnbSale
        Inherits FrmMasterData

        Public Sub New()
            MyBase.New(New MasterFormConfig(
                "Penjualan FNB",
                "fnb_sales",
                New List(Of MasterField) From {
                    MasterField.Text("sale_no", "No Penjualan", True, "FNB-20260627-001"),
                    MasterField.LookupField("customer_id", "Pelanggan", "SELECT id, name FROM customers WHERE is_active = 1 ORDER BY name"),
                    MasterField.DateTimeField("sale_date", "Tanggal", True),
                    MasterField.DecimalValue("total_amount", "Total", True),
                    MasterField.DecimalValue("paid_amount", "Dibayar", True),
                    MasterField.ComboField("status", "Status", New String() {"draft", "paid", "void"}, True),
                    MasterField.Multiline("notes", "Catatan")
                },
                New List(Of String) From {"sale_no", "status"},
                "sale_no"
            ))
        End Sub
    End Class
End Namespace
