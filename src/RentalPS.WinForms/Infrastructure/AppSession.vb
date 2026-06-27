Namespace RentalPS.WinForms.Infrastructure
    Public NotInheritable Class AppSession
        Private Sub New()
        End Sub

        Public Shared Property Username As String = ""
        Public Shared Property IsAdmin As Boolean

        Public Shared Sub Start(username As String, isAdmin As Boolean)
            AppSession.Username = username
            AppSession.IsAdmin = isAdmin
        End Sub

        Public Shared Sub Clear()
            Username = ""
            IsAdmin = False
        End Sub
    End Class
End Namespace
