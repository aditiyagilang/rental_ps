Imports System.Collections.Generic
Imports System.Globalization
Imports System.Windows.Forms
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Friend NotInheritable Class GridHeaderHelper
        Private Sub New()
        End Sub

        Public Shared Sub Apply(grid As DataGridView)
            For Each column As DataGridViewColumn In grid.Columns
                column.HeaderText = ToTitle(column.Name)
            Next
        End Sub

        Public Shared Sub Apply(grid As DataGridView, fields As IEnumerable(Of MasterField))
            Apply(grid)

            For Each field In fields
                If grid.Columns.Contains(field.ColumnName) Then
                    grid.Columns(field.ColumnName).HeaderText = field.LabelText
                End If
            Next

            If grid.Columns.Contains("id") Then
                grid.Columns("id").HeaderText = "ID"
            End If
        End Sub

        Private Shared Function ToTitle(value As String) As String
            If String.IsNullOrWhiteSpace(value) Then
                Return value
            End If

            Dim cleaned = value.Replace("_", " ")
            Return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleaned.ToLowerInvariant())
        End Function
    End Class
End Namespace
