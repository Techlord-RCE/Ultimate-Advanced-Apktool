Imports System.IO

Public Class Other

#Region " Form Load "
    Private Sub Other_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Try
            If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\UAA\Config", "Top", "") Then
                Panel1.BackColor = ColorTranslator.FromHtml(My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\UAA\Config", "Top", ""))
            End If
        Catch ex As Exception
        End Try
    End Sub
#End Region

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

#Region " Drag & Drop "

    Private Sub TextBox1_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox1.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        Me.TextBox1.Text = (strArray(0))
    End Sub

    Private Sub TextBox1_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox1.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith(".apk")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub
#End Region

#Region " Button Click "

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        With OpenFileDialog1
            .FileName = ""
            .Title = "Select .apk file"
            .Filter = "(*.apk)|*.apk"
            .ShowDialog()
            If Not .FileName.Length = 0 Then
                Me.TextBox1.Text = .FileName
            End If
        End With
    End Sub

    Private Sub Button5_Click(sender As System.Object, e As System.EventArgs) Handles Button5.Click
        If Not Me.TextBox1.Text.Length = 0 Then
            Shell("java.exe -jar bin\apktool.jar -f -o " & """" & "Output\Patcher\" & "" & """" & " d -p bin\Framework -r -s " & """" & TextBox1.Text & """", AppWinStyle.NormalFocus, True)
            If File.Exists(Application.StartupPath & "\Output\Patcher\apktool.yml") = True Then
                If Me.RadioButton1.Checked = True Then
                    If File.Exists(Application.StartupPath & "\Output\Patcher\classes.dex") = True Then
                        If Environment.Is64BitOperatingSystem = False Then
                            Shell(Application.StartupPath & "\bin\GAR.exe", AppWinStyle.NormalFocus, True)
                        Else
                            Shell(Application.StartupPath & "\bin\GAR_x64.exe", AppWinStyle.NormalFocus, True)
                        End If
                        Shell("java -jar bin\apktool.jar -f b -o " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\RAdsun.apk" & """" & " -p bin\Framework " & " " & """" & "Output\Patcher" & """", AppWinStyle.NormalFocus, True)
                        My.Computer.FileSystem.DeleteDirectory(Application.StartupPath & "\Output\Patcher", FileIO.DeleteDirectoryOption.DeleteAllContents)
                        Shell("java -jar bin\signapk.jar bin\testkey.x509.pem bin\testkey.pk8 " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\RAdsun.apk" & """" & " " & """" & Replace(Me.TextBox1.Text, My.Computer.FileSystem.GetFileInfo(Me.TextBox1.Text).Extension, "-RAds.apk") & """", AppWinStyle.NormalFocus, True)
                        If File.Exists(My.Computer.FileSystem.SpecialDirectories.Temp & "\RAdsun.apk") = True Then
                            File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\RAdsun.apk")
                        End If
                        MsgBox("Done" & vbCrLf & "Output: " & """" & Replace(Me.TextBox1.Text, My.Computer.FileSystem.GetFileInfo(Me.TextBox1.Text).Extension, "-RAds.apk") & """", MsgBoxStyle.Information, "Info")
                    End If
                End If
            Else
                MsgBox("Error...?", MsgBoxStyle.Critical, "Error")
            End If
        Else
            MsgBox("Select .apk file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub
#End Region
End Class