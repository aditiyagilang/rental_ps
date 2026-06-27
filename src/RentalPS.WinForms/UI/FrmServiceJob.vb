Imports System.Collections.Generic
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class FrmServiceJob
        Inherits FrmMasterData

        Public Sub New()
            MyBase.New(New MasterFormConfig(
                "Transaksi Service",
                "service_jobs",
                New List(Of MasterField) From {
                    MasterField.Text("service_no", "No Service", True, "SV-20260627-001"),
                    MasterField.LookupField("customer_id", "Pelanggan", "SELECT id, name FROM customers WHERE is_active = 1 ORDER BY name", True),
                    MasterField.LookupField("service_id", "Jasa", "SELECT id, name FROM services WHERE is_active = 1 ORDER BY name"),
                    MasterField.Text("item_name", "Barang", True, "PS / Controller"),
                    MasterField.Multiline("problem_description", "Keluhan", True),
                    MasterField.Multiline("technician_notes", "Catatan Teknisi"),
                    MasterField.DecimalValue("labor_cost", "Biaya Jasa", True),
                    MasterField.DecimalValue("sparepart_cost", "Biaya Sparepart", True),
                    MasterField.DecimalValue("total_amount", "Total", True),
                    MasterField.ComboField("status", "Status", New String() {"received", "diagnosis", "processing", "completed", "picked_up", "cancelled"}, True),
                    MasterField.DateTimeField("received_at", "Diterima", True),
                    MasterField.DateTimeField("completed_at", "Selesai"),
                    MasterField.DateTimeField("picked_up_at", "Diambil")
                },
                New List(Of String) From {"service_no", "item_name", "status"},
                "service_no"
            ))
        End Sub
    End Class
End Namespace
