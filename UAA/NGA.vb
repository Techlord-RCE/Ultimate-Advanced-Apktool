Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Threading
Imports System
Imports System.Security.Principal

Public Class NGA

#Region " Border Less Form Move"
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

#Region " Private Sub"
    Private Sub SetPer(ByVal FP As String, ByVal FPL As String)
        If Not FPL = 0 Then
            If File.Exists("bin\aapt.exe") Then

                File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\CMDNGA.bat", "bin\aapt.exe d permissions " & """" & FP & """" & " >>" & "bin\permissions.txt")

                If File.Exists(My.Computer.FileSystem.SpecialDirectories.Temp & "\CMDNGA.bat") Then
                    Shell(My.Computer.FileSystem.SpecialDirectories.Temp & "\CMDNGA.bat", AppWinStyle.Hide, True)
                End If
            Else
            End If
        End If
    End Sub

    ''' Check file Modified\Unmodified
    Private Sub GetStatus()
        Try
            Shell(Application.StartupPath & "\bin\7z.exe e -o" & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\" & """" & " """ & TextBox1.Text & """" & " " & "META-INF/CERT.RSA -y", AppWinStyle.Hide, True)
            ' Write file by size
            Shell(Application.StartupPath & "\Bin\GAR.exe", AppWinStyle.Hide, True)
            
            If File.Exists(My.Computer.FileSystem.SpecialDirectories.Temp & "\Check.nga") Then
                If System.Text.Encoding.Default.GetString(File.ReadAllBytes(My.Computer.FileSystem.SpecialDirectories.Temp & "\Check.nga")) = System.Text.Encoding.Default.GetString(My.Resources.UAA.Sign) Then
                    Label36.ForeColor = Color.Red
                    Label36.Text = "App Status : Modified"
                Else
                    Label36.ForeColor = Color.DarkGreen
                    Label36.Text = "App Status : Unmodified"
                End If
                File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\Check.nga")
            Else
                Label36.ForeColor = Color.DarkGreen
                Label36.Text = "App Status : Unmodified"
            End If
        Catch ex As Exception
            RichTextBox2.Text = ex.ToString
        End Try
    End Sub

    ' Drop .apk file in exe
    Private Sub DAFE()
        Dim args() As String = Environment.GetCommandLineArgs()
        Try
            ' Drop .apk file in exe
            If args.Length > 1 Then
                If Path.GetExtension(My.Computer.FileSystem.GetFileInfo(args(1)).ToString) = ".apk" Then
                    ' get path and file name
                    Dim fileinfo As New FileInfo(My.Computer.FileSystem.GetName(args(0)))
                    TextBox1.Text = fileinfo.DirectoryName & "\" & My.Computer.FileSystem.GetName(args(1))
                    ChDir(Application.StartupPath) ' file change dir
                Else
                    MsgBox("Drop '.apk' file only", MsgBoxStyle.Information, "")
                    Process.GetCurrentProcess.Kill()
                End If
            End If
        Catch ex As Exception
            Process.GetCurrentProcess.Kill()
        End Try

        ' Get Drop .apk in exe and set .apk permissions
        SetPer(TextBox1.Text, TextBox1.Text.Length)

        If File.Exists("bin\permissions.txt") Then
            RichTextBox1.LoadFile("bin\permissions.txt", RichTextBoxStreamType.PlainText) ' Set Permissions RichTextBox1

            File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\CMDNGA.bat")

            File.Delete("bin\permissions.txt")
            GetStatus()
        End If

        ChDir(Application.StartupPath)
    End Sub

    ' Check Java
    Private Sub CheckJava()
        Try
            Shell("java.exe -version", AppWinStyle.Hide)
        Catch ex As Exception
            Dim MB As New MBox()
            MB.ShowDialog()
            MB.Dispose()
        End Try
    End Sub

    ''' Color Picker
    Private Sub Customize(ByVal Name As Control, ByVal NameC As String)
        Dim ColorPicker As New ColorDialog()
        ColorPicker.AllowFullOpen = True
        ColorPicker.FullOpen = True
        ColorPicker.AnyColor = True
        If ColorPicker.ShowDialog = Windows.Forms.DialogResult.OK Then
            If Not ColorPicker.Color = Color.White Then
                Name.BackColor = ColorPicker.Color
                My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\UAA\Config", NameC, ColorPicker.Color.ToArgb)
            End If
        End If
    End Sub
#End Region

#Region "Function"

    ' Check Administrator
    Private Function IsAdministrator() As Boolean
        Dim windowsPrincipal As WindowsPrincipal = New WindowsPrincipal(WindowsIdentity.GetCurrent())
        Return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator)
    End Function

    Public Shared Function ReadALine(ByVal File_Path As String, ByVal TotalLine As Integer, ByVal Line2Read As Integer) As String
        Dim Buffer As Array
        Dim Line As String
        If TotalLine <= Line2Read Then
            Return ""
        End If
        Buffer = File.ReadAllLines(File_Path)
        Line = Buffer(Line2Read)
        Return Line
    End Function

    Public Shared Function GetNumberOfLine(ByVal File_Path As String) As Integer
        Dim SR As New StreamReader(File_Path)
        Dim NumberOfLineS As Integer
        Do While SR.Peek >= 0
            SR.ReadLine()
            NumberOfLineS += 1
        Loop
        Return NumberOfLineS
        SR.Close()
        SR.Dispose()
    End Function
#End Region

