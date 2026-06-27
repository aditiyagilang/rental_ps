Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class SimpleBarChart
        Inherits Control

        Public Property Items As New List(Of ChartItem)()
        Public Property BarColor As Color = AppTheme.Accent
        Public Property Title As String = ""

        Public Sub New()
            DoubleBuffered = True
            BackColor = Color.White
            MinimumSize = New Size(240, 180)
        End Sub

        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias
            e.Graphics.Clear(BackColor)

            Using titleFont = New Font("Segoe UI", 11.0F, FontStyle.Bold)
                e.Graphics.DrawString(Title, titleFont, New SolidBrush(AppTheme.TextDark), 14, 12)
            End Using

            If Items.Count = 0 Then
                DrawEmpty(e.Graphics)
                Return
            End If

            Dim chartRect = New Rectangle(14, 48, Width - 28, Height - 82)
            If chartRect.Width <= 0 OrElse chartRect.Height <= 0 Then
                Return
            End If

            Dim maxValue As Decimal = 0D
            For Each item In Items
                If item.Value > maxValue Then
                    maxValue = item.Value
                End If
            Next

            If maxValue <= 0D Then
                maxValue = 1D
            End If

            Dim gap = 10
            Dim barWidth = Math.Max(12, CInt((chartRect.Width - ((Items.Count - 1) * gap)) / Items.Count))

            Using gridPen = New Pen(AppTheme.Border)
                For i = 0 To 3
                    Dim y = chartRect.Bottom - CInt(chartRect.Height * (i / 3.0))
                    e.Graphics.DrawLine(gridPen, chartRect.Left, y, chartRect.Right, y)
                Next
            End Using

            For i = 0 To Items.Count - 1
                Dim item = Items(i)
                Dim ratio = Convert.ToDouble(item.Value / maxValue)
                Dim barHeight = CInt(chartRect.Height * ratio)
                Dim x = chartRect.Left + i * (barWidth + gap)
                Dim y = chartRect.Bottom - barHeight
                Dim rect = New Rectangle(x, y, barWidth, barHeight)

                Using brush = New SolidBrush(BarColor)
                    e.Graphics.FillRectangle(brush, rect)
                End Using

                Using labelFont = New Font("Segoe UI", 8.0F)
                    Dim label = item.Label
                    Dim size = e.Graphics.MeasureString(label, labelFont)
                    e.Graphics.DrawString(label, labelFont, New SolidBrush(AppTheme.TextMuted), x + (barWidth - size.Width) / 2, chartRect.Bottom + 8)
                End Using
            Next
        End Sub

        Private Sub DrawEmpty(graphics As Graphics)
            Using font = New Font("Segoe UI", 9.0F)
                graphics.DrawString("Belum ada data", font, New SolidBrush(AppTheme.TextMuted), 14, 52)
            End Using
        End Sub
    End Class
End Namespace
