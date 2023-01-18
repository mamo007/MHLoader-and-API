using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

#pragma warning disable 1591
#pragma warning disable 0618

namespace HiddenCheats_Loader2.Auth
{
    class Authenticator
    {
        #region "Detect Strings"
        //public static string[] DetectedStrings = { "Debug", "HTTP", "Fiddler", "mongod", "Fid", "bug", "HTTPDebugger", "Hacker", "Wire", "Shark", /*"Engine",*/ "HxD", "ResourceHacker", "Httpanalyzerstdv7", "NetworkTrafficView", "smsniff", "Hocus", "Pocus", "Charles", "Hex", "Detect it Easy", "crack", "Crack", "Unpack", "Reko", "Decompiler", "decompiler", "Fuser", "DisAssembuler", "Assembuler", "Sandbox", "PE Explorer", "engine", "DotNetResolver", "dnspy", "IaT FiXeR", "IDMGrHlp", "IDA", "Reflector", "crypt", "OllyDbg", "OLLYDBG", "Oll", "ILSpy", "Resolver", "de4dot", "Exeinfo", "Dump", "PETools", "PE Tools", "Fixer", "Universal", "www.uinc.ru", "TMAC", "xCheat", "XCHEAT", "xcheat", "XCheat", "PEiD", "Suck my Source Code" };
        #endregion
        public static string[] CheatStatus = { "Online", "Offline", "Maintenance" };
        public static string[] CheatOS = { "x86", "x64" };

        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        #region HashChanger

        // Original Source: https://github.com/ewwink/MD5-Hash-Changer/blob/master/MD5HashChanger/
        public static void changeMD5(string fileNames)
        {
            Random random = new Random();
            Thread.Sleep(1000);

            int num = random.Next(2, 7);
            byte[] extraByte = new byte[num];
            for (int j = 0; j < num; j++)
            {
                extraByte[j] = (byte)0;
            }
            long fileSize = new FileInfo(fileNames).Length;
            if (fileSize == 0L)
            {
                //this.dgvMD5.Rows[i].Cells[3].Value = "Empty";
            }
            else
            {
                using (FileStream fileStream = new FileStream(fileNames, FileMode.Append))
                {
                    fileStream.Write(extraByte, 0, extraByte.Length);
                }
                int bufferSize = fileSize > 1048576L ? 1048576 : 4096;
                string md5hash = "";
                using (MD5 md = MD5.Create())
                {
                    using (FileStream fileStream2 = new FileStream(fileNames, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize))
                    {
                        md5hash = BitConverter.ToString(md.ComputeHash(fileStream2)).Replace("-", "");
                    }
                }
                //this.dgvMD5.Rows[i].Cells[2].Value = md5hash;
                //MessageBox.Show(md5hash);
            }
        }

        public static void checkMD5(string[] fileNames)
        {
            int index = 0;
            foreach (string name in fileNames)
            {
                string md5hash = "";
                long fileSize = new FileInfo(name).Length;
                int bufferSize = fileSize > 1048576L ? 1048576 : 4096;
                using (MD5 md = MD5.Create())
                {
                    using (FileStream fileStream = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize))
                    {
                        md5hash = BitConverter.ToString(md.ComputeHash(fileStream)).Replace("-", "");
                    }
                }
                index++;

                //this.labelItem.Text = index.ToString();
                //this.progressBarStatus.Value = index;
                //this.dgvMD5.Rows.Add(new object[] { name, md5hash, "", "idle" });
                //this.dgvMD5.Rows[0].Selected = false;
            }
        }

        #endregion

