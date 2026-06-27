Imports System.ComponentModel

Namespace RentalPS.WinForms.UI
    Friend NotInheritable Class DesignModeHelper
        Private Sub New()
        End Sub

        Public Shared Function IsDesignMode() As Boolean
            Return LicenseManager.UsageMode = LicenseUsageMode.Designtime
        End Function
    End Class
End Namespace
