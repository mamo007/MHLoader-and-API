using System;
using System.IO;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using HiddenCheats_Loader2.Auth;
using System.Net;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using Lunar;

namespace HiddenCheats_Loader2
{
    public partial class Main : Form
    {
        static bool injonce = false; static bool buttoncheck = false;
        private AES encrypt = new AES();
        #region AES Decrypt/Encrypt
        /*
            //Encrypt
            byte[] str = File.ReadAllBytes("C:\\Users\\Mamo\\Desktop\\sBrJF3GdqfZ8MeLPV5npaYNKzD2HTSuUcgxAj9h47RmW_cf_w_vip.dll");
            string s = AES.Encrypt(Convert.ToBase64String(str), "v$9$rgb$ir1w0bl1futp");
            File.WriteAllText("C:\\Users\\Mamo\\Desktop\\sBrJF3GdqfZ8MeLPV5npaYNKzD2HTSuUcgxAj9h47RmW_cf_w_vip.txt", s);
            
            //Decrypt
            string str = File.ReadAllText("C:\\Users\\Mamo\\Desktop\\sBrJF3GdqfZ8MeLPV5npaYNKzD2HTSuUcgxAj9h47RmW_cf_w_vip.txt");
            string s = AES.Decrypt(str, "v$9$rgb$ir1w0bl1futp");
            File.WriteAllBytes("C:\\Users\\Mamo\\Desktop\\sBrJF3GdqfZ8MeLPV5npaYNKzD2HTSuUcgxAj9h47RmW_cf_w_vip.dll", Convert.FromBase64String(s));
         */
        #endregion

        #region Random Name 
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion

        static string DllPath = Environment.GetEnvironmentVariable("windir") + @"\Prefetch\SVCHOST.EXE-" + RandomString(8) + ".pf";
        static string DllPath2 = Environment.SystemDirectory + "\\drivers\\etc\\hosts"; //Environment.GetEnvironmentVariable("windir") + @"\Prefetch\SVCHOST.EXE-" + RandomString(8) + ".pf";
        Process[] tempProcess;
        Lunar.LibraryMapper mapper;
        public Main()
        {
            InitializeComponent();
        }