        #region "Avatar"
        public static Image GetImageFromPicPath(string strUrl)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidateRemoteCertificate);
            using (WebResponse wrFileResponse = WebRequest.Create(strUrl).GetResponse())
            using (Stream objWebStream = wrFileResponse.GetResponseStream())
            {
                MemoryStream ms = new MemoryStream();
                objWebStream.CopyTo(ms, 8192);
                return System.Drawing.Image.FromStream(ms);
            }
        }
        #endregion

        #region "Data"
        internal static string UserAgent = "123";
        static string LogsURL = "";
        static string AuthURL = "https://solyarauth.sharp-aim.xyz/Auth/auth.php";
        static string TimeAPIURL = "https://solyarauth.sharp-aim.xyz/API/GetTime.php";
        static string IPAPIURL = "https://solyarauth.sharp-aim.xyz/API/GetIP.php";
        static string BanChecker = "https://solyarauth.sharp-aim.xyz/Auth/loaderban.php";
        static string BanChecker2 = "https://solyarauth.sharp-aim.xyz/Auth/loaderban2.php";
        static string InfoURL = "https://solyarauth.sharp-aim.xyz/Auth/loadercheats.php";
        static string LoaderInfoURL = "https://solyarauth.sharp-aim.xyz/Auth/loaderinfo.php";
        static string LoaderToken = "https://solyarauth.sharp-aim.xyz/Auth/checktoken.php";
        static string LoaderStrings = "https://solyarauth.sharp-aim.xyz/Auth/loaderstrings.php";

        //static string ActivateKeyURL = "https://solyarauth.sharp-aim.xyz/Auth/keyverification.php";
        static string ActivateKeyURL = "https://sharp-aim.xyz/applications/nexus/interface/licenses/?activate&key=";

        public static string MessagesTitle = "SharpAIM";
        #endregion

        #region "Decryption"

        public static string Decrypt(string text, string password, string salt)
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));
            SymmetricAlgorithm algorithm = new TripleDESCryptoServiceProvider();
            byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);
            ICryptoTransform transform = algorithm.CreateDecryptor(rgbKey, rgbIV);
            using (MemoryStream buffer = new MemoryStream(Convert.FromBase64String(text)))
            {
                using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
        #endregion

        #region Encryption

        public static string Encrypt(string value, string password, string salt)
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));
            SymmetricAlgorithm algorithm = new TripleDESCryptoServiceProvider();
            byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);
            ICryptoTransform transform = algorithm.CreateEncryptor(rgbKey, rgbIV);
            using (MemoryStream buffer = new MemoryStream())
            {
                using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
                    {
                        writer.Write(value);
                    }
                }
                return Convert.ToBase64String(buffer.ToArray());
            }
        }

        public static string PHPMd5Hash(string pass)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] input = Encoding.UTF8.GetBytes(pass);
                byte[] hash = md5.ComputeHash(input);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }
        #endregion

        #region "UserAccount"
        internal static Image Avatar { get; private set; }
        internal static string DisplayName { get; private set; }
        internal static string GroupName { get; private set; }
        internal static string GroupId { get; private set; }
        internal static string AvatarURL { get; private set; }
        internal static string MemberHWID { get; private set; }
        internal static string MemberMachine { get; private set; }
        internal static string MemberOSVersion { get; private set; }
        internal static string LoaderNews { get; private set; }
        internal static string LoaderHWIDLT { get; private set; }
        internal static string LoaderHWIDMN { get; private set; }
        internal static string LoaderEXDate { get; private set; }
        internal static string LoaderProdName { get; private set; }
        internal static string LoaderVersion;
        internal static string LoaderExeUpdate;
        internal static string LoaderGroupColor;
        internal static List<string> LoaderDll = new List<string>();
        internal static List<string> LoaderAESKey = new List<string>();
        internal static List<string> LoaderProcess = new List<string>();
        internal static List<string> LoaderHosts = new List<string>();
        internal static List<string> LoaderShitStrings = new List<string>();
        internal static List<string> LoaderServicename = new List<string>();
        #endregion

        #region "Imports"
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        private static extern int OneWayAttribute([In] IntPtr obj0, [In] int obj1, [In] int obj2);
        #endregion

        #region "ReadOnly"
        internal static int tries, tries2, tries3;

        internal static string DeData, DeData3, DeData4, DeData5, DeData10, DeData13, DeData99, DeDataToken, DeDataStrings;
        internal static string tpml, tpmp;
        internal static string BanReason;
        #endregion

        #region "ComputerData"
        public static string GetIPAddress()
        {
            try
            {
                String address = "";
                WebRequest request = WebRequest.Create(IPAPIURL);
                request.Proxy = new WebProxy();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;
                using (WebResponse response = request.GetResponse())
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    address = stream.ReadToEnd();
                }
                return address;
            }
            catch
            {
                return "";
            }
        }

        public static string GetMemberHWID()
        {
            RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Control\IDConfigDB\Hardware Profiles\0001");
            return (String)rk.GetValue("HwProfileGuid");
        }

        public static string GetOSFriendlyName()
        {
            string result = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                result = os["Caption"].ToString();
                break;
            }
            return result;
        }
        public static string GetMemberMac()
        {
            string id = " ";
            ManagementObjectSearcher query = null;
            ManagementObjectCollection queryCollection = null;
            try
            {
                query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
                queryCollection = query.Get();
                foreach (ManagementObject mo in queryCollection)
                {
                    if (mo["MacAddress"] != null)
                    {
                        id = mo["MacAddress"].ToString();
                        //MessageBox.Show(id);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Source);
                MessageBox.Show(ex.Message);
            }
            return id;
        }
        public static string GetCPUID()
        {
            string cpuID = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (cpuID == "")
                {
                    //Remark gets only the first CPU ID
                    cpuID = mo.Properties["processorID"].Value.ToString();

                }
            }
            return cpuID;
        }
        public static string GetMotherboardSerialNumber()
        {
            string SerialNumber = string.Empty;
            ManagementObjectSearcher mc = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            ManagementObjectCollection moc = mc.Get();

            foreach (ManagementObject mo in moc)
            {
                if (SerialNumber == "")
                {
                    //Remark gets only the first Serial Number
                    SerialNumber = mo.Properties["SerialNumber"].Value.ToString();

                }
            }
            return SerialNumber;
        }
        #endregion

        #region "Security"
        private static bool CheckForAnyProxyConnections() //Lets disable all proxy ^^
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", writable: true);
            string a = registryKey.GetValue("ProxyEnable").ToString();
            registryKey.GetValue("ProxyServer");
            if (a == "1")
            {
                try
                {
                    return false;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool AntiWeb() //Lets disable all debuggers ^^ 
        {
            if (GetModuleHandle("HTTPDebuggerBrowser.dll") != IntPtr.Zero || GetModuleHandle("FiddlerCore4.dll") != IntPtr.Zero || GetModuleHandle("RestSharp.dll") != IntPtr.Zero || GetModuleHandle("Titanium.Web.Proxy.dll") != IntPtr.Zero || GetModuleHandle("libwireshark.dll") != IntPtr.Zero || GetModuleHandle("Telerik.NetworkConnections.dll") != IntPtr.Zero)
            {
                try
                {
                    return false;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool CheckForHTTPDebugger() //Disable HTTPDebugger PRO
        {
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                try
                {
                    foreach (ProcessModule module in process.Modules)
                    {
                        Console.WriteLine(module.FileName);
                        if (module.FileName.Contains("HTTPDebuggerBrowser.dll"))
                        {
                            return false;
                        }
                    }
                }
                catch
                {

                }
            }
            return true;
        }
        internal static bool CheckForBannedHWID() // Check Banned Users HWID/mname
        {
            Authenticator.DeData5 = Authenticator.DecryptThisShit5();
            if (DeData == "Connection Blocked")
            {
                MessageBox.Show("Too many failed request attempts", MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (DeData == "trymenow")
            {
                MessageBox.Show("Something Went Wrong! (5)", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            //if (Authenticator.DeData5 == "Something Went Wrong! (5)")
            //{
            //   MessageBox.Show("Something Went Wrong! (5)", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //   return false;
            //}
             else if (Authenticator.DeData5 == "beo beo")//found banned hwid/mname
            {
                WebClient LoginWebClient = new WebClient();
                LoginWebClient.Headers.Add(HttpRequestHeader.UserAgent, Authenticator.UserAgent);
                LoginWebClient.Proxy = new WebProxy();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = Authenticator.ValidateRemoteCertificate;
                return false;
            }
            else if (Authenticator.DeData5 == "Hoho") //didnt find banned hwid/mname
            {
                WebClient LoginWebClient = new WebClient();
                LoginWebClient.Headers.Add(HttpRequestHeader.UserAgent, Authenticator.UserAgent);
                LoginWebClient.Proxy = new WebProxy();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = Authenticator.ValidateRemoteCertificate;
                return true;
            }

            return true;
        }

        internal static bool AddBannedHWID() //Add Banned HWID/MName
        {
            DeData4 = DecryptThisShit4();
            if (DeData4 == "Something Went Wrong! (4)")
            {
                MessageBox.Show("Something Went Wrong! (4)", MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            else
            {
                WebClient LoginWebClient = new WebClient();
                LoginWebClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                LoginWebClient.Proxy = new WebProxy();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;
                return false;
            }
            return true;
        }

        internal static bool IsSandboxie() //Sandbox? Nope ^^
        {
            if (GetModuleHandle("SbieDll.dll") != IntPtr.Zero)
                return true;
            return false;
        }
        internal static bool IsEmulation() //No more emulations ;)
        {
            var millisecondsTimeout = new Random().Next(3000, 10000);
            var now = DateTime.Now;
            Thread.Sleep(millisecondsTimeout);
            if ((DateTime.Now - now).TotalMilliseconds >= millisecondsTimeout)
                return false;
            return true;
        }
        internal static void WellKnownSidType()
        {
            var handle = Process.GetCurrentProcess().Handle;
            while (true)
            {
                do
                {
                    Thread.Sleep(100);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                } while (Environment.OSVersion.Platform != PlatformID.Win32NT);

                OneWayAttribute(handle, -1, -1);
            }
        }

        internal static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        #endregion

        #region "Authorization"
        public static string DecryptThisShit(string username, string password)
        {
            try
            {
                if (tries <= 3)
                {
                    tries += 1;
                    WebClient TimeWebClient = new WebClient();
                    TimeWebClient.Proxy = new WebProxy();
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                    String TimeOnServer = TimeWebClient.DownloadString(TimeAPIURL);

                    WebClient LoginWebClient = new WebClient();
                    LoginWebClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    LoginWebClient.Proxy = new WebProxy();
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                    String ServerResponse = LoginWebClient.DownloadString(AuthURL + "?login=" + username + "&password=" + password + "&hwid=" + GetMemberHWID() + "&machinename=" + Environment.MachineName + "&osversion=" + GetOSFriendlyName() + "&cid=" + GetCPUID() + "&mac=" + GetMemberMac() + "&mbsn=" + GetMotherboardSerialNumber());

                    return Decrypt(ServerResponse, password, TimeOnServer);
                }
                else
                {
                    return "Connection Blocked";
                }
            }
            catch
            {
                if (tries <= 3)
                {
                    Thread.Sleep(1);

                    return "trymenow";
                }
                else
                {
                    return "Connection Blocked";
                }
            }
        }

        public static string TokenCheckFunc(string username, string prodname, string groupid, string password)
        {
            try
            {
                WebClient TimeWebClient = new WebClient();
                TimeWebClient.Proxy = new WebProxy();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                String TimeOnServer = TimeWebClient.DownloadString(TimeAPIURL);

                WebClient LoginWebClient = new WebClient();
                LoginWebClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                LoginWebClient.Proxy = new WebProxy();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                string hashed = Encrypt(username + prodname + groupid, password, TimeOnServer);

                String ServerResponse = LoginWebClient.DownloadString(LoaderToken + "?login=" + username + "&token=" + hashed + "&password=" + password);

                return Decrypt(ServerResponse, password, TimeOnServer);
            }
            catch
            {
                return "Something Went Wrong! (1000)";
            }
        }

        public static string DecryptThisShit3(string username, string groupid, string password)
        {
            try
            {
                if (tries3 <= 3)
                {
                    tries3 += 1;
                    WebClient TimeWebClient = new WebClient();
                    TimeWebClient.Proxy = new WebProxy();
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                    String TimeOnServer = TimeWebClient.DownloadString(TimeAPIURL);

                    WebClient LoginWebClient = new WebClient();
                    LoginWebClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    LoginWebClient.Proxy = new WebProxy();
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                    String ServerResponse = LoginWebClient.DownloadString(InfoURL + "?login=" + username + "&groupid=" + groupid + "&password=" + password);

                    return Decrypt(ServerResponse, password, TimeOnServer);
                }
                else
                {
                    return "Connection Blocked";
                }
            }
            catch
            {
                if (tries3 <= 3)
                {
                    Thread.Sleep(1);

                    return "trymenow";
                }
                else
                {
                    return "Connection Blocked";
                }
                    //return "Something Went Wrong! (3)";
            }
        }

        public static string DecryptThisShit4()
        {
            try
            {
                WebClient TimeWebClient = new WebClient();
                TimeWebClient.Proxy = new WebProxy();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                String TimeOnServer = TimeWebClient.DownloadString(TimeAPIURL);

                WebClient LoginWebClient = new WebClient();
                LoginWebClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                LoginWebClient.Proxy = new WebProxy();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                String ServerResponse = LoginWebClient.DownloadString(BanChecker + "?&hwid=" + GetMemberHWID() + "&machinename=" + Environment.MachineName + "&reason=" + BanReason + "&cpuid=" + GetCPUID() + "&mac=" + GetMemberMac() + "&ip=" + GetIPAddress() + "&mbsn=" + GetMotherboardSerialNumber());

                return Decrypt(ServerResponse, "4sad45as54d54as", TimeOnServer);
            }
            catch
            {
                return "Something Went Wrong! (4)";
            }
        }
        public static string DecryptThisShit5()
        {
            try
            {
                if (tries2 <= 3)
                {
                    tries2 += 1;
                    WebClient TimeWebClient = new WebClient();
                    TimeWebClient.Proxy = new WebProxy();
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                    String TimeOnServer = TimeWebClient.DownloadString(TimeAPIURL);

                    WebClient LoginWebClient = new WebClient();
                    LoginWebClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    LoginWebClient.Proxy = new WebProxy();
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                    String ServerResponse = LoginWebClient.DownloadString(BanChecker2 + "?&hwid=" + GetMemberHWID() + "&machinename=" + Environment.MachineName + "&cpuid=" + GetCPUID() + "&mac=" + GetMemberMac() + "&ip=" + GetIPAddress() + "&mbsn=" + GetMotherboardSerialNumber());

                    return Decrypt(ServerResponse, "4sad45as54d54as", TimeOnServer);
                }
                else
                {
                    return "Connection Blocked";
                }
            }
            catch
            {
                if (tries2 <= 3)
                {
                    Thread.Sleep(1);

                    return "trymenow";
                }
                else
                {
                    return "Connection Blocked";
                }
                //return "Something Went Wrong! (5)";
            }
        }

        public static string Detect_Strings()
        {
            try
            {
                if (tries2 <= 3)
                {
                    tries2 += 1;
                    WebClient TimeWebClient = new WebClient();
                    TimeWebClient.Proxy = new WebProxy();
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                    String TimeOnServer = TimeWebClient.DownloadString(TimeAPIURL);

                    WebClient LoginWebClient = new WebClient();
                    LoginWebClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                    LoginWebClient.Proxy = new WebProxy();
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                    String ServerResponse = LoginWebClient.DownloadString(LoaderStrings);

                    return Decrypt(ServerResponse, "ds54bd5asf4d4a5sd", TimeOnServer);
                }
                else
                {
                    return "Connection Blocked";
                }
            }
            catch
            {
                if (tries2 <= 3)
                {
                    Thread.Sleep(1);

                    return "trymenow";
                }
                else
                {
                    return "Connection Blocked";
                }
                //return "Something Went Wrong! (2000)";
            }
        }

        public static string DecryptThisShit13(string username, string password) //Get Expiration Date
        {
            try
            {
                WebClient TimeWebClient = new WebClient();
                TimeWebClient.Proxy = new WebProxy();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                String TimeOnServer = TimeWebClient.DownloadString(TimeAPIURL);

                WebClient LoginWebClient = new WebClient();
                LoginWebClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                LoginWebClient.Proxy = new WebProxy();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                String ServerResponse = LoginWebClient.DownloadString(LoaderInfoURL + "?login=" + username + "&password=" + password);

                return Decrypt(ServerResponse, password, TimeOnServer);
            }
            catch
            {
                return "Something Went Wrong! (13)";
            }
        }

        public static string ActivateKeyFunc(string licensekey, string username) // directly
        {
            try
            {
                WebClient LoginWebClient = new WebClient();
                //LoginWebClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                LoginWebClient.Proxy = new WebProxy();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                String ServerResponse = LoginWebClient.DownloadString(ActivateKeyURL + licensekey + "&identifier=" + username);

                return ServerResponse;
            }
            catch (WebException ee)
            {
                using (StreamReader r = new StreamReader(ee.Response.GetResponseStream()))
                {
                    string response = r.ReadToEnd(); // access the reponse message

                    return response;
                }
            }
            catch (Exception ex)
            {
                return "Something Went Wrong! (99)";
            }
        }

        internal static bool Login(string username, string password, bool autoLogin = false)
        {
            if (AntiWeb() && CheckForHTTPDebugger())
            {
                if (CheckForAnyProxyConnections())
                {
                    try
                    {
                        WebClient LogsWebClient = new WebClient();
                        LogsWebClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                        LogsWebClient.Proxy = new WebProxy();

                        System.Net.ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(ValidateRemoteCertificate);

                        string OPSV = "UNK";

                        if (Environment.Is64BitOperatingSystem)
                        {
                            OPSV = "64";
                        }
                        else
                        {
                            OPSV = "86";
                        }

                        //string SendLogs = LogsWebClient.DownloadString(LogsURL + "?login=" + username + "&mn=" + Environment.MachineName + "&udn=" + Environment.UserDomainName + "&pcu=" + Environment.UserName + "&osfn=" + GetOSFriendlyName() + "&ost=" + OPSV + "&hwid=" + GetMemberHWID());

                        try
                        {
                            tpml = username;
                            tpmp = password;

                            DeData = DecryptThisShit(username, password);
                            if (DeData == "Connection Blocked")
                            {
                                MessageBox.Show("Too many failed login attempts", MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }
                            else if (DeData == "trymenow")
                            {
                                return false;
                            }
                            else
                            {
                                DeData13 = DecryptThisShit13(username, password);
                                if (DeData13 == "Something Went Wrong! (13)")
                                {
                                    MessageBox.Show("Something Went Wrong! (13)", MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }
                                else
                                {
                                    char[] spearator2 = { '|' };
                                    string[] LoginRespond2 = DeData13.Split(spearator2);
                                    LoaderNews = LoginRespond2[0];
                                    LoaderVersion = LoginRespond2[1];
                                    LoaderExeUpdate = LoginRespond2[2];
                                    LoaderEXDate = LoginRespond2[3];
                                    LoaderProdName = LoginRespond2[4];
                                    LoaderHWIDLT = LoginRespond2[5];
                                    LoaderHWIDMN = LoginRespond2[6];

                                    WebClient LoginWebClient2 = new WebClient();
                                    LoginWebClient2.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                                    LoginWebClient2.Proxy = new WebProxy();
                                    System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;
                                }

                                char[] spearator = { '|' };
                                string[] LoginRespond = DeData.Split(spearator);
                                DisplayName = LoginRespond[0];
                                GroupId = LoginRespond[1];
                                AvatarURL = LoginRespond[2];
                                MemberHWID = LoginRespond[3];
                                //MemberMachine = LoginRespond[4];
                                //MemberOSVersion = LoginRespond[5];
                                //LoginRespond[6] => CPUID .. LoginRespond[7] => MacAddress

                                WebClient LoginWebClient = new WebClient();
                                LoginWebClient.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
                                LoginWebClient.Proxy = new WebProxy();
                                System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateRemoteCertificate;

                                if (LoginRespond[0] + LoginRespond[1] + LoginRespond[2] + LoginRespond[3] == "ErrorFoundInvalidAgent")
                                {
                                    MessageBox.Show("Wrong User Agent", MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }

                                else if (LoginRespond[0] + LoginRespond[1] + LoginRespond[2] + LoginRespond[3] == "ErrorFoundMaintenanceEnabled")
                                {
                                    MessageBox.Show("Auth Maintenance", MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }

                                else if (LoginRespond[0] + LoginRespond[1] + LoginRespond[2] + LoginRespond[3] == "ErrorFoundInvalidPassword")
                                {
                                    MessageBox.Show("Wrong Login or Password user " + username, MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }

                                else if (LoginRespond[0] + LoginRespond[1] + LoginRespond[2] + LoginRespond[3] == "ErrorFoundInvalidHWID")
                                {
                                    MessageBox.Show("Wrong PC for user: " + username, MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }

                                else if (LoginRespond[0] + LoginRespond[1] + LoginRespond[2] + LoginRespond[3] == "ErrorFoundNoSubscription")
                                {
                                    MessageBox.Show("You need to buy subscription " + username, MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }

                                else if (LoginRespond[0] + LoginRespond[1] + LoginRespond[2] + LoginRespond[3] == "ErrorFoundHWIDSuspended")
                                {
                                    MessageBox.Show("Your HWID has need Suspended", MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }

                                else if (LoginRespond[0] + LoginRespond[1] + LoginRespond[2] + LoginRespond[3] == "ErrorCantAccessDatabase")
                                {
                                    MessageBox.Show("Can't connect to Database", MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }

                                else if (LoginRespond[0] + LoginRespond[1] + LoginRespond[2] + LoginRespond[3] == "ErrorFoundAccountSuspended")
                                {
                                    MessageBox.Show("Your account has been suspended \n Please check forum for more details", MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }

                                else if (!LoginRespond[0].Contains(username))
                                {
                                    MessageBox.Show("Unknown error occurred while attempting to get user data", MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }
                                else
                                {
                                    try
                                    {
                                        Avatar = GetImageFromPicPath("https://hiddencheats.net/uploads/" + AvatarURL); // Change Link after changing the subdomain name.
                                    }
                                    catch
                                    {

                                    }
                                    return true;
                                }
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Error occurred while contacting auth servers \n Please check your network connection", MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                    }
                    catch
                    {
                        MessageBox.Show("Error occurred while contacting auth servers \n Please check your network connection", MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("System Proxy change detected\n If You want use our tools please disable it :)", MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Running HTTP Debugger detected!\n If You want use our tools please close it :)", MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion
    }
}