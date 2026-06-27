Namespace RentalPS.WinForms.Models
    Public Class StockTransactionItem
        Public Property ItemId As Long
        Public Property ItemName As String
        Public Property Qty As Integer
        Public Property Price As Decimal
        Public ReadOnly Property Subtotal As Decimal
            Get
                Return Qty * Price
            End Get
        End Property
    End Class
End Namespace
