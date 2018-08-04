Public Class AInformation
    Dim YML As String = NGA.TextBox2.Text & "\apktool.yml"

#Region " Border Less Form Move "
    Private mouseOffset As Point

    Private Sub Panel1_MouseDown(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles Panel1.MouseDown
        mouseOffset = New Point(-e.X, -e.Y)
    End Sub

    Private Sub Panel1_MouseMove(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles Panel1.MouseMove
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Dim mousePos As Point = Control.MousePosition
            mousePos.Offset(mouseOffset.X, mouseOffset.Y)
            Location = mousePos
        End If
    End Sub
#End Region

#Region " Form Load "
    Private Sub AInformation_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Try
            Me.RichTextBox1.LoadFile(YML, RichTextBoxStreamType.PlainText)
        Catch ex As Exception
            NGA.RichTextBox2.Text = ex.ToString
            Me.Close()
        End Try

        Try
            If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\UAA\Config", "Top", "") Then
                Panel1.BackColor = ColorTranslator.FromHtml(My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\UAA\Config", "Top", ""))
            End If
        Catch ex As Exception
        End Try
    End Sub
#End Region

#Region " Button Click "
    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        If Not Me.RichTextBox1.Text.Length = 0 Then
            My.Computer.FileSystem.WriteAllText(YML, Me.RichTextBox1.Text, False)
        End If
    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub
#End Region

End Class