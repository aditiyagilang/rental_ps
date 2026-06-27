Imports System.Collections.Generic
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class FrmBooking
        Inherits FrmMasterData

        Public Sub New()
            MyBase.New(New MasterFormConfig(
                "Transaksi Booking",
                "bookings",
                New List(Of MasterField) From {
                    MasterField.Text("booking_no", "No Booking", True, "BK-20260627-001"),
                    MasterField.LookupField("customer_id", "Pelanggan", "SELECT id, name FROM customers WHERE is_active = 1 ORDER BY name", True),
                    MasterField.LookupField("room_id", "Ruang", "SELECT id, name FROM rooms WHERE status <> 'inactive' ORDER BY name"),
                    MasterField.LookupField("console_id", "PS", "SELECT id, name FROM consoles WHERE availability_status <> 'inactive' ORDER BY name"),
                    MasterField.DateTimeField("start_time", "Mulai", True),
                    MasterField.DateTimeField("end_time", "Selesai", True),
                    MasterField.DecimalValue("deposit_amount", "DP", True),
                    MasterField.ComboField("status", "Status", New String() {"booked", "checked_in", "completed", "cancelled"}, True),
                    MasterField.Multiline("notes", "Catatan")
                },
                New List(Of String) From {"booking_no", "status"},
                "booking_no"
            ))
        End Sub
    End Class
End Namespace
