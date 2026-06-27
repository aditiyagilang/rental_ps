Imports System.Collections.Generic
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class FrmSpareparts
        Inherits FrmMasterData

        Public Sub New()
            MyBase.New(New MasterFormConfig(
                "Sparepart",
                "spareparts",
                New List(Of MasterField) From {
                    MasterField.Text("code", "Kode", True, "SP001"),
                    MasterField.Text("name", "Nama", True),
                    MasterField.Text("category", "Kategori"),
                    MasterField.DecimalValue("purchase_price", "Harga Beli", True),
                    MasterField.DecimalValue("selling_price", "Harga Jual", True),
                    MasterField.IntegerValue("stock_qty", "Stok", True),
                    MasterField.IntegerValue("minimum_stock", "Minimum Stok", True),
                    MasterField.Text("unit", "Satuan", True, "pcs"),
                    MasterField.BooleanField("is_active", "Aktif")
                },
                New List(Of String) From {"code", "name", "category"},
                "name"
            ))
        End Sub
    End Class
End Namespace
