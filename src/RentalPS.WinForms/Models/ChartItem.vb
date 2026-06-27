Namespace RentalPS.WinForms.Models
    Public Class ChartItem
        Public Property Label As String
        Public Property Value As Decimal

        Public Sub New(label As String, value As Decimal)
            Me.Label = label
            Me.Value = value
        End Sub
    End Class
End Namespace