        private const int TOKEN_ADJUST_PRIVILEGES = 0x0020;
        private const int TOKEN_QUERY = 0x00000008;
        private const int SE_PRIVILEGE_ENABLED = 0x00000002;
        [StructLayout(LayoutKind.Sequential)]
        private struct LUID
        {
            public UInt32 LowPart;
            public Int32 HighPart;
        }
        private struct LUID_AND_ATTRIBUTES
        {
            public LUID Luid;
            public UInt32 Attributes;
        }
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, ref LUID lpLuid);
        struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;
            [MarshalAs(UnmanagedType.ByValArray)]
            public LUID_AND_ATTRIBUTES[] Privileges;
        }
        // Use this signature if you want the previous state information returned
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle,
           [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
           ref TOKEN_PRIVILEGES NewState,
           UInt32 BufferLengthInBytes,
           IntPtr prev,
           IntPtr relen);
        [DllImport("advapi32", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);

        [DllImport("kernel32", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        private static extern bool CloseHandle(IntPtr handle);
        private bool EnableDebugPriv()
        {
            IntPtr hToken = IntPtr.Zero;
            if (!OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref hToken))
            {
                return false;
            }
            LUID luid = new LUID();
            if (!LookupPrivilegeValue(null, "SeDebugPrivilege", ref luid))
            {
                CloseHandle(hToken);
                return false;
            }
            TOKEN_PRIVILEGES tp = new TOKEN_PRIVILEGES();
            tp.PrivilegeCount = 1;
            tp.Privileges = new LUID_AND_ATTRIBUTES[1];
            tp.Privileges[0].Luid = luid;
            tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
            if (!AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero))
            {
                return false;
            }
            CloseHandle(hToken);
            return true;
        }

        public static void StopService(string strServiceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(strServiceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch
            {
                //...
            }
        }

        public static void StartService(string strServiceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(strServiceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {
                //...
            }
        }

        public void injectLib(string ProcessName, byte[] dllbytes)
        {
            try
            {
                EnableDebugPriv();

                var process = Process.GetProcessesByName(ProcessName)[0];

                var flags = MappingFlags.DiscardHeaders;

                mapper = new LibraryMapper(process, dllbytes, flags);

                mapper.MapLibrary();

                if (File.Exists(DllPath) || injonce)
                    File.Delete(DllPath);

                //ServiceController service = new ServiceController("xhunter1");
                //if (service.Status == ServiceControllerStatus.Stopped)
                //{
                //    MessageBox.Show("Abort, Close Game, Restart loader, etc."); // Game will cause xigncode error if main service is stopped.
                //    Environment.Exit(0);
                //    // Close game
                //}
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TokenThreadFunc()
        {
            Thread.CurrentThread.IsBackground = true;

            try
            {
                Authenticator.DeDataToken = Authenticator.TokenCheckFunc(Authenticator.DisplayName, Authenticator.LoaderProdName, Authenticator.GroupId, Authenticator.tpmp);
                if (Authenticator.DeDataToken == "Something Went Wrong! (1000)")
                {
                    // Does it's job somehow.

                    WebClient LoginWebClient2 = new WebClient();
                    LoginWebClient2.Headers.Add(HttpRequestHeader.UserAgent, Authenticator.UserAgent);
                    LoginWebClient2.Proxy = new WebProxy();
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = Authenticator.ValidateRemoteCertificate;
                    //MessageBox.Show("Something Went Wrong! (1000)", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Environment.Exit(0);
                }
                else
                {
                    WebClient LoginWebClient2 = new WebClient();
                    LoginWebClient2.Headers.Add(HttpRequestHeader.UserAgent, Authenticator.UserAgent);
                    LoginWebClient2.Proxy = new WebProxy();
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = Authenticator.ValidateRemoteCertificate;
                }
            }
            catch
            {
                Environment.Exit(0);
            }
        }
        private bool GrantAccess(string fullPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(fullPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.NoPropagateInherit,
                AccessControlType.Allow));

            dInfo.SetAccessControl(dSecurity);
            return true;
        }
        public static void DownloadDll(string LoaderDllLink)
        {
            try
            {
                WebClient LoginWebClient = new WebClient();
                LoginWebClient.Proxy = new WebProxy();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = Authenticator.ValidateRemoteCertificate;
                LoginWebClient.DownloadFile(LoaderDllLink, DllPath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }
        public static void DownloadDll2(string LoaderDllLink)
        {
            try
            {
                WebClient LoginWebClient = new WebClient();
                LoginWebClient.Proxy = new WebProxy();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = Authenticator.ValidateRemoteCertificate;
                LoginWebClient.DownloadFile(LoaderDllLink, DllPath2);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.Text = RandomString(25);

            if (Authenticator.LoaderVersion == "1.0") // Changes every update.
            {
                label10.Text = Authenticator.LoaderHWIDLT;
                label3.Text = Authenticator.LoaderHWIDMN;
                if (label10.Text == "Available")
                {
                    label10.Text = "Available. You can use another PC.";
                    label10.ForeColor = Color.Lime;
                }
                //label10.Text = Authenticator.GetMemberHWID();
                pictureBox1.Image = Authenticator.Avatar;
                label17.Text = Authenticator.DisplayName;
                label18.Text = Authenticator.GroupId;
                label8.Text = Authenticator.LoaderNews;
                label19.Text = Authenticator.LoaderEXDate;
                label4.Text = Authenticator.LoaderProdName;
                label7.Text = Authenticator.GetIPAddress();

                try
                {
                    GrantAccess(Environment.GetEnvironmentVariable("windir") + @"\Prefetch\");
                    //GrantAccess(Environment.SystemDirectory + "\\drivers\\etc\\");
                }
                catch (Exception ee)
                {
                    Environment.Exit(0);
                }

                try
                {
                    Authenticator.DeData3 = Authenticator.DecryptThisShit3(Authenticator.DisplayName, Authenticator.GroupId, Authenticator.tpmp);
                    if (Authenticator.DeData3 == "Connection Blocked")
                    {
                        MessageBox.Show("Too many failed request attempts", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                    }
                    else if (Authenticator.DeData3 == "trymenow")
                    {
                        MessageBox.Show("Something Went Wrong! (3)", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                    }
                    else
                    {
                        ListViewItem Item;

                        try
                        {
                            char[] spearator = { '|' };
                            string[] LoginRespond = Authenticator.DeData3.Split(spearator);

                            Authenticator.LoaderGroupColor = LoginRespond[9];

                            #region Group Colors
                            if (Authenticator.LoaderGroupColor == "White")
                                label18.ForeColor = Color.White;
                            else if (Authenticator.LoaderGroupColor == "Red")
                                label18.ForeColor = Color.FromArgb(255, 0, 0);
                            else if (Authenticator.LoaderGroupColor == "Blue")
                                label18.ForeColor = Color.Blue;
                            else if (Authenticator.LoaderGroupColor == "Orange")
                                label18.ForeColor = Color.Orange;
                            else
                                Environment.Exit(0);
                            #endregion

                            for (int x = 0; x < LoginRespond.Length; x += 12)
                            {
                                string[] str = new string[ListView1.Columns.Count];
                                int y = 0;

                                str[0] = LoginRespond[x + 3];
                                str[1] = LoginRespond[x];
                                str[2] = LoginRespond[x + 4];
                                str[3] = LoginRespond[x + 1];
                                Item = new ListViewItem(str);
                                ListView1.Items.Add(Item);

                                Authenticator.LoaderDll.Add(LoginRespond[x + 2]);//download links
                                Authenticator.LoaderAESKey.Add(LoginRespond[x + 6] + LoginRespond[x + 7] + LoginRespond[x + 8]);//aes keys
                                Authenticator.LoaderProcess.Add(LoginRespond[x + 5]);
                                Authenticator.LoaderHosts.Add(LoginRespond[x + 10]);
                                Authenticator.LoaderServicename.Add(LoginRespond[x + 11]);
                            }
                        }
                        catch (Exception es) // To prevent out of index error
                        {
                            //MessageBox.Show(es.Message);
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

                foreach (ListViewItem lvw in ListView1.Items)
                {
                    lvw.UseItemStyleForSubItems = false;

                    for (int i = 0; i < ListView1.Columns.Count; i++)
                    {
                        if (lvw.SubItems[i].Text.ToString() == Authenticator.CheatStatus[1])
                        {
                            lvw.SubItems[i].ForeColor = Color.Red;
                        }
                        if (lvw.SubItems[i].Text.ToString() == Authenticator.CheatStatus[0])
                        {
                            lvw.SubItems[i].ForeColor = Color.Green;
                        }
                        if (lvw.SubItems[i].Text.ToString() == "VIP")
                        {
                            lvw.SubItems[i].ForeColor = Color.Orange;
                        }
                    }
                }
            }
            else
            {
                this.Close();
                System.Windows.Forms.MessageBox.Show("There's a newer version.", Authenticator.MessagesTitle, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                var UpdaterForm = new Updater();
                UpdaterForm.Show();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (buttoncheck)
            {
                if (ListView1.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem Item in ListView1.SelectedItems)
                    {
                        if (Item.SubItems[3].Text == Authenticator.CheatStatus[0])
                        {
                            ServiceController service = new ServiceController(Authenticator.LoaderServicename[Item.Index]);
                            Process[] pCheck = Process.GetProcessesByName(Authenticator.LoaderProcess[Item.Index]);
                            if (pCheck.Length > 0)
                            {
                                if (!injonce)
                                {
                                    string s = AES.Decrypt(File.ReadAllText(DllPath), Authenticator.LoaderAESKey[Item.Index]);

                                    timer2.Stop();
                                    try
                                    {
                                        //StartService(Authenticator.LoaderServicename[Item.Index], 1);
                                    }
                                    catch
                                    {
                                        // ...
                                    }
                                    injectLib(Authenticator.LoaderProcess[Item.Index], Convert.FromBase64String(s));

                                    if (Authenticator.LoaderProcess[Item.Index] == Process.GetCurrentProcess().ProcessName)
                                    {
                                        this.Visible = false;
                                        this.ShowInTaskbar = false;
                                        label16.Text = "Press on Load cheat.";

                                        injonce = true;
                                    }
                                    else
                                    {
                                        Environment.Exit(0);
                                    }
                                }
                            }
                            else
                            {
                                if (injonce)
                                {
                                    this.Visible = true;
                                    this.ShowInTaskbar = true;
                                    ////mapper.UnmapLibrary();
                                    button1.Enabled = true;
                                    ListView1.Enabled = true;

                                    ListView1.SelectedItems.Clear();
                                }
                                else
                                {
                                    label16.Text = "Waiting for " + Authenticator.LoaderProcess[Item.Index] + ".exe";
                                    button1.Enabled = false;
                                    ListView1.Enabled = false;
                                }
                            }
                        }
                        else if (Item.SubItems[3].Text == Authenticator.CheatStatus[1])
                        {
                            label16.Text = Item.SubItems[1].Text + " Cheat is Offline.";
                            button1.Enabled = false;
                            ListView1.Enabled = false;
                        }
                    }
                }
                else
                {
                    buttoncheck = false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!buttoncheck)
            {
                injonce = false;
                button1.Enabled = false;
                ListView1.Enabled = false;
                string CheckOS;

#if WIN64
                CheckOS = Authenticator.CheatOS[1];
#else
                CheckOS = Authenticator.CheatOS[0];
#endif

                if (ListView1.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem Item in ListView1.SelectedItems)
                    {
                        if (Item.SubItems[2].Text == CheckOS)
                        {
                            if (Item.SubItems[3].Text == Authenticator.CheatStatus[0])
                            {
                                DownloadDll(Authenticator.LoaderDll[Item.Index]);
                                Thread TokenThread = new Thread(new ThreadStart(TokenThreadFunc));
                                TokenThread.Start();
                                timer1.Start();
                                //timer2.Start();

                                if (File.Exists(Environment.SystemDirectory + "\\drivers\\etc\\hosts"))
                                    File.Delete(Environment.SystemDirectory + "\\drivers\\etc\\hosts");

                                DownloadDll2(Authenticator.LoaderHosts[Item.Index]);
                            }
                            else
                            {
                                label16.Text = Item.SubItems[1].Text + " Cheat is Offline.";
                                button1.Enabled = false;
                                ListView1.Enabled = false;
                            }
                        }
                        else
                        {
                            if (Environment.Is64BitProcess)
                                MessageBox.Show("Please, Run x86 version!", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            else
                                MessageBox.Show("Please, Run x64 version!", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            Environment.Exit(0);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You have to select a cheat!", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ListView1.Enabled = true;
                    button1.Enabled = true;
                    //Environment.Exit(0);
                }

                buttoncheck = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Authenticator.OpenBrowser("https://hiddencheats.net/index.php?/support/");
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (File.Exists(DllPath) || injonce)
                File.Delete(DllPath);

            Environment.Exit(0);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Authenticator.OpenBrowser("https://hiddencheats.net/index.php?/store/");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (ListView1.SelectedItems.Count > 0)
            {
                foreach (ListViewItem Item in ListView1.SelectedItems)
                {
                    if (Item.SubItems[3].Text == Authenticator.CheatStatus[0])
                    {
                        ServiceController service = new ServiceController(Authenticator.LoaderServicename[Item.Index]);
                        if (service.Status == ServiceControllerStatus.StartPending || service.Status == ServiceControllerStatus.Running)
                        {
                            try
                            {
                                StopService(Authenticator.LoaderServicename[Item.Index], 1);
                            }
                            catch
                            {
                                // ...
                            }
                        }
                    }
                }
            }
        }
    }
}
