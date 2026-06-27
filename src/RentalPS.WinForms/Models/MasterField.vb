Imports System.Collections.Generic

Namespace RentalPS.WinForms.Models
    Public Class MasterField
        Public Property ColumnName As String
        Public Property LabelText As String
        Public Property Kind As MasterFieldKind
        Public Property Required As Boolean
        Public Property Placeholder As String
        Public Property Options As List(Of String)
        Public Property LookupSql As String
        Public Property LookupDisplayMember As String
        Public Property LookupValueMember As String

        Public Sub New(columnName As String, labelText As String, kind As MasterFieldKind, required As Boolean, Optional placeholder As String = "")
            Me.ColumnName = columnName
            Me.LabelText = labelText
            Me.Kind = kind
            Me.Required = required
            Me.Placeholder = placeholder
            Options = New List(Of String)()
            LookupDisplayMember = "name"
            LookupValueMember = "id"
        End Sub

        Public Shared Function Text(columnName As String, labelText As String, Optional required As Boolean = False, Optional placeholder As String = "") As MasterField
            Return New MasterField(columnName, labelText, MasterFieldKind.Text, required, placeholder)
        End Function

        Public Shared Function Multiline(columnName As String, labelText As String, Optional required As Boolean = False) As MasterField
            Return New MasterField(columnName, labelText, MasterFieldKind.MultilineText, required)
        End Function

        Public Shared Function IntegerValue(columnName As String, labelText As String, Optional required As Boolean = False) As MasterField
            Return New MasterField(columnName, labelText, MasterFieldKind.IntegerNumber, required)
        End Function

        Public Shared Function DecimalValue(columnName As String, labelText As String, Optional required As Boolean = False) As MasterField
            Return New MasterField(columnName, labelText, MasterFieldKind.DecimalNumber, required)
        End Function

        Public Shared Function DateField(columnName As String, labelText As String, Optional required As Boolean = False) As MasterField
            Return New MasterField(columnName, labelText, MasterFieldKind.DateValue, required)
        End Function

        Public Shared Function DateTimeField(columnName As String, labelText As String, Optional required As Boolean = False) As MasterField
            Return New MasterField(columnName, labelText, MasterFieldKind.DateTimeValue, required)
        End Function

        Public Shared Function BooleanField(columnName As String, labelText As String) As MasterField
            Return New MasterField(columnName, labelText, MasterFieldKind.BooleanValue, False)
        End Function

        Public Shared Function ComboField(columnName As String, labelText As String, options As IEnumerable(Of String), Optional required As Boolean = False) As MasterField
            Dim field = New MasterField(columnName, labelText, MasterFieldKind.Combo, required)
            field.Options.AddRange(options)
            Return field
        End Function

        Public Shared Function LookupField(columnName As String, labelText As String, lookupSql As String, Optional required As Boolean = False) As MasterField
            Return New MasterField(columnName, labelText, MasterFieldKind.Lookup, required) With {
                .LookupSql = lookupSql
            }
        End Function
    End Class
End Namespace
