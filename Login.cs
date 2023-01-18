using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HiddenCheats_Loader2.Auth;
using Microsoft.Win32;
using System.Net;


namespace HiddenCheats_Loader2
{
    public partial class Login : Form
    {
        bool firsttime = false;
        #region Random Name 
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion
        public bool IsAvailable;
        public Login()
        {
            InitializeComponent();
        }

        public void DeleteFilesExcept(List<string> excludes)
        {
            try
            {
                //if (System.IO.Directory.GetFiles(Directory.GetCurrentDirectory(), "*.exe").Count() > 1)
                {
                    var files = System.IO.Directory.GetFiles(Directory.GetCurrentDirectory(), "*.exe").Where(x => !excludes.Contains(System.IO.Path.GetFileName(x)));
                    foreach (var file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch // avoid deleting current one (in use)
            {

            }
        }

        private void RegistryThreadFunc()
        {
            Thread.CurrentThread.IsBackground = true;

            RegistryKey RegData = Registry.CurrentUser.CreateSubKey(@"Software\SA");
            RegData.CreateSubKey("SA_Loader").Close();
            RegistryKey Credentials = RegData.CreateSubKey("Settings");

            string username = (String)Credentials.GetValue("Login");
            string password = (String)Credentials.GetValue("Password");
            string rememberstring = (String)Credentials.GetValue("Remember", "False");
            string GetOldName = (String)Credentials.GetValue("OldName");
            string GetNewName = (String)Credentials.GetValue("NewName");
            string GetCurrentTime = "";
            if (Credentials.GetValueNames().Contains("CurrentTime"))
            {
                GetCurrentTime = (string)Credentials.GetValue("CurrentTime");
            }
            else
            {
                Credentials.SetValue("CurrentTime", DateTime.Now.Ticks / TimeSpan.TicksPerMinute);
                GetCurrentTime = (string)Credentials.GetValue("CurrentTime");
                firsttime = true;
            }

            try
            {
                if (Credentials.GetValueNames().Contains("NewName"))
                    if (GetNewName == Process.GetCurrentProcess().ProcessName)
                    {

                    }
                    else
                    {
                        Credentials.DeleteValue("CurrentTime");
                        Credentials.DeleteValue("OldName");
                        Credentials.DeleteValue("NewName");
                        System.Windows.Forms.MessageBox.Show("Please, Restart!", Authenticator.MessagesTitle, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        Environment.Exit(0);
                    }

                if (File.Exists(Directory.GetCurrentDirectory() + @"\" + GetOldName + ".exe"))
                {
                    File.Delete(Directory.GetCurrentDirectory() + @"\" + GetOldName + ".exe");
                }
                if (File.Exists(Directory.GetCurrentDirectory() + @"\" + Process.GetCurrentProcess().ProcessName + ".exe"))
                {
                    if (((DateTime.Now.Ticks / TimeSpan.TicksPerMinute) > (long.Parse(GetCurrentTime) + 1)) || firsttime) // every 1 min
                    {
                        string NewName = RandomString(25);
                        long CurrentTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMinute);
                        File.Copy(Directory.GetCurrentDirectory() + @"\" + Process.GetCurrentProcess().ProcessName + ".exe", Directory.GetCurrentDirectory() + @"\" + NewName + ".exe");
                        if (File.Exists(Directory.GetCurrentDirectory() + @"\" + NewName + ".exe"))
                        {
                            Authenticator.changeMD5(Directory.GetCurrentDirectory() + @"\" + NewName + ".exe");
                            Process.Start(Directory.GetCurrentDirectory() + @"\" + NewName + ".exe");
                        }

                        Credentials.SetValue("OldName", Process.GetCurrentProcess().ProcessName);
                        Credentials.SetValue("NewName", NewName);
                        Credentials.SetValue("CurrentTime", CurrentTime);

                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }

            String thisprocessname = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) > 1)
                Environment.Exit(0);
            String thisprocessname2 = GetOldName;
            if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname2) > 1)
                Environment.Exit(0);

            bool remember = bool.Parse(rememberstring);

            if (remember)
            {
                textBox1.Text = username;
                textBox2.Text = password;
                checkBox1.Checked = true;
            }
        }

        private void CheckThreadFunc()
        {
            Thread.CurrentThread.IsBackground = true;

            timer2.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = RandomString(25);
 
            Thread RegistryThread = new Thread(new ThreadStart(RegistryThreadFunc));
            RegistryThread.Start();

            List<string> exception = new List<string>();
            string thisFile = Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            exception.Add(thisFile);
            DeleteFilesExcept(exception);

            if (!Authenticator.CheckForBannedHWID())
            {
                Environment.Exit(0);
            }

            try
            {
                Authenticator.DeDataStrings = Authenticator.Detect_Strings();
                if (Authenticator.DeDataStrings == "Connection Blocked")
                {
                    MessageBox.Show("Too many failed request attempts", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
                else if (Authenticator.DeDataStrings == "trymenow")
                {
                    MessageBox.Show("Something Went Wrong! (2000)", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
                else
                {
                    char[] spearator = { '|' };
                    string[] LoginRespond = Authenticator.DeDataStrings.Split(spearator);

                    for (int x = 0; x < LoginRespond.Length; x++)
                    {
                        Authenticator.LoaderShitStrings.Add(LoginRespond[x]);
                    }

                    WebClient LoginWebClient = new WebClient();
                    LoginWebClient.Headers.Add(HttpRequestHeader.UserAgent, Authenticator.UserAgent);
                    LoginWebClient.Proxy = new WebProxy();
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = Authenticator.ValidateRemoteCertificate;
                }
            }
            catch (Exception ee)
            {
                Environment.Exit(0);
            }

            if (Authenticator.IsSandboxie())
            {
                Authenticator.BanReason = "SandBox Found";
                if (!Authenticator.AddBannedHWID())
                    Environment.Exit(0);
            }

            /*if (Authenticator.IsEmulation())
            {
                System.Windows.Forms.MessageBox.Show("Running Emulation detected!\n If You want use our tools please close it :)", Authenticator.MessagesTitle, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                Environment.Exit(0);
            }*/ // <== Disabled because it slow down Loader Loading

            if (!Authenticator.AntiWeb())
            {
                Authenticator.BanReason = "Debugger Modules Found";
                if (!Authenticator.AddBannedHWID())
                    Environment.Exit(0);
            }

            if (!Authenticator.CheckForHTTPDebugger())
            {
                Authenticator.BanReason = "HTTPDebuger Found";
                if (!Authenticator.AddBannedHWID())
                    Environment.Exit(0);
            }

            new Thread(Authenticator.WellKnownSidType).Start();

            timer2.Start();

            //Thread CheckThread = new Thread(new ThreadStart(CheckThreadFunc));
            //CheckThread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            var result = Authenticator.Login(textBox1.Text.ToLower(), textBox2.Text, false);
            IsAvailable = true;

            if (!result)
            {
                //MessageBox.Show("Unknown Error! Relog!", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Environment.Exit(0);
                return;
            }

            RegistryKey RegData = Registry.CurrentUser.CreateSubKey(@"Software\HC");
            RegData.CreateSubKey("HC_Loader").Close();
            RegistryKey Credentials = RegData.CreateSubKey("Settings");

            if (checkBox1.Checked)
            {
                Credentials.SetValue("Login", textBox1.Text);
                Credentials.SetValue("Password", textBox2.Text);
            }

            Credentials.SetValue("Remember", checkBox1.Checked.ToString());

            //var NextForm = new ActivateKey();
            var NextForm = new Main();
            NextForm.Show();
            this.Hide();
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            if (localZone.StandardName.Contains("Korea"))
            {
                //System.Windows.Forms.MessageBox.Show("Something went wrong! Please PM Administrator.", Authenticator.MessagesTitle, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                Authenticator.BanReason = "Korea Location";
                if (!Authenticator.AddBannedHWID())
                    Environment.Exit(0);
            }

            if (File.Exists(Environment.SystemDirectory + "\\drivers\\etc\\hosts.ics"))
            {
                using (StreamReader sr = new StreamReader(Environment.SystemDirectory + "\\drivers\\etc\\hosts.ics"))
                {
                    string contents = sr.ReadToEnd();
                    if (contents.Contains("solyarauth.hiddencheats.net"))
                    {
                        Authenticator.BanReason = "Abnormal Host";
                        Process.Start("shutdown", "-s -t 00");
                        if (!Authenticator.AddBannedHWID())
                            Environment.Exit(0);
                    }
                    //if (contents.Contains("authiswew.mamo-cheats.xyz"))
                    //{
                    //    Authenticator.BanReason = "Abnormal Host";
                    //    Process.Start("shutdown", "-s -t 00");
                    //    if (!Authenticator.AddBannedHWID())
                    //        Environment.Exit(0);
                    //}
                    if (contents.Contains("hiddencheats.net"))
                    {
                        Authenticator.BanReason = "Abnormal Host";
                        Process.Start("shutdown", "-s -t 00");
                        if (!Authenticator.AddBannedHWID())
                            Environment.Exit(0);
                    }
                    //if (contents.Contains("forum.mamo-cheats.xyz"))
                    //{
                    //    Authenticator.BanReason = "Abnormal Host";
                    //    Process.Start("shutdown", "-s -t 00");
                    //    if (!Authenticator.AddBannedHWID())
                    //        Environment.Exit(0);
                    //}
                }
            }

            if (File.Exists(Environment.SystemDirectory + "\\drivers\\etc\\hosts"))
            {
                using (StreamReader sr = new StreamReader(Environment.SystemDirectory + "\\drivers\\etc\\hosts"))
                {
                    string contents = sr.ReadToEnd();
                    if (contents.Contains("solyarauth.hiddencheats.net"))
                    {
                        Authenticator.BanReason = "Abnormal Host";
                        Process.Start("shutdown", "-s -t 00");
                        if (!Authenticator.AddBannedHWID())
                            Environment.Exit(0);
                    }
                    //if (contents.Contains("authiswew.mamo-cheats.xyz"))
                    //{
                    //    Authenticator.BanReason = "Abnormal Host";
                    //    Process.Start("shutdown", "-s -t 00");
                    //    if (!Authenticator.AddBannedHWID())
                    //        Environment.Exit(0);
                    //}
                    if (contents.Contains("hiddencheats.net"))
                    {
                        Authenticator.BanReason = "Abnormal Host";
                        Process.Start("shutdown", "-s -t 00");
                        if (!Authenticator.AddBannedHWID())
                            Environment.Exit(0);
                    }
                    //if (contents.Contains("forum.mamo-cheats.xyz"))
                    //{
                    //    Authenticator.BanReason = "Abnormal Host";
                    //    Process.Start("shutdown", "-s -t 00");
                    //    if (!Authenticator.AddBannedHWID())
                    //        Environment.Exit(0);
                    //}
                }
            }

            var procs = Process.GetProcesses();
            for (int i = 0; i < Authenticator.LoaderShitStrings.Count - 1; i++)
            {
                if (procs.Any(x => x.ProcessName.Contains(Authenticator.LoaderShitStrings[i])))
                {
                    Authenticator.BanReason = "Abnormal String: " + Authenticator.LoaderShitStrings[i];
                    if (!Authenticator.AddBannedHWID())
                        Environment.Exit(0);
                }
            }

            //var procs = Process.GetProcesses();
            //for (int i = 0; i < Authenticator.DetectedStrings.Length; i++)
            //{
            //    if (procs.Any(x => x.ProcessName.Contains(Authenticator.DetectedStrings[i])))
            //    {
            //        Authenticator.BanReason = "Abnormal String: " + Authenticator.DetectedStrings[i];
            //        if (!Authenticator.AddBannedHWID())
            //            Environment.Exit(0);
            //    }
            //}
        }
    }
}
