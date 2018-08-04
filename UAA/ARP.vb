Public Class ARP

#Region " Form Load "
    Private Sub ARP_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Try
            If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\UAA\Config", "Top", "") Then
                Panel1.BackColor = ColorTranslator.FromHtml(My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\UAA\Config", "Top", ""))
                Me.BackColor = ColorTranslator.FromHtml(My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\UAA\Config", "Top", ""))
            End If
        Catch ex As Exception
        End Try
        Me.RichTextBox1.LoadFile(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\AndroidManifest.xml", RichTextBoxStreamType.PlainText)
    End Sub
#End Region

#Region " Click "
    Private Sub Panel1_MouseDoubleClick(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles Panel1.MouseDoubleClick
        If Me.WindowState = FormWindowState.Normal Then
            Me.WindowState = FormWindowState.Maximized
        Else
            Me.WindowState = FormWindowState.Normal
        End If
    End Sub

    Private Sub Button45_Click(sender As System.Object, e As System.EventArgs) Handles Button45.Click
        My.Computer.FileSystem.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\AndroidManifest.xml", Me.RichTextBox1.Text, False)
        Me.Close()
    End Sub
#End Region

End Class