Imports System.Collections.Generic
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class FrmGames
        Inherits FrmMasterData

        Public Sub New()
            MyBase.New(New MasterFormConfig(
                "Game",
                "games",
                New List(Of MasterField) From {
                    MasterField.Text("code", "Kode", True, "G001"),
                    MasterField.Text("title", "Judul", True),
                    MasterField.Text("platform", "Platform", True, "PS4/PS5"),
                    MasterField.Text("genre", "Genre"),
                    MasterField.DecimalValue("size_gb", "Ukuran GB"),
                    MasterField.DecimalValue("install_price", "Harga Install", True),
                    MasterField.BooleanField("is_active", "Aktif")
                },
                New List(Of String) From {"code", "title", "platform", "genre"},
                "title"
            ))
        End Sub
    End Class
End Namespace
