Imports System.Collections.Generic
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class FrmRental
        Inherits FrmMasterData

        Public Sub New()
            MyBase.New(New MasterFormConfig(
                "Transaksi Sewa",
                "rentals",
                New List(Of MasterField) From {
                    MasterField.Text("rental_no", "No Sewa", True, "SW-20260627-001"),
                    MasterField.LookupField("booking_id", "Booking", "SELECT id, booking_no AS name FROM bookings WHERE status IN ('booked','checked_in') ORDER BY start_time DESC"),
                    MasterField.LookupField("customer_id", "Pelanggan", "SELECT id, name FROM customers WHERE is_active = 1 ORDER BY name", True),
                    MasterField.LookupField("room_id", "Ruang", "SELECT id, name FROM rooms WHERE status <> 'inactive' ORDER BY name", True),
                    MasterField.LookupField("console_id", "PS", "SELECT id, name FROM consoles WHERE availability_status <> 'inactive' ORDER BY name", True),
                    MasterField.DateTimeField("start_time", "Mulai", True),
                    MasterField.DateTimeField("planned_end_time", "Rencana Selesai"),
                    MasterField.DateTimeField("actual_end_time", "Selesai Aktual"),
                    MasterField.DecimalValue("hourly_rate", "Tarif/Jam", True),
                    MasterField.IntegerValue("duration_minutes", "Durasi Menit", True),
                    MasterField.DecimalValue("rental_amount", "Biaya Sewa", True),
                    MasterField.DecimalValue("discount_amount", "Diskon", True),
                    MasterField.DecimalValue("total_amount", "Total", True),
                    MasterField.DecimalValue("paid_amount", "Dibayar", True),
                    MasterField.ComboField("status", "Status", New String() {"running", "completed", "cancelled"}, True),
                    MasterField.Multiline("notes", "Catatan")
                },
                New List(Of String) From {"rental_no", "status"},
                "rental_no"
            ))
        End Sub
    End Class
End Namespace
