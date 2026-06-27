Imports System.Collections.Generic
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class FrmGameInstall
        Inherits FrmMasterData

        Public Sub New()
            MyBase.New(New MasterFormConfig(
                "Transaksi Isi Game",
                "game_installs",
                New List(Of MasterField) From {
                    MasterField.Text("install_no", "No Transaksi", True, "IG-20260627-001"),
                    MasterField.LookupField("customer_id", "Pelanggan", "SELECT id, name FROM customers WHERE is_active = 1 ORDER BY name", True),
                    MasterField.Text("device_name", "Device", True, "HDD / PS / Laptop"),
                    MasterField.DecimalValue("device_capacity_gb", "Kapasitas GB"),
                    MasterField.DecimalValue("service_fee", "Biaya Jasa", True),
                    MasterField.DecimalValue("total_amount", "Total", True),
                    MasterField.ComboField("status", "Status", New String() {"received", "processing", "completed", "picked_up", "cancelled"}, True),
                    MasterField.Multiline("notes", "Catatan"),
                    MasterField.DateTimeField("received_at", "Diterima", True),
                    MasterField.DateTimeField("completed_at", "Selesai")
                },
                New List(Of String) From {"install_no", "device_name", "status"},
                "install_no"
            ))
        End Sub
    End Class
End Namespace
