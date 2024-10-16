using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Security.Principal;
using System.Threading;

namespace VideoVirtualCamRouter
{
    public partial class MainForm : Form
    {
        private bool start_state = false;
        private MJPEGServer srvr;
        private bool tickedOk = true;

        public MainForm()
        {
            InitializeComponent();
            chromaBox.SelectedIndex = 1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReloadSourceCameras();
            ReloadDestinationCameras();
            ReloadBackgroundImage();

            if (destCamBox.Items.Count > 0) button3.Enabled = false;
        }

        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            };
        }

        private void ReloadSourceCameras()
        {
            int selected = sourceCamsBox.SelectedIndex;
            if (selected == -1) selected = 0;
            sourceCamsBox.Items.Clear();
            List<CameraDevice> devices = CameraDevice.GetAllConnectedCameras();
            foreach (CameraDevice device in devices)
                sourceCamsBox.Items.Add(device);
            try { sourceCamsBox.SelectedIndex = selected; } catch { };
            sourceCamsBox.Enabled = sourceCamsBox.Items.Count > 0;
        }

        private void ReloadDestinationCameras()
        {
            int selected = destCamBox.SelectedIndex;
            if (selected == -1) selected = 0;
            destCamBox.Items.Clear();

            string akvCamMan = GetAkvCamManager();

            if(!File.Exists(akvCamMan))
            {
                MessageBox.Show("AKVirtualCamera notfound, please install!", "Loading", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                System.Diagnostics.Process.Start("https://github.com/webcamoid/akvirtualcamera/releases");
                return;
            };

            string table = null;
            try { table = RunConsoleAndReadOutput(akvCamMan, "devices"); } catch { };
            if (string.IsNullOrEmpty(table)) return;
            List<List<string>> cams = ParseTable(table);
            if(cams.Count < 2) return;
            for (int i = 1; i < cams.Count; i++)
                destCamBox.Items.Add(new VirtualCameraDevice() { Name = cams[i][0], Description = cams[i][1] });

            try { destCamBox.SelectedIndex = selected; } catch { };
            destCamBox.Enabled = destCamBox.Items.Count > 0;
        }

        private string GetAkvCamManager()
        {
            string dllPath = null;
            try
            {
                RegistryKey baseKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.ClassesRoot, RegistryView.Registry64);
                if (baseKey != null)
                {
                    RegistryKey subKey = baseKey.OpenSubKey(@"CLSID\{1BFC8ECE-D2B7-43DA-FA0D-6BD955E689DA}\InprocServer32");
                    if (subKey != null)
                    {
                        dllPath = subKey.GetValue(null).ToString();
                        subKey.Close();
                    };
                    baseKey.Close();
                };
            }
            catch { };
            if (string.IsNullOrEmpty(dllPath))
            {
                string programFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
                return programFiles + @"\AkVirtualCamera\x64\AkVCamManager.exe";
            };
            string exePath = Path.Combine(System.IO.Path.GetDirectoryName(dllPath), "AkVCamManager.exe");
            return exePath;
        }

        private string RunConsoleAndReadOutput(string exe, string arguments, bool hide = true, bool redirect = true)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(exe, arguments);
            psi.WorkingDirectory = Path.GetDirectoryName(exe);
            if (redirect)
            {
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;                
            };
            if (hide)
            {
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                psi.CreateNoWindow = true;
            };
            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(psi);

            string result = null;
            if (redirect) result = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            return result;
        }

        private List<List<string>> ParseTable(string txt)
        {
            Regex rxnull = new Regex("^([+-]*)$");
            Regex rxrow = new Regex(@"^\|(?:([^\|]*)\|)*");

            List<List<string>> result  = new List<List<string>>();
            if(string.IsNullOrEmpty(txt)) return result;
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buff = System.Text.ASCIIEncoding.ASCII.GetBytes(txt);
                ms.Write(buff,0, buff.Length);
                ms.Flush();
                ms.Position = 0;
                using (StreamReader reader = new StreamReader(ms))
                {
                    while(!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (rxnull.IsMatch(line)) continue;
                        Match mx = rxrow.Match(line);
                        if (mx.Success)
                        {
                            List<string> record = new List<string>();
                            for (int i = 0; i < mx.Groups[1].Captures.Count; i++)
                                record.Add(mx.Groups[1].Captures[i].Value.Trim());
                            result.Add(record);
                        };
                    };
                };
            };
            return result;
        }

        private void ReloadBackgroundImage()
        {
            try { bgImage.Image = Image.FromFile(bgFile.Text); } catch { };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.png;*.bmp;*.jpg;*.jpeg;*.gif;*.tiff";
            try { openFileDialog.FileName = bgFile.Text; } catch { };
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                bgFile.Text = openFileDialog.FileName;
                ReloadBackgroundImage();
            };
            openFileDialog.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = pColBG.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pColBG.BackColor = colorDialog1.Color;
                textColor.Text = colorDialog1.Color.ToString();
                dkxce.RealCamToVirtualCamRouter.ChangeValue("background_color", colorDialog1.Color);
            };
        }

        private void velBar_Scroll(object sender, EventArgs e)
        {
            velVal.Text = velBar.Value.ToString();
            dkxce.RealCamToVirtualCamRouter.ChangeValue("velocity", velBar.Value);
        }

        private void trsBar_Scroll(object sender, EventArgs e)
        {
            trsVal.Text = trsBar.Value.ToString();
            dkxce.RealCamToVirtualCamRouter.ChangeValue("treshold", trsBar.Value);
        }

        private void ssBtn_Click(object sender, EventArgs e)
        {
            if(start_state)
            {
                ssBtn.Text = "Start";
                start_state = false;
                panel1.Enabled = true;
                Stop();
            }
            else
            {
                if (sourceCamsBox.Items.Count == 0) return;
                if (destCamBox.Items.Count == 0) return;
                ssBtn.Text = "Stop";
                start_state = true;
                panel1.Enabled = false;
                Start();
            };
        }

        private void Start()
        {
            dkxce.RealCamToVirtualCamRouter.akvcamman_path = GetAkvCamManager();
            dkxce.RealCamToVirtualCamRouter.virtualcam_fps = (int)fpsEdit.Value;
            dkxce.RealCamToVirtualCamRouter.default_width = (int)wiEdit.Value;
            dkxce.RealCamToVirtualCamRouter.default_height = (int)heEdit.Value;
            dkxce.RealCamToVirtualCamRouter.realcam_num = ((CameraDevice)sourceCamsBox.SelectedItem).OpenCvId;
            dkxce.RealCamToVirtualCamRouter.virtualcam_name = ((VirtualCameraDevice)destCamBox.SelectedItem).Name;
            dkxce.RealCamToVirtualCamRouter.Start();
        }

        private void Stop()
        {
            dkxce.RealCamToVirtualCamRouter.Stop();
        }

        private void chromaBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            dkxce.RealCamToVirtualCamRouter.ChangeValue("channel", chromaBox.SelectedIndex);
        }

        private void chbCK_CheckedChanged(object sender, EventArgs e)
        {
            dkxce.RealCamToVirtualCamRouter.ChangeValue("chromakey_remove", chbCK.Checked);
            
        }

        private void bgFile_TextChanged(object sender, EventArgs e)
        {
            if ((!string.IsNullOrEmpty(bgFile.Text)) && File.Exists(bgFile.Text))
                dkxce.RealCamToVirtualCamRouter.ChangeValue("background", bgFile.Text);
        }

        private void chbBG_CheckedChanged(object sender, EventArgs e)
        {
            dkxce.RealCamToVirtualCamRouter.ChangeValue("use_background", chbBG.Checked);
        }

        private void chbOvl_CheckedChanged(object sender, EventArgs e)
        {
            dkxce.RealCamToVirtualCamRouter.ChangeValue("ovelay", chbOvl.Checked);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            cSize.Text = $"{dkxce.RealCamToVirtualCamRouter.curr_wi} x {dkxce.RealCamToVirtualCamRouter.curr_he}";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (destCamBox.Items.Count > 0) return;

            if (!IsAdministrator())
            {
                MessageBox.Show("Run as administrator firstly!", "Setup Virtual Camera", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            };

            string akvCamMan = GetAkvCamManager();
            string table = null;
            string res = null;
            string deviceName = "dkxceVirtualCamera0";
            try { res = RunConsoleAndReadOutput(akvCamMan, $"add-device \"{deviceName}\"").Trim(); } catch { return; };
            if(!string.IsNullOrEmpty(res))
            {
                string[] words = res.Split(new char[] { ' ' });
                deviceName = words[words.Length-1];
            };
            try { res = RunConsoleAndReadOutput(akvCamMan, $"set-description {deviceName} \"Video Virtual Cam Router by dkxce\""); } catch { return; };
            Thread.Sleep(1000);
            try { res = RunConsoleAndReadOutput(akvCamMan, $"add-format {deviceName} RGB24 720 480 25"); } catch { return; };
            Thread.Sleep(1000);
            try { res = RunConsoleAndReadOutput(akvCamMan, $"add-format {deviceName} RGB24 640 480 25"); } catch { return; };
            Thread.Sleep(1000);
            // try { res = RunConsoleAndReadOutput(akvCamMan, $"set-picture \"{bgFile.Text}\""); } catch { return; };
            // Thread.Sleep(1000);
            try { res = RunConsoleAndReadOutput(akvCamMan, "update"); } catch { return; };
            Thread.Sleep(1000);
            try { res = RunConsoleAndReadOutput(akvCamMan, $"formats {deviceName}"); } catch { return; };
            Thread.Sleep(1000);
            ReloadDestinationCameras();
        }

       

        private void chbPreview_CheckedChanged(object sender, EventArgs e)
        {
            dkxce.RealCamToVirtualCamRouter.ChangeValue("use_preview", chbPreview.Checked);
            timer2.Interval = (int)(1000 / fpsEdit.Value);
            timer2.Enabled = chbMJpeg.Enabled = chbPreview.Checked;
            if (!chbPreview.Checked && previewBox.Image != null)
            {
                previewBox.Image.Dispose();
                previewBox.Image = null;
            };
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (!tickedOk) return;
            tickedOk = false;

            try
            {
                Image img = dkxce.RealCamToVirtualCamRouter.GetPreview();
                if (img != null) img = (Image)img.Clone();
                previewBox.Image = img;

                if (chbMJpeg.Checked)
                {
                    if (srvr == null) srvr = new MJPEGServer();
                    if (!srvr.IsRunning) srvr.Start((int)mjPort.Value);

                    if (img != null)
                    {
                        using (MemoryStream ms2 = new MemoryStream())
                        {
                            img.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg);
                            byte[] jpeg = ms2.ToArray();
                            srvr.Write(jpeg);
                        };
                    };
                }
                else
                {
                    if (srvr != null)
                    {
                        if (srvr.IsRunning) srvr.Stop();
                        srvr.Dispose();
                        srvr = null;
                    };
                };
            } 
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            tickedOk = true;
        }

        private void chbMJpeg_CheckedChanged(object sender, EventArgs e)
        {
            mjPort.Enabled = !(button4.Enabled = chbMJpeg.Checked);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start($"http://localhost:{mjPort.Value}/");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (srvr != null)
            {
                if (srvr.IsRunning) srvr.Stop();
                srvr.Dispose();
            };
            Stop();
            Application.Exit();
        }

        private void chbUVC_CheckedChanged(object sender, EventArgs e)
        {
            dkxce.RealCamToVirtualCamRouter.ChangeValue("use_virtcam", chbUVC.Checked); 
        }
    }
}
