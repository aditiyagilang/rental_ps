Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms
Imports RentalPS.WinForms.Models

Namespace RentalPS.WinForms.UI
    Public Class SimpleDonutChart
        Inherits Control

        Private ReadOnly _colors As Color() = {
            Color.FromArgb(37, 99, 235),
            Color.FromArgb(22, 163, 74),
            Color.FromArgb(234, 88, 12),
            Color.FromArgb(147, 51, 234),
            Color.FromArgb(220, 38, 38)
        }

        Public Property Items As New List(Of ChartItem)()
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

            Dim total As Decimal = 0D
            For Each item In Items
                total += item.Value
            Next

            If total <= 0D Then
                Using font = New Font("Segoe UI", 9.0F)
                    e.Graphics.DrawString("Belum ada data", font, New SolidBrush(AppTheme.TextMuted), 14, 52)
                End Using
                Return
            End If

            Dim diameter = Math.Min(Width - 160, Height - 64)
            If diameter < 70 Then
                diameter = Math.Min(Width - 36, Height - 80)
            End If

            Dim rect = New Rectangle(18, 52, diameter, diameter)
            Dim startAngle = -90.0F

            For i = 0 To Items.Count - 1
                Dim sweep = CSng((Items(i).Value / total) * 360D)
                Using brush = New SolidBrush(_colors(i Mod _colors.Length))
                    e.Graphics.FillPie(brush, rect, startAngle, sweep)
                End Using
                startAngle += sweep
            Next

            Dim inner = Rectangle.Inflate(rect, -CInt(diameter * 0.28), -CInt(diameter * 0.28))
            Using brush = New SolidBrush(Color.White)
                e.Graphics.FillEllipse(brush, inner)
            End Using

            Dim legendX = rect.Right + 18
            Dim legendY = 54
            Using font = New Font("Segoe UI", 8.5F)
                For i = 0 To Items.Count - 1
                    Using brush = New SolidBrush(_colors(i Mod _colors.Length))
                        e.Graphics.FillRectangle(brush, legendX, legendY + 4, 10, 10)
                    End Using
                    Dim text = Items(i).Label & " (" & Items(i).Value.ToString("N0") & ")"
                    e.Graphics.DrawString(text, font, New SolidBrush(AppTheme.TextDark), legendX + 16, legendY)
                    legendY += 24
                Next
            End Using
        End Sub
    End Class
End Namespace
