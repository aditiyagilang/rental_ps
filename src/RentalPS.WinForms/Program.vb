Imports System
Imports System.Windows.Forms
Imports RentalPS.WinForms.UI

Namespace RentalPS.WinForms
    Friend Module Program
        <STAThread>
        Sub Main()
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            Application.Run(New FrmLogin())
        End Sub
    End Module
End Namespace
