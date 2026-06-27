Imports System.Collections.Generic

Namespace RentalPS.WinForms.Models
    Public Class MasterFormConfig
        Public Property Title As String
        Public Property TableName As String
        Public Property Fields As List(Of MasterField)
        Public Property SearchColumns As List(Of String)
        Public Property OrderBy As String

        Public Sub New(title As String, tableName As String, fields As List(Of MasterField), searchColumns As List(Of String), orderBy As String)
            Me.Title = title
            Me.TableName = tableName
            Me.Fields = fields
            Me.SearchColumns = searchColumns
            Me.OrderBy = orderBy
        End Sub
    End Class
End Namespace
