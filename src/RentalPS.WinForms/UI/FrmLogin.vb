Imports System.Drawing
Imports System.Windows.Forms
Imports RentalPS.WinForms.Infrastructure
Imports RentalPS.WinForms.Repositories

Namespace RentalPS.WinForms.UI
    Public Class FrmLogin
        Inherits Form

        Private ReadOnly _usernameTextBox As New TextBox()
        Private ReadOnly _passwordTextBox As New TextBox()
        Private ReadOnly _statusLabel As New Label()
        Private ReadOnly _userRepository As New UserRepository()

        Public Sub New()
            Text = "Login - Rental PS"
            StartPosition = FormStartPosition.CenterScreen
            MinimumSize = New Size(420, 460)
            Size = New Size(420, 460)
            BackColor = AppTheme.Surface
            Font = New Font("Segoe UI", 9.0F)

            BuildLayout()
        End Sub

        Private Sub BuildLayout()
            Dim panel = New Panel With {
                .Dock = DockStyle.Fill,
                .Padding = New Padding(36)
            }

            Dim title = New Label With {
                .AutoSize = False,
                .Dock = DockStyle.Top,
                .Height = 56,
                .Text = "Rental PS",
                .Font = New Font("Segoe UI", 22.0F, FontStyle.Bold),
                .ForeColor = AppTheme.TextDark
            }

            Dim subtitle = New Label With {
                .AutoSize = False,
                .Dock = DockStyle.Top,
                .Height = 42,
                .Text = "Masuk ke aplikasi operasional",
                .ForeColor = AppTheme.TextMuted
            }

            _usernameTextBox.PlaceholderText = "Username"
            _usernameTextBox.Height = 36
            _usernameTextBox.Dock = DockStyle.Top
            _usernameTextBox.Text = "admin"

            _passwordTextBox.PlaceholderText = "Password"
            _passwordTextBox.Height = 36
            _passwordTextBox.Dock = DockStyle.Top
            _passwordTextBox.UseSystemPasswordChar = True

            Dim loginButton = New Button With {
                .Text = "Masuk",
                .Dock = DockStyle.Top,
                .Margin = New Padding(0, 16, 0, 0)
            }
            AppTheme.StylePrimaryButton(loginButton)
            AddHandler loginButton.Click, AddressOf LoginButton_Click

            _statusLabel.Dock = DockStyle.Top
            _statusLabel.Height = 54
            _statusLabel.ForeColor = AppTheme.TextMuted

            panel.Controls.Add(_statusLabel)
            panel.Controls.Add(loginButton)
            panel.Controls.Add(CreateSpacer(12))
            panel.Controls.Add(_passwordTextBox)
            panel.Controls.Add(CreateFieldLabel("Password"))
            panel.Controls.Add(CreateSpacer(10))
            panel.Controls.Add(_usernameTextBox)
            panel.Controls.Add(CreateFieldLabel("Username"))
            panel.Controls.Add(subtitle)
            panel.Controls.Add(title)

            Controls.Add(panel)
        End Sub

        Private Shared Function CreateFieldLabel(text As String) As Control
            Return New Label With {
                .Dock = DockStyle.Top,
                .Height = 28,
                .Text = text,
                .ForeColor = AppTheme.TextDark,
                .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold),
                .TextAlign = ContentAlignment.BottomLeft
            }
        End Function

        Private Shared Function CreateSpacer(height As Integer) As Control
            Return New Panel With {.Dock = DockStyle.Top, .Height = height}
        End Function

        Private Sub LoginButton_Click(sender As Object, e As EventArgs)
            Try
                Dim username = _usernameTextBox.Text.Trim()
                Dim password = _passwordTextBox.Text

                If username = "" Then
                    _statusLabel.ForeColor = AppTheme.Danger
                    _statusLabel.Text = "Username wajib diisi."
                    Return
                End If

                If Not _userRepository.UsernameExists(username) Then
                    _statusLabel.ForeColor = AppTheme.Danger
                    _statusLabel.Text = "User tidak ditemukan atau nonaktif."
                    Return
                End If

                If Not _userRepository.ValidateLogin(username, password) Then
                    _statusLabel.ForeColor = AppTheme.Danger
                    _statusLabel.Text = "Password salah."
                    Return
                End If

                AppSession.Start(username, _userRepository.IsInRole(username, "admin"))

                Hide()
                Using mainForm = New FrmMain(username)
                    mainForm.ShowDialog(Me)

                    If mainForm.IsLogoutRequested Then
                        AppSession.Clear()
                        _passwordTextBox.Clear()
                        _statusLabel.ForeColor = AppTheme.TextMuted
                        _statusLabel.Text = "Silakan login kembali."
                        Show()
                        _passwordTextBox.Focus()
                        Return
                    End If
                End Using

                Close()
            Catch ex As Exception
                _statusLabel.ForeColor = AppTheme.Danger
                _statusLabel.Text = "Login gagal: " & ex.Message
            End Try
        End Sub
    End Class
End Namespace