#Region " Form Load "
    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        CheckJava()
        DAFE()

        Try
            If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\UAA\Config", "Hide", "") = "1" Then
                Label25.Visible = False
                CheckBox13.Checked = True
            End If
        Catch ex As Exception
        End Try

        Try
            If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\UAA\Config", "Top", "") Then
                Panel1.BackColor = ColorTranslator.FromHtml(My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\UAA\Config", "Top", ""))
            End If
        Catch ex As Exception
        End Try

        Try
            If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\UAA\Config", "Bottom", "") Then
                Panel2.BackColor = ColorTranslator.FromHtml(My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\UAA\Config", "Bottom", ""))
            End If
        Catch ex As Exception
        End Try

        Try
            If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\UAA\Config", "AI", "") = "1" Then
                CheckBox19.Checked = True
            Else
                CheckBox19.Checked = False
            End If
        Catch ex As Exception
        End Try

        Try
            Me.Label25.Text = "User Name : " & Environment.UserName.ToString
            Me.Label26.Text = ".Net Runtme Version: " & Environment.Version.ToString
            My.Computer.FileSystem.CreateDirectory(Application.StartupPath & "\Output")
            My.Computer.FileSystem.CreateDirectory(Application.StartupPath & "\Output\Decompile")
            My.Computer.FileSystem.CreateDirectory(Application.StartupPath & "\Output\Recompile")
            My.Computer.FileSystem.CreateDirectory(Application.StartupPath & "\Output\Deodex")
            My.Computer.FileSystem.CreateDirectory(Application.StartupPath & "\Output\Deodex_ART")
            My.Computer.FileSystem.CreateDirectory(Application.StartupPath & "\Output\Jar_decompile")
        Catch ex As Exception
            Me.RichTextBox2.Text = ex.ToString
        End Try

        If IsAdministrator() = True Then
            Me.Label39.ForeColor = Color.Green
            Me.Button73.Enabled = True
            Me.Button74.Enabled = True
        Else
            Me.Button73.Enabled = False
            Me.Button74.Enabled = False
        End If

    End Sub
#End Region

#Region " Drag & Drop "

    Private Sub TextBox1_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox1.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        TextBox1.Text = (strArray(0))
        SetPer(TextBox1.Text, TextBox1.Text.Length)
        If File.Exists("bin\permissions.txt") Then
            RichTextBox1.LoadFile("bin\permissions.txt", RichTextBoxStreamType.PlainText) ' Write Permissions RichTextBox1

            File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\CMDNGA.bat")

            File.Delete("bin\permissions.txt")
            GetStatus()
        End If
        ChDir(Application.StartupPath)
    End Sub

    Private Sub TextBox1_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox1.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith(".apk")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox2_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox2.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If Directory.Exists((strArray(0))) And File.Exists((strArray(0)) & "\apktool.yml") Then
            TextBox2.Text = (strArray(0))
        Else
            MsgBox("Drop decompile .apk folder only", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub TextBox2_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox2.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith("")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox3_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox3.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        TextBox3.Text = (strArray(0))
    End Sub

    Private Sub TextBox3_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox3.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith(".dex")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox4_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox4.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        TextBox4.Text = (strArray(0))
    End Sub

    Private Sub TextBox4_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox4.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith(".jar")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox5_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox5.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If Directory.Exists((strArray(0))) Then
            TextBox5.Text = (strArray(0))
        Else
        End If
    End Sub

    Private Sub TextBox5_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox5.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith("")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox6_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox6.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If Directory.Exists((strArray(0))) Then
            TextBox6.Text = (strArray(0))
        Else
        End If
    End Sub

    Private Sub TextBox6_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox6.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith("")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox7_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox7.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If Directory.Exists((strArray(0))) Then
            TextBox7.Text = (strArray(0))
        Else
        End If
    End Sub

    Private Sub TextBox7_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox7.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith("")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox8_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox8.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If Directory.Exists((strArray(0))) Then
            TextBox8.Text = (strArray(0))
        Else
        End If
    End Sub

    Private Sub TextBox8_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox8.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith("")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox11_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox11.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If Directory.Exists((strArray(0))) Then
            TextBox11.Text = (strArray(0))
        Else
        End If
    End Sub

    Private Sub TextBox11_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox11.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith("")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox13_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox13.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        TextBox13.Text = (strArray(0))
    End Sub

    Private Sub TextBox13_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox13.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith(".png")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox15_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox15.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If Directory.Exists((strArray(0))) Then
            TextBox15.Text = (strArray(0))
        Else
        End If
    End Sub

    Private Sub TextBox15_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox15.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith("")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox16_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox16.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        TextBox16.Text = (strArray(0))
    End Sub

    Private Sub TextBox16_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox16.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith(".apk")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox17_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox17.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        TextBox17.Text = (strArray(0))
    End Sub

    Private Sub TextBox17_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox17.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith(".apk")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox23_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox23.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If Directory.Exists((strArray(0))) Then
            TextBox23.Text = (strArray(0))
        Else
        End If
    End Sub

    Private Sub TextBox23_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox23.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith("")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox24_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox24.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        TextBox24.Text = (strArray(0))
        TextBox25.Text = "Deodex_Output\" & Replace(My.Computer.FileSystem.GetFileInfo(TextBox24.Text).Name, My.Computer.FileSystem.GetFileInfo(TextBox24.Text).Extension, "")
    End Sub

    Private Sub TextBox24_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox24.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith(".odex")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox27_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox27.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If Directory.Exists((strArray(0))) Then
            TextBox27.Text = (strArray(0))
        Else
        End If
    End Sub

    Private Sub TextBox27_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox27.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith("")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox28_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox28.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If Directory.Exists((strArray(0))) Then
            TextBox28.Text = (strArray(0))
        Else
        End If
    End Sub

    Private Sub TextBox28_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox28.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith("")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox29_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox29.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If Directory.Exists((strArray(0))) Then
            TextBox29.Text = (strArray(0))
        Else
        End If
    End Sub

    Private Sub TextBox29_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox29.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith("")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox30_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox30.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        TextBox30.Text = (strArray(0))
    End Sub

    Private Sub TextBox30_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox30.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith(".odex")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox31_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox31.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        TextBox31.Text = (strArray(0))
    End Sub

    Private Sub TextBox31_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox31.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith(".oat")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox38_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox38.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        TextBox38.Text = (strArray(0))
    End Sub

    Private Sub TextBox38_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox38.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith(".png")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

    Private Sub TextBox40_DragDrop(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox40.DragDrop
        Dim strArray() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If Directory.Exists((strArray(0))) Then
            TextBox40.Text = (strArray(0))
        Else
        End If
    End Sub

    Private Sub TextBox40_DragEnter(sender As System.Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox40.DragEnter
        If (Not e.Data.GetDataPresent(DataFormats.FileDrop, False)) OrElse (Not CType(e.Data.GetData(DataFormats.FileDrop), String())(0).ToLower().EndsWith("")) Then
            Return
        End If
        e.Effect = DragDropEffects.All
    End Sub

#End Region

#Region "Button Click "

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Process.GetCurrentProcess.Kill()
    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        With OpenFileDialog1
            .FileName = ""
            .Title = "Choose .apk file"
            .Filter = "Android(*.apk)|*.apk"
            .ShowDialog()
            If Not .FileName.Length = 0 Then
                TextBox1.Text = .FileName
                If TextBox1.Text.Length Then
                    SetPer(TextBox1.Text, TextBox1.Text.Length)

                    If File.Exists("bin\permissions.txt") Then
                        RichTextBox1.LoadFile("bin\permissions.txt", RichTextBoxStreamType.PlainText) ' Write Permissions RichTextBox1

                        File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\CMDNGA.bat")

                        File.Delete("bin\permissions.txt")
                        GetStatus()
                    End If

                    ChDir(Application.StartupPath)

                End If
            End If
        End With
    End Sub

    Private Sub Button4_Click(sender As System.Object, e As System.EventArgs) Handles Button4.Click
        With FolderBrowserDialog1
            .SelectedPath = ""
            .ShowNewFolderButton = False
            .ShowDialog()
            If Not .SelectedPath.Length = 0 Then
                If File.Exists(.SelectedPath & "\apktool.yml") Then
                    TextBox2.Text = .SelectedPath
                Else
                    MsgBox("Select decompile .apk folder Only.", MsgBoxStyle.Information, "Info")
                End If
            End If
        End With
    End Sub

    Private Sub Button5_Click(sender As System.Object, e As System.EventArgs) Handles Button5.Click
        If Not TextBox1.Text.Length = 0 Then
            Try
                Dim decompilePath As String = "Output\decompile\" & Replace(My.Computer.FileSystem.GetFileInfo(TextBox1.Text).Name.ToString, My.Computer.FileSystem.GetFileInfo(TextBox1.Text).Extension.ToString, "") & """"

                If Me.CheckBox1.Checked = False And Me.CheckBox2.Checked = False Then
                    Shell("java.exe -jar bin\apktool.jar -f -o " & """" & decompilePath & " d -p bin\Framework " & """" & TextBox1.Text & """", AppWinStyle.NormalFocus)
                End If

                If Me.CheckBox1.Checked = True And Me.CheckBox2.Checked = False Then
                    Shell("java.exe -jar bin\apktool.jar -f -o " & """" & decompilePath & " d -p bin\Framework -r " & """" & TextBox1.Text & """", AppWinStyle.NormalFocus)
                End If

                If Me.CheckBox1.Checked = False And Me.CheckBox2.Checked = True Then
                    Shell("java.exe -jar bin\apktool.jar -f -o " & """" & decompilePath & " d -p bin\Framework -s " & """" & TextBox1.Text & """", AppWinStyle.NormalFocus)
                End If

                If Me.CheckBox1.Checked = True And Me.CheckBox2.Checked = True Then
                    Shell("java.exe -jar bin\apktool.jar -f -o " & """" & decompilePath & " d -p bin\Framework -r -s " & """" & TextBox1.Text & """", AppWinStyle.NormalFocus)
                End If

                If CheckBox7.Checked Then
                    TextBox2.Text = Application.StartupPath & "\" & decompilePath.Replace("""", "")
                End If

            Catch ex As Exception
                Me.RichTextBox2.Text = ex.ToString
            End Try

        Else
            MsgBox("Select .apk file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button6_Click(sender As System.Object, e As System.EventArgs) Handles Button6.Click
        If Not TextBox2.Text.Length = 0 Then
            Try
                Shell("java.exe -jar bin\apktool.jar -f b -o " & """" & TextBox2.Text & "-UnSign.apk" & """" & " -p bin\Framework " & " " & """" & TextBox2.Text & """", AppWinStyle.NormalFocus)
            Catch ex As Exception
                Me.RichTextBox2.Text = ex.ToString
            End Try
        Else
            MsgBox("Select decompile .apk folder.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button7_Click(sender As System.Object, e As System.EventArgs) Handles Button7.Click
        If Not TextBox2.Text.Length = 0 Then
            Try
                Shell("java -jar bin\apktool.jar -f b -o " & """" & TextBox2.Text & "-UnSign.apk" & """" & " -p bin\Framework " & " " & """" & TextBox2.Text & """", AppWinStyle.NormalFocus, True)

                Shell("java -jar bin\signapk.jar bin\testkey.x509.pem bin\testkey.pk8 " & """" & TextBox2.Text & "-UnSign.apk" & """" & " " & """" & TextBox2.Text & "-Sign.apk" & """", AppWinStyle.NormalFocus, True)

                File.Delete(TextBox2.Text & "-UnSign.apk")
            Catch ex As Exception
                Me.RichTextBox2.Text = ex.ToString
            End Try
        Else
            MsgBox("Select decompile .apk folder.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button8_Click(sender As System.Object, e As System.EventArgs) Handles Button8.Click
        If Not TextBox2.Text.Length = 0 Then
            Try
                Shell("java.exe -jar bin\apktool.jar -f b -o " & """" & TextBox2.Text & "-UnSign.apk" & """" & " -p bin\Framework " & " " & """" & TextBox2.Text & """", AppWinStyle.NormalFocus, True)
                Shell("java.exe -jar bin\signapk.jar bin\testkey.x509.pem bin\testkey.pk8 " & """" & TextBox2.Text & "-UnSign.apk" & """" & " " & """" & TextBox2.Text & "-Sign.apk" & """", AppWinStyle.NormalFocus, True)

                File.Delete(TextBox2.Text & "-UnSign.apk")
                Shell("bin\zipalign.exe -f 4 " & """" & TextBox2.Text & "-Sign.apk" & """" & " " & """" & TextBox2.Text & "-Sign_zipalign.apk" & """", AppWinStyle.NormalFocus, True)
                File.Delete(TextBox2.Text & "-Sign.apk")
            Catch ex As Exception
                Me.RichTextBox2.Text = ex.ToString
            End Try
        Else
            MsgBox("Select decompile .apk folder.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button9_Click(sender As System.Object, e As System.EventArgs) Handles Button9.Click
        If Not TextBox1.Text.Length = 0 Then
            Try
                Shell("java.exe -jar bin\signapk.jar bin\testkey.x509.pem bin\testkey.pk8 " & """" & TextBox1.Text & """" & " " & """" & TextBox1.Text & "-Sign.apk" & """", AppWinStyle.NormalFocus)
            Catch ex As Exception
                Me.RichTextBox2.Text = ex.ToString
            End Try
        Else
            MsgBox("Select .apk file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button10_Click(sender As System.Object, e As System.EventArgs) Handles Button10.Click
        If Not TextBox1.Text.Length = 0 Then
            Try
                Shell("java.exe -jar bin\signapk.jar bin\testkey.x509.pem bin\testkey.pk8 " & """" & TextBox1.Text & """" & " " & """" & TextBox1.Text & "-Sign.apk" & """", AppWinStyle.NormalFocus, True)
                Shell("bin\zipalign.exe -f 4 " & """" & TextBox1.Text & "-Sign.apk" & """" & " " & """" & TextBox1.Text & "-Sign_zipalign.apk" & """", AppWinStyle.NormalFocus, True)

                File.Delete(TextBox1.Text & "-Sign.apk")
            Catch ex As Exception
                Me.RichTextBox2.Text = ex.ToString
            End Try
        Else
            MsgBox("Select .apk file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button11_Click(sender As System.Object, e As System.EventArgs) Handles Button11.Click
        If Not TextBox1.Text.Length = 0 Then
            Try
                Shell("bin\zipalign.exe -f 4 " & """" & TextBox1.Text & """" & " " & """" & TextBox1.Text & "-zipalign.apk" & """", AppWinStyle.NormalFocus)
            Catch ex As Exception
                Me.RichTextBox2.Text = ex.ToString
            End Try
        Else
            MsgBox("Select .apk file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button12_Click(sender As System.Object, e As System.EventArgs) Handles Button12.Click
        Dim EditMainf As New AndroidManifest
        Dim XML As String = TextBox2.Text & "\" & "AndroidManifest.xml"
        If TextBox2.Text.Length Then
            If File.Exists(XML) Then
                EditMainf.ShowDialog()
                EditMainf.Dispose()
            End If
        Else
            MsgBox("Select decompile .apk folder.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button13_Click(sender As System.Object, e As System.EventArgs) Handles Button13.Click
        Dim EditInfo As New AInformation
        Dim YML As String = TextBox2.Text & "\" & "apktool.yml"
        If TextBox2.Text.Length Then
            If File.Exists(YML) Then
                EditInfo.ShowDialog()
                EditInfo.Dispose()
            End If
        Else
            MsgBox("Select decompile .apk folder.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button14_Click(sender As System.Object, e As System.EventArgs) Handles Button14.Click
        Try
            Shell("explorer.exe " & Application.StartupPath & "\Output\Decompile", AppWinStyle.NormalFocus)
        Catch ex As Exception
            Me.RichTextBox2.Text = ex.ToString
        End Try
    End Sub

    Private Sub Button15_Click(sender As System.Object, e As System.EventArgs) Handles Button15.Click
        If TextBox1.Text.Length Then
            Try
                If File.Exists(Application.StartupPath & "\bin\classes.dex") Then
                    File.Delete(Application.StartupPath & "\bin\classes.dex")
                End If
                Shell(Application.StartupPath & "\bin\7z.exe e -obin " & """" & TextBox1.Text & """" & " " & "classes.dex", AppWinStyle.NormalFocus, True)
                If File.Exists(Application.StartupPath & "\bin\classes.dex") Then
                    Shell("bin\dex2jar\d2j-dex2jar.bat " & """" & Application.StartupPath & "\bin\classes.dex" & """", AppWinStyle.NormalFocus, True)
                End If
                If File.Exists(Application.StartupPath & "\classes-dex2jar.jar") Then
                    File.Move(Application.StartupPath & "\classes-dex2jar.jar", Replace(My.Computer.FileSystem.GetFileInfo(TextBox1.Text).FullName, ".apk", ".jar"))
                End If
                If File.Exists(Application.StartupPath & "\bin\classes.dex") Then
                    File.Delete(Application.StartupPath & "\bin\classes.dex")
                End If
            Catch ex As Exception
                Me.RichTextBox2.Text = ex.ToString
            End Try
        Else
            MsgBox("Select .apk file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button16_Click(sender As System.Object, e As System.EventArgs) Handles Button16.Click
        Try
            Shell("CMD.exe", AppWinStyle.NormalFocus)
        Catch ex As Exception
            Me.RichTextBox2.Text = ex.ToString
        End Try
    End Sub

    Private Sub Button17_Click(sender As System.Object, e As System.EventArgs) Handles Button17.Click
        Try
            Shell("notepad.exe", AppWinStyle.NormalFocus)
        Catch ex As Exception
            Me.RichTextBox2.Text = ex.ToString
        End Try
    End Sub

    Private Sub Button18_Click(sender As System.Object, e As System.EventArgs) Handles Button18.Click
        Try
            Shell("calc.exe", AppWinStyle.NormalFocus)
        Catch ex As Exception
            Me.RichTextBox2.Text = ex.ToString
        End Try
    End Sub

    Private Sub Button19_Click(sender As System.Object, e As System.EventArgs) Handles Button19.Click
        With OpenFileDialog1
            .FileName = ""
            .Title = "Choose .dex file"
            .Filter = "Android(*.dex)|*.dex"
            .ShowDialog()
            If Not .FileName.Length = 0 Then
                TextBox3.Text = .FileName
            End If
        End With
    End Sub

    Private Sub Button20_Click(sender As System.Object, e As System.EventArgs) Handles Button20.Click
        With OpenFileDialog1
            .FileName = ""
            .Title = "Choose .jar file"
            .Filter = "(*.jar)|*.jar"
            .ShowDialog()
            If Not .FileName.Length = 0 Then
                TextBox4.Text = .FileName
            End If
        End With
    End Sub

    Private Sub Button21_Click(sender As System.Object, e As System.EventArgs) Handles Button21.Click
        With FolderBrowserDialog1
            .SelectedPath = ""
            .ShowNewFolderButton = False
            .ShowDialog()
            If Not .SelectedPath.Length = 0 Then
                TextBox5.Text = .SelectedPath
            End If
        End With
    End Sub

    Private Sub Button22_Click(sender As System.Object, e As System.EventArgs) Handles Button22.Click
        With FolderBrowserDialog1
            .SelectedPath = ""
            .ShowNewFolderButton = False
            .ShowDialog()
            If Not .SelectedPath.Length = 0 Then
                TextBox6.Text = .SelectedPath
            End If
        End With
    End Sub

    Private Sub Button23_Click(sender As System.Object, e As System.EventArgs) Handles Button23.Click
        With FolderBrowserDialog1
            .SelectedPath = ""
            .ShowNewFolderButton = False
            .ShowDialog()
            If Not .SelectedPath.Length = 0 Then
                TextBox8.Text = .SelectedPath
            End If
        End With
    End Sub

    Private Sub Button24_Click(sender As System.Object, e As System.EventArgs) Handles Button24.Click
        With FolderBrowserDialog1
            .SelectedPath = ""
            .ShowNewFolderButton = False
            .ShowDialog()
            If Not .SelectedPath.Length = 0 Then
                TextBox7.Text = .SelectedPath
            End If
        End With
    End Sub

    Private Sub Button25_Click(sender As System.Object, e As System.EventArgs) Handles Button25.Click
        If Not TextBox7.Text.Length = 0 Then
            Try
                Dim DI As New IO.DirectoryInfo(TextBox7.Text)
                Dim FI As IO.FileInfo() = DI.GetFiles("*.apk")
                Dim FIS As IO.FileInfo
                If CheckBox8.Checked = True Then
                    For Each FIS In FI
                        Shell("java.exe -jar bin\signapk.jar bin\testkey.x509.pem bin\testkey.pk8 " & """" & TextBox7.Text & "\" & FIS.ToString & """" & " " & """" & TextBox7.Text & "\" & FIS.ToString & "-Sign.apk" & """", AppWinStyle.NormalFocus, True)
                    Next
                Else
                    For Each FIS In FI
                        Shell("java.exe -jar bin\signapk.jar bin\testkey.x509.pem bin\testkey.pk8 " & """" & TextBox7.Text & "\" & FIS.ToString & """" & " " & """" & TextBox7.Text & "\" & FIS.ToString & "-Sign.apk" & """", AppWinStyle.NormalFocus)
                    Next
                End If

            Catch ex As Exception
            End Try
        Else
            MsgBox("Select .apk folder to sign.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button26_Click(sender As System.Object, e As System.EventArgs) Handles Button26.Click
        If Not TextBox10.Text.Length = 0 Then
            Try
                Shell("explorer.exe " & """" & TextBox10.Text & """", AppWinStyle.NormalFocus)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub Button27_Click(sender As System.Object, e As System.EventArgs) Handles Button27.Click
        If TextBox29.Text.Length Then
            If TextBox11.Text.Length Then
                If TextBox10.Text.Length Then
                    Try
                        Shell("java.exe -jar bin\oat2dex.jar -o " & """" & TextBox10.Text & """" & " devfw " & """" & TextBox29.Text & """", AppWinStyle.NormalFocus)
                    Catch ex As Exception
                        MsgBox(ex.ToString, MsgBoxStyle.OkOnly, "error")
                    End Try
                Else
                    MsgBox("Select output path", MsgBoxStyle.Information, "Info")
                End If
            Else
                MsgBox("Select odex folder", MsgBoxStyle.Information, "Info")
            End If
        Else
            MsgBox("Select framework folder", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button28_Click(sender As System.Object, e As System.EventArgs) Handles Button28.Click
        With FolderBrowserDialog1
            .SelectedPath = ""
            .ShowNewFolderButton = True
            .ShowDialog()
            If Not .SelectedPath.Length = 0 Then
                TextBox10.Text = .SelectedPath
            End If
        End With
    End Sub

    Private Sub Button29_Click(sender As System.Object, e As System.EventArgs) Handles Button29.Click
        With FolderBrowserDialog1
            .SelectedPath = ""
            .ShowNewFolderButton = False
            .ShowDialog()
            If Not .SelectedPath.Length = 0 Then
                TextBox11.Text = .SelectedPath
            End If
        End With
    End Sub

    Private Sub Button30_Click(sender As System.Object, e As System.EventArgs) Handles Button30.Click
        With OpenFileDialog1
            .FileName = ""
            .Filter = "Icon(*.png)|*.png"
            .Title = "Select Icon"
            .ShowDialog()
            If Not .FileName.Length = 0 Then
                TextBox13.Text = .FileName
                If File.Exists(.FileName) = True Then
                    Me.PictureBox2.ImageLocation = .FileName
                End If
            End If
        End With
    End Sub

    Private Sub Button31_Click(sender As System.Object, e As System.EventArgs) Handles Button31.Click
        If TextBox4.Text.Length Then
            If File.Exists(TextBox4.Text) Then
                If Directory.Exists(Application.StartupPath & "\Output\Jar_decompile") Then
                    Try
                        Shell("java -jar " & "bin\fernflower.jar " & """" & TextBox4.Text & """" & " " & """" & Application.StartupPath & "\Output\Jar_decompile""", AppWinStyle.NormalFocus, True)
                    Catch ex As Exception
                        Me.RichTextBox5.Text = ex.ToString
                    End Try
                    If File.Exists(Application.StartupPath & "\Output\Jar_decompile\" & My.Computer.FileSystem.GetFileInfo(TextBox4.Text).Name.ToString) Then
                        MsgBox("Successfully Decompile .jar", MsgBoxStyle.Information, "Info")
                    Else
                        MsgBox("Error", MsgBoxStyle.Critical, "")
                    End If
                Else
                    Try
                        FileSystem.MkDir(Application.StartupPath & "\Output\Jar_decompile")
                        Shell("java -jar " & "bin\fernflower.jar " & """" & TextBox4.Text & """" & " " & """" & Application.StartupPath & "\Output\Jar_decompile""", AppWinStyle.NormalFocus, True)
                    Catch ex As Exception
                        Me.RichTextBox5.Text = ex.ToString
                    End Try
                    If File.Exists(Application.StartupPath & "\Output\Jar_decompile\" & My.Computer.FileSystem.GetFileInfo(TextBox4.Text).Name.ToString) Then
                        MsgBox("Decompile .jar", MsgBoxStyle.Information, "Info")
                    Else
                        MsgBox("Error", MsgBoxStyle.Critical, "")
                    End If
                End If
            Else
                MsgBox("File not exist.", MsgBoxStyle.Information, "Info")
            End If
        Else
            MsgBox("Select .jar file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button32_Click(sender As System.Object, e As System.EventArgs) Handles Button32.Click
        If TextBox3.Text.Length Then
            Dim fileinfo As New FileInfo(TextBox3.Text)
            Dim GetPath = fileinfo.DirectoryName & "\" & My.Computer.FileSystem.GetName(TextBox3.Text) & ".baksmali"
            Try
                Shell("java.exe -jar bin\baksmali.jar -o " & """" & GetPath & """ " & """" & TextBox3.Text & """", AppWinStyle.NormalFocus)
            Catch ex As Exception
                Me.RichTextBox5.Text = ex.ToString
            End Try
        Else
            MsgBox("Select .dex file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button33_Click(sender As System.Object, e As System.EventArgs) Handles Button33.Click
        If TextBox3.Text.Length Then
            Try
                Shell("bin\dex2jar\d2j-dex2jar.bat " & """" & TextBox3.Text & """", AppWinStyle.NormalFocus)
            Catch ex As Exception
                Me.RichTextBox5.Text = ex.ToString
            End Try
        Else
            MsgBox("Select .dex file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button34_Click(sender As System.Object, e As System.EventArgs) Handles Button34.Click
        If TextBox4.Text.Length Then
            Try
                Shell("bin\dex2jar\d2j-jar2jasmin.bat " & """" & TextBox4.Text & """", AppWinStyle.NormalFocus)
            Catch ex As Exception
                Me.RichTextBox5.Text = ex.ToString
            End Try
        Else
            MsgBox("Select .jar file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button35_Click(sender As System.Object, e As System.EventArgs) Handles Button35.Click
        If Not TextBox8.Text.Length = 0 Then
            Try
                Dim DI As New IO.DirectoryInfo(TextBox8.Text)
                Dim FI As IO.FileInfo() = DI.GetFiles("*.apk")
                Dim FIS As IO.FileInfo
                If CheckBox9.Checked = True Then
                    For Each FIS In FI
                        Shell("bin\zipalign.exe -f 4 " & """" & TextBox8.Text & "\" & FIS.ToString & """" & " " & """" & TextBox8.Text & "\" & FIS.ToString & "-zipalign.apk" & """", AppWinStyle.NormalFocus, True)
                    Next
                Else
                    For Each FIS In FI
                        Shell("bin\zipalign.exe -f 4 " & """" & TextBox8.Text & "\" & FIS.ToString & """" & " " & """" & TextBox8.Text & "\" & FIS.ToString & "-zipalign.apk" & """", AppWinStyle.NormalFocus)
                    Next
                End If

            Catch ex As Exception
            End Try
        Else
            MsgBox("Select .apk folder to zipalign.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button36_Click(sender As System.Object, e As System.EventArgs) Handles Button36.Click
        If TextBox5.Text.Length Then
            Dim fileinfo As New FileInfo(TextBox5.Text)
            Dim GetPath = fileinfo.DirectoryName & "\" & My.Computer.FileSystem.GetName(TextBox5.Text) & ".smali.dex"
            Try
                Shell("java.exe -jar bin\smali.jar ass -o " & """" & GetPath & """ " & """" & TextBox5.Text & """", AppWinStyle.NormalFocus)
            Catch ex As Exception
                Me.RichTextBox5.Text = ex.ToString
            End Try
        Else
            MsgBox("Select baksmali folder.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button37_Click(sender As System.Object, e As System.EventArgs) Handles Button37.Click
        Try
            Shell("CMD.exe", AppWinStyle.NormalFocus)
        Catch ex As Exception
            Me.RichTextBox5.Text = ex.ToString
        End Try
    End Sub

    Private Sub Button38_Click(sender As System.Object, e As System.EventArgs) Handles Button38.Click
        If TextBox4.Text.Length Then
            Try
                Shell("bin\dex2jar\d2j-jar2dex.bat " & """" & TextBox4.Text & """", AppWinStyle.NormalFocus)
            Catch ex As Exception
                Me.RichTextBox5.Text = ex.ToString
            End Try
        Else
            MsgBox("Select .jar file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button39_Click(sender As System.Object, e As System.EventArgs) Handles Button39.Click
        If TextBox6.Text.Length Then
            Try
                Shell("bin\dex2jar\d2j-jasmin2jar.bat " & """" & TextBox6.Text & """", AppWinStyle.NormalFocus)
            Catch ex As Exception
                Me.RichTextBox5.Text = ex.ToString
            End Try
        Else
            MsgBox("Select jasmin folder.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button40_Click(sender As System.Object, e As System.EventArgs) Handles Button40.Click
        If TextBox4.Text.Length Then
            Try
                Shell("bin\dex2jar\d2j-jar2dex.bat " & """" & TextBox4.Text & """", AppWinStyle.NormalFocus)
            Catch ex As Exception
                Me.RichTextBox5.Text = ex.ToString
            End Try
        Else
            MsgBox("Select jasmin .jar file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button41_Click(sender As System.Object, e As System.EventArgs) Handles Button41.Click
        Try
            Shell("explorer.exe " & Application.StartupPath, AppWinStyle.NormalFocus)
        Catch ex As Exception
            Me.RichTextBox5.Text = ex.ToString
        End Try
    End Sub

    Private Sub Button42_Click(sender As System.Object, e As System.EventArgs) Handles Button42.Click
        With SaveFileDialog1
            .FileName = ""
            .Filter = "App(*.apk)|*.apk"
            .Title = "Save output app name"
            .ShowDialog()
            If Not .FileName.Length = 0 Then
                TextBox12.Text = .FileName
            End If
        End With
    End Sub

    Private Sub Button43_Click(sender As System.Object, e As System.EventArgs) Handles Button43.Click
        Try
            Shell("explorer.exe " & Application.StartupPath & "\Output\Recompile", AppWinStyle.NormalFocus)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button44_Click(sender As System.Object, e As System.EventArgs) Handles Button44.Click
        If CheckBox11.Checked = True Then
            Try
                For Each Dir As String In Directory.GetDirectories(TextBox9.Text)
                    Dim DI As New DirectoryInfo(Dir)
                    Shell("java.exe -jar bin\apktool.jar -f b -o " & """" & "Output\Recompile\" & DI.Name & "-UnSign.apk" & """" & " -p bin\Framework " & " " & """" & Dir & """", AppWinStyle.NormalFocus, True)
                Next
            Catch ex As Exception
            End Try
        Else
            Try
                For Each Dir As String In Directory.GetDirectories(TextBox9.Text)
                    Dim DI As New DirectoryInfo(Dir)
                    Shell("java.exe -jar bin\apktool.jar -f b -o " & """" & "Output\Recompile\" & DI.Name & "-UnSign.apk" & """" & " -p bin\Framework " & " " & """" & Dir & """", AppWinStyle.NormalFocus)
                Next
            Catch ex As Exception
            End Try
        End If

    End Sub

    Private Sub Button45_Click(sender As System.Object, e As System.EventArgs) Handles Button45.Click
        Shell("javaw.exe -jar " & Application.StartupPath & "\bin\jd-gui-1.4.0.jar", AppWinStyle.NormalFocus, False)
    End Sub

    Private Sub Button46_Click(sender As System.Object, e As System.EventArgs) Handles Button46.Click
        Dim Ot As New Other()
        Ot.ShowDialog()
        Ot.Dispose()
    End Sub

    Private Sub Button47_Click(sender As System.Object, e As System.EventArgs) Handles Button47.Click
        If Not TextBox15.Text.Length = 0 Then
            Try
                Dim DI As New IO.DirectoryInfo(TextBox15.Text)
                Dim FI As IO.FileInfo() = DI.GetFiles("*.apk")
                Dim FIS As IO.FileInfo
                Dim cmdArgs As String = "java.exe -jar bin\apktool.jar -f -o " & """" & "Output\decompile\" & Replace(FIS.ToString, My.Computer.FileSystem.GetFileInfo(FIS.ToString).Extension.ToString, "") & """" & " d -p bin\Framework "

                If CheckBox10.Checked = True Then
                    If Me.CheckBox5.Checked = False And Me.CheckBox3.Checked = False Then
                        For Each FIS In FI
                            Shell(cmdArgs & """" & TextBox15.Text & "\" & FIS.ToString & """", AppWinStyle.NormalFocus, True)
                        Next
                    End If

                    If Me.CheckBox5.Checked = True And Me.CheckBox3.Checked = False Then
                        For Each FIS In FI
                            Shell("java.exe -jar bin\apktool.jar -f  -o " & """" & "Output\decompile\" & Replace(FIS.ToString, My.Computer.FileSystem.GetFileInfo(FIS.ToString).Extension.ToString, "") & """" & " d -p bin\Framework" & "-r " & """" & TextBox15.Text & "\" & FIS.ToString & """", AppWinStyle.NormalFocus, True)
                        Next
                    End If

                    If Me.CheckBox5.Checked = False And Me.CheckBox3.Checked = True Then
                        For Each FIS In FI
                            Shell(cmdArgs & "-s " & """" & TextBox15.Text & "\" & FIS.ToString & """", AppWinStyle.NormalFocus, True)
                        Next
                    End If

                    If Me.CheckBox5.Checked = True And Me.CheckBox3.Checked = True Then
                        For Each FIS In FI
                            Shell(cmdArgs & "-r -s " & """" & TextBox15.Text & "\" & FIS.ToString & """", AppWinStyle.NormalFocus, True)
                        Next
                    End If
                Else
                    If Me.CheckBox5.Checked = False And Me.CheckBox3.Checked = False Then
                        For Each FIS In FI
                            Shell(cmdArgs & """" & TextBox15.Text & "\" & FIS.ToString & """", AppWinStyle.NormalFocus)
                        Next
                    End If

                    If Me.CheckBox5.Checked = True And Me.CheckBox3.Checked = False Then
                        For Each FIS In FI
                            Shell(cmdArgs & "-r " & """" & TextBox15.Text & "\" & FIS.ToString & """", AppWinStyle.NormalFocus)
                        Next
                    End If

                    If Me.CheckBox5.Checked = False And Me.CheckBox3.Checked = True Then
                        For Each FIS In FI
                            Shell(cmdArgs & "-s " & """" & TextBox15.Text & "\" & FIS.ToString & """", AppWinStyle.NormalFocus)
                        Next
                    End If

                    If Me.CheckBox5.Checked = True And Me.CheckBox3.Checked = True Then
                        For Each FIS In FI
                            Shell(cmdArgs & "-r -s " & """" & TextBox15.Text & "\" & FIS.ToString & """", AppWinStyle.NormalFocus)
                        Next
                    End If
                End If

            Catch ex As Exception
            End Try
        Else
            MsgBox("Select .apk folder to decompile.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button48_Click(sender As System.Object, e As System.EventArgs) Handles Button48.Click
        If TextBox16.Text.Length Then
            Try
                Shell("java.exe -jar bin\apktool.jar if -p bin\Framework " & """" & TextBox16.Text & """", AppWinStyle.NormalFocus)
            Catch ex As Exception
                MsgBox(ex.ToString, MsgBoxStyle.OkOnly, "error")
            End Try
        Else
            MsgBox("Select framework-res.apk file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button49_Click(sender As System.Object, e As System.EventArgs) Handles Button49.Click
        With FolderBrowserDialog1
            .SelectedPath = ""
            .ShowNewFolderButton = False
            .ShowDialog()
            If Not .SelectedPath.Length = 0 Then
                TextBox15.Text = .SelectedPath
            End If
        End With
    End Sub

    Private Sub Button50_Click(sender As System.Object, e As System.EventArgs) Handles Button50.Click
        MsgBox("7z : Igor Pavlov" & vbCrLf & "Apktool : Ryszard Wisniewski, Connor Tumbleson" & vbCrLf & "ApkShellext2 : kkguo" & vbCrLf & "baksmali / smali : JesusFreke" & vbCrLf & "oat2dex : Softdx" & vbCrLf & "dex2jar : Bob Pan", MsgBoxStyle.OkOnly, "Credits")
    End Sub

    Private Sub Button51_Click(sender As System.Object, e As System.EventArgs) Handles Button51.Click
        With OpenFileDialog1
            .FileName = ""
            .Title = "Choose framework-res.apk file"
            .Filter = "Android|framework-res.apk"
            .ShowDialog()
            If Not .FileName.Length = 0 Then
                TextBox16.Text = .FileName
            End If
        End With
    End Sub

    Private Sub Button52_Click(sender As System.Object, e As System.EventArgs) Handles Button52.Click
        If TextBox17.Text.Length Then
            Try
                Shell("java.exe -jar bin\apktool.jar if -p bin\Framework " & """" & TextBox17.Text & """", AppWinStyle.NormalFocus)
            Catch ex As Exception
                MsgBox(ex.ToString, MsgBoxStyle.OkOnly, "error")
            End Try
        Else
            MsgBox("Select .apk file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button53_Click(sender As System.Object, e As System.EventArgs)
        With OpenFileDialog1
            .FileName = ""
            .Title = "Select jd-gui jar"
            .Filter = "Java Decompiler|*.jar"
            .ShowDialog()
            If Not .FileName.Length = 0 Then
                If File.Exists(.FileName) Then
                    My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\UAA\Config", "JD", .FileName.ToString)
                End If
            End If
        End With
    End Sub

    Private Sub Button54_Click(sender As System.Object, e As System.EventArgs) Handles Button54.Click
        With OpenFileDialog1
            .FileName = ""
            .Title = "Choose .apk file"
            .Filter = "Android|*.apk"
            .ShowDialog()
            If Not .FileName.Length = 0 Then
                TextBox17.Text = .FileName
            End If
        End With
    End Sub

    Private Sub Button55_Click(sender As System.Object, e As System.EventArgs) Handles Button55.Click
        If Not TextBox20.Text.Length = 0 Then ' URL
            If Not TextBox12.Text.Length = 0 Then ' Output
                If Not TextBox18.Text.Length = 0 Then ' Package Name 2
                    If Not TextBox19.Text.Length = 0 Then ' Package Name 3
                        If Not TextBox21.Text.Length = 0 Then ' App Title
                            If Not TextBox22.Text.Length = 0 Then ' About Dialog Text
                                Try
                                    My.Computer.FileSystem.WriteAllBytes(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool.apk", My.Resources.UAA.NGA, False)

                                    If File.Exists(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool.apk") = True Then
                                        Shell("java.exe -jar bin\apktool.jar -f -o " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool" & """" & " d -p bin\Framework " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool.apk" & """", AppWinStyle.Hide, True)
                                        File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool.apk")
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\AndroidManifest.xml", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\AndroidManifest.xml"), "com.nga.nga", "com." & TextBox18.Text & "." & TextBox19.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\res\values\strings.xml", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\res\values\strings.xml"), "NextGenerationApktool", TextBox21.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\res\values\strings.xml", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\res\values\strings.xml"), "NGA By Gorav Gupta", TextBox22.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\a.smali", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\a.smali"), "Lcom/nga/nga", "Lcom/" & TextBox18.Text & "/" & TextBox19.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\b.smali", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\b.smali"), "Lcom/nga/nga", "Lcom/" & TextBox18.Text & "/" & TextBox19.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\c.smali", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\c.smali"), "Lcom/nga/nga", "Lcom/" & TextBox18.Text & "/" & TextBox19.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\d.smali", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\d.smali"), "Lcom/nga/nga", "Lcom/" & TextBox18.Text & "/" & TextBox19.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\MainActivity.smali", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\MainActivity.smali"), "Lcom/nga/nga", "Lcom/" & TextBox18.Text & "/" & TextBox19.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\MainActivity.smali", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\MainActivity.smali"), "Next Generation Apktool", TextBox20.Text))
                                        My.Computer.FileSystem.RenameDirectory(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga", TextBox18.Text)
                                        My.Computer.FileSystem.RenameDirectory(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\" & TextBox18.Text & "\nga", TextBox19.Text)
                                        If Not TextBox13.Text.Length = 0 Then
                                            My.Computer.FileSystem.CopyFile(TextBox13.Text, My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\res\drawable\ic_launcher.png", True)
                                        End If
                                        If CheckBox4.Checked = True Then
                                            If File.Exists(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\AndroidManifest.xml") = True Then
                                                Dim ARPe As New ARP()
                                                ARPe.ShowDialog()
                                                ARPe.Dispose()
                                            End If
                                        End If
                                        Shell("java.exe -jar bin\apktool.jar -f -o " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool.apk" & """" & " b -p bin\Framework " & " " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool" & """", AppWinStyle.NormalFocus, True)
                                        Shell("java.exe -jar bin\signapk.jar bin\testkey.x509.pem bin\testkey.pk8 " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool.apk" & """" & " " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool-Sign.apk" & """", AppWinStyle.NormalFocus, True)

                                        File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool.apk")
                                        Shell("bin\zipalign.exe -f 4 " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool-Sign.apk" & """" & " " & """" & TextBox12.Text & """", AppWinStyle.NormalFocus, True)
                                        File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool-Sign.apk")
                                        My.Computer.FileSystem.DeleteDirectory(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool", FileIO.DeleteDirectoryOption.DeleteAllContents)
                                        MsgBox("All Done Successfully", MsgBoxStyle.Information, "")
                                    End If

                                Catch ex As Exception
                                    MsgBox(ex.ToString, MsgBoxStyle.Critical)
                                End Try
                            End If
                        End If
                    End If
                End If
            End If
        End If

    End Sub

    Private Sub Button56_Click(sender As System.Object, e As System.EventArgs) Handles Button56.Click
        Try
            Shell("explorer.exe " & Application.StartupPath, AppWinStyle.NormalFocus)
        Catch ex As Exception
            Me.RichTextBox2.Text = ex.ToString
        End Try
    End Sub

    Private Sub Button57_Click(sender As System.Object, e As System.EventArgs) Handles Button57.Click
        With FolderBrowserDialog1
            .SelectedPath = ""
            .ShowNewFolderButton = False
            .ShowDialog()
            If Not .SelectedPath.Length = 0 Then
                TextBox23.Text = .SelectedPath
            End If
        End With
    End Sub

    Private Sub Button58_Click(sender As System.Object, e As System.EventArgs) Handles Button58.Click
        With OpenFileDialog1
            .FileName = ""
            .Filter = "Odex File (*.odex)|*.odex"
            .ShowDialog()
            If Not .FileName.Length = 0 Then
                TextBox24.Text = .FileName
                TextBox25.Text = "Output\Deodex\" & Replace(My.Computer.FileSystem.GetFileInfo(TextBox24.Text).Name, My.Computer.FileSystem.GetFileInfo(TextBox24.Text).Extension, "")
            End If
        End With
    End Sub

    Private Sub Button59_Click(sender As System.Object, e As System.EventArgs) Handles Button59.Click
        Customize(Me.Panel1, "Top")
    End Sub

    Private Sub Button60_Click(sender As System.Object, e As System.EventArgs) Handles Button60.Click
        If Not TextBox23.Text.Length = 0 Then
            If Not TextBox24.Text.Length = 0 Then
                Try
                    Shell("java.exe -jar bin\dex2jar\baksmali.jar -d " & """" & TextBox23.Text & """" & " -o " & """" & TextBox25.Text & """" & " -x " & """" & TextBox24.Text & """", AppWinStyle.NormalFocus, False)
                Catch ex As Exception
                    RichTextBox5.Text = ex.ToString
                End Try
            Else
                MsgBox("Select Odex File", MsgBoxStyle.Information, "Info")
            End If
        Else
            MsgBox("Select Framework Path", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button61_Click(sender As System.Object, e As System.EventArgs) Handles Button61.Click
        If Not TextBox25.Text.Length = 0 Then
            Shell("explorer.exe " & TextBox25.Text, AppWinStyle.NormalFocus, False)
        End If
    End Sub

    Private Sub Button62_Click(sender As System.Object, e As System.EventArgs) Handles Button62.Click
        With FolderBrowserDialog1
            .SelectedPath = ""
            .ShowNewFolderButton = False
            .ShowDialog()
            If Not .SelectedPath.Length = 0 Then
                TextBox27.Text = .SelectedPath
            End If
        End With
    End Sub

    Private Sub Button63_Click(sender As System.Object, e As System.EventArgs) Handles Button63.Click
        With FolderBrowserDialog1
            .SelectedPath = ""
            .ShowNewFolderButton = False
            .ShowDialog()
            If Not .SelectedPath.Length = 0 Then
                TextBox28.Text = .SelectedPath
            End If
        End With
    End Sub

    Private Sub Button64_Click(sender As System.Object, e As System.EventArgs) Handles Button64.Click
        With FolderBrowserDialog1
            .SelectedPath = ""
            .ShowNewFolderButton = True
            .ShowDialog()
            If Not .SelectedPath.Length = 0 Then
                TextBox26.Text = .SelectedPath
            End If
        End With
    End Sub

    Private Sub Button65_Click(sender As System.Object, e As System.EventArgs) Handles Button65.Click
        If Not TextBox28.Text.Length = 0 Then
            If Not TextBox27.Text.Length = 0 Then
                If Not TextBox26.Text.Length = 0 Then
                    Try
                        Dim DI As New IO.DirectoryInfo(TextBox27.Text)
                        Dim FI As IO.FileInfo() = DI.GetFiles("*.odex")
                        Dim FIS As IO.FileInfo
                        If CheckBox12.Checked = True Then
                            For Each FIS In FI
                                Shell("java.exe -jar bin\baksmali.jar -d " & """" & TextBox28.Text & """" & " -o " & """" & TextBox26.Text & "\" & Replace(FIS.ToString, My.Computer.FileSystem.GetFileInfo(FIS.ToString).Extension, "") & """" & " -x " & """" & TextBox27.Text & "\" & FIS.ToString & """", AppWinStyle.NormalFocus, True)
                            Next
                        Else
                            For Each FIS In FI
                                Shell("java.exe -jar bin\baksmali.jar -d " & """" & TextBox28.Text & """" & " -o " & """" & TextBox26.Text & "\" & Replace(FIS.ToString, My.Computer.FileSystem.GetFileInfo(FIS.ToString).Extension, "") & """" & " -x " & """" & TextBox27.Text & "\" & FIS.ToString & """", AppWinStyle.NormalFocus, False)
                            Next
                        End If
                    Catch ex As Exception
                    End Try
                Else
                    MsgBox("Select Output Path", MsgBoxStyle.Information, "Info")
                End If
            Else
                MsgBox("Select Odex Folder", MsgBoxStyle.Information, "Info")
            End If
        Else
            MsgBox("Select Framework Path", MsgBoxStyle.Information, "Info")
        End If

    End Sub

    Private Sub Button66_Click(sender As System.Object, e As System.EventArgs) Handles Button66.Click
        If Not TextBox26.Text.Length = 0 Then
            Shell("explorer.exe " & TextBox26.Text, AppWinStyle.NormalFocus, False)
        End If
    End Sub

    Private Sub Button67_Click(sender As System.Object, e As System.EventArgs) Handles Button67.Click
        Customize(Me.Panel2, "Bottom")
    End Sub

    Private Sub Button68_Click(sender As System.Object, e As System.EventArgs) Handles Button68.Click
        With FolderBrowserDialog1
            .SelectedPath = ""
            .ShowNewFolderButton = False
            .ShowDialog()
            If Not .SelectedPath.Length = 0 Then
                TextBox29.Text = .SelectedPath
            End If
        End With
    End Sub

    Private Sub Button69_Click(sender As System.Object, e As System.EventArgs) Handles Button69.Click
        With OpenFileDialog1
            .FileName = ""
            .Filter = "(*.odex)|*.odex"
            .ShowDialog()
            If Not .FileName.Length = 0 Then
                TextBox30.Text = .FileName
            End If
        End With
    End Sub

    Private Sub Button70_Click(sender As System.Object, e As System.EventArgs) Handles Button70.Click
        If TextBox31.Text.Length Then
            Try
                Shell("java.exe -jar bin\oat2dex2.jar " & """" & TextBox31.Text & """ " & """" & TextBox31.Text & ".dex""", AppWinStyle.NormalFocus)
            Catch ex As Exception
                MsgBox(ex.ToString, MsgBoxStyle.OkOnly, "error")
            End Try
        Else
            MsgBox("Select .oat file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button71_Click(sender As System.Object, e As System.EventArgs) Handles Button71.Click
        With OpenFileDialog1
            .FileName = ""
            .Filter = "(*.oat)|*.oat"
            .ShowDialog()
            If Not .FileName.Length = 0 Then
                TextBox31.Text = .FileName
            End If
        End With
    End Sub

    Private Sub Button72_Click(sender As System.Object, e As System.EventArgs) Handles Button72.Click
        If TextBox30.Text.Length Then
            Try
                Shell("java.exe -jar bin\oat2dex2.jar " & """" & TextBox30.Text & """ " & """" & TextBox30.Text & ".dex""", AppWinStyle.NormalFocus)
            Catch ex As Exception
                Me.RichTextBox5.Text = ex.ToString
            End Try
        Else
            MsgBox("Select .odex file.", MsgBoxStyle.Information, "Info")
        End If
    End Sub

    Private Sub Button73_Click(sender As System.Object, e As System.EventArgs) Handles Button73.Click
        Try
            My.Computer.Registry.SetValue("HKEY_CLASSES_ROOT\.apk\Shell\Open with UAA\command", "", """" & Application.ExecutablePath & """" & " " & """" & "%1" & """")
            My.Computer.Registry.SetValue("HKEY_CLASSES_ROOT\APKfile\Shell\Open with UAA\command", "", """" & Application.ExecutablePath & """" & " " & """" & "%1" & """")
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button74_Click(sender As System.Object, e As System.EventArgs) Handles Button74.Click
        Try
            My.Computer.Registry.ClassesRoot.DeleteSubKeyTree(".apk\Shell\Open with UAA")
            My.Computer.Registry.ClassesRoot.DeleteSubKeyTree("APKfile\Shell\Open with UAA")
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button75_Click(sender As System.Object, e As System.EventArgs) Handles Button75.Click
        If Not TextBox32.Text.Length = 0 Then
            If Not TextBox33.Text.Length = 0 Then
                If Not TextBox34.Text.Length = 0 Then
                    If Not TextBox35.Text.Length = 0 Then
                        If Not TextBox36.Text.Length = 0 Then
                            If Not TextBox39.Text.Length = 0 Then
                                Try
                                    My.Computer.FileSystem.WriteAllBytes(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool.apk", My.Resources.UAA.NGA, False)

                                    If File.Exists(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool.apk") = True Then
                                        Shell("java.exe -jar bin\apktool.jar -f -o " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool" & """" & " d -p bin\Framework " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool.apk" & """", AppWinStyle.Hide, True)
                                        File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool.apk")
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\AndroidManifest.xml", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\AndroidManifest.xml"), "com.nga.nga", "com." & TextBox36.Text & "." & TextBox35.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\res\values\strings.xml", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\res\values\strings.xml"), "NextGenerationApktool", TextBox33.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\res\values\strings.xml", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\res\values\strings.xml"), "NGA By Gorav Gupta", TextBox32.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\a.smali", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\a.smali"), "Lcom/nga/nga", "Lcom/" & TextBox36.Text & "/" & TextBox35.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\b.smali", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\b.smali"), "Lcom/nga/nga", "Lcom/" & TextBox36.Text & "/" & TextBox35.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\c.smali", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\c.smali"), "Lcom/nga/nga", "Lcom/" & TextBox36.Text & "/" & TextBox35.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\d.smali", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\d.smali"), "Lcom/nga/nga", "Lcom/" & TextBox36.Text & "/" & TextBox35.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\MainActivity.smali", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\MainActivity.smali"), "Lcom/nga/nga", "Lcom/" & TextBox36.Text & "/" & TextBox35.Text))
                                        File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\MainActivity.smali", Replace(File.ReadAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga\nga\MainActivity.smali"), "Next Generation Apktool", "file:///android_asset/index.html"))
                                        My.Computer.FileSystem.RenameDirectory(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\nga", TextBox36.Text)
                                        My.Computer.FileSystem.RenameDirectory(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\smali\com\" & TextBox36.Text & "\nga", TextBox35.Text)
                                        My.Computer.FileSystem.CopyFile(TextBox34.Text, My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\assets\index.html", True)
                                        If Not TextBox40.Text.Length = 0 Then
                                            My.Computer.FileSystem.CopyDirectory(TextBox40.Text, My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\assets\" & My.Computer.FileSystem.GetDirectoryInfo(TextBox40.Text).Name, True)
                                        End If
                                        If Not TextBox38.Text.Length = 0 Then
                                            My.Computer.FileSystem.CopyFile(TextBox38.Text, My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\res\drawable\ic_launcher.png", True)
                                        End If
                                        If CheckBox6.Checked = True Then
                                            If File.Exists(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool\AndroidManifest.xml") = True Then
                                                Dim ARPe As New ARP()
                                                ARPe.ShowDialog()
                                                ARPe.Dispose()
                                            End If
                                        End If
                                        Shell("java.exe -jar bin\apktool.jar -f -o " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool.apk" & """" & " b -p bin\Framework " & " " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool" & """", AppWinStyle.NormalFocus, True)
                                        Shell("java.exe -jar bin\signapk.jar bin\testkey.x509.pem bin\testkey.pk8 " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool.apk" & """" & " " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool-Sign.apk" & """", AppWinStyle.NormalFocus, True)

                                        File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool.apk")
                                        Shell("bin\zipalign.exe -f 4 " & """" & My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool-Sign.apk" & """" & " " & """" & TextBox39.Text & """", AppWinStyle.NormalFocus, True)
                                        File.Delete(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool-Sign.apk")
                                        My.Computer.FileSystem.DeleteDirectory(My.Computer.FileSystem.SpecialDirectories.Temp & "\NextGenerationApktool", FileIO.DeleteDirectoryOption.DeleteAllContents)
                                        MsgBox("All Done Successfully", MsgBoxStyle.Information, "")
                                    End If

                                Catch ex As Exception
                                    MsgBox(ex.ToString, MsgBoxStyle.Critical)
                                End Try
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub Button76_Click(sender As System.Object, e As System.EventArgs) Handles Button76.Click
        With OpenFileDialog1
            .FileName = ""
            .Filter = "Icon(*.png)|*.png"
            .Title = "Select Icon"
            .ShowDialog()
            If Not .FileName.Length = 0 Then
                TextBox38.Text = .FileName
                If File.Exists(.FileName) = True Then
                    Me.PictureBox3.ImageLocation = .FileName
                End If
            End If
        End With
    End Sub

    Private Sub Button77_Click(sender As System.Object, e As System.EventArgs) Handles Button77.Click
        With SaveFileDialog1
            .FileName = ""
            .Filter = "App(*.apk)|*.apk"
            .Title = "Save output app name"
            .ShowDialog()
            If Not .FileName.Length = 0 Then
                TextBox39.Text = .FileName
            End If
        End With
    End Sub

    Private Sub Button78_Click(sender As System.Object, e As System.EventArgs) Handles Button78.Click
        With OpenFileDialog1
            .FileName = ""
            .Filter = "(*.html)|*.html"
            .ShowDialog()
            .Title = "Select HTML File"
            If Not .FileName.Length = 0 Then
                TextBox34.Text = .FileName
            End If
        End With
    End Sub

    Private Sub Button79_Click(sender As System.Object, e As System.EventArgs) Handles Button79.Click
        With FolderBrowserDialog1
            .SelectedPath = ""
            .ShowNewFolderButton = False
            .ShowDialog()
            If Not .SelectedPath.Length = 0 Then
                TextBox40.Text = .SelectedPath
            End If
        End With
    End Sub

#End Region

#Region " CheckBox "
    Private Sub CheckBox13_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBox13.CheckedChanged
        If CheckBox13.Checked = True Then
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\UAA\Config", "Hide", "1")
            Me.Label25.Visible = False
        Else
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\UAA\Config", "Hide", "0")
            Me.Label25.Visible = True
        End If
    End Sub

    Private Sub CheckBox19_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBox19.CheckedChanged

        If CheckBox19.Checked = True Then
            Try
                If Not My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\UAA\Config", "AI", "") = "1" Then
                    Shell(Application.StartupPath & "\bin\install.bat", AppWinStyle.Hide, True)

                    MsgBox("Restart explorer.", MsgBoxStyle.Information, "Info")
                    Shell("TASKKILL /F /IM explorer.exe", AppWinStyle.Hide, True)
                    Shell("explorer.exe", AppWinStyle.Hide, False)
                    My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\UAA\Config", "AI", "1")
                End If
            Catch ex As Exception
            End Try
        Else
            Try
                If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\UAA\Config", "AI", "") = "1" Then
                    MsgBox("Restart explorer.", MsgBoxStyle.Information, "Info")

                    Shell("TASKKILL /F /IM explorer.exe", AppWinStyle.Hide, True)

                    Shell(Application.StartupPath & "\bin\uninstall.bat", AppWinStyle.Hide, True)
                   
                    Shell("explorer.exe", AppWinStyle.Hide, False)
                    My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\UAA\Config", "AI", "0")
                End If
            Catch ex As Exception
            End Try
        End If
    End Sub

#End Region

End Class
