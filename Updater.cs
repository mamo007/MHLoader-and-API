using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HiddenCheats_Loader2.Auth;
using System.Net;
using System.IO;

namespace HiddenCheats_Loader2
{
    public partial class Updater : Form
    {
        #region Random Name 
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion
        static string ExePath = Directory.GetCurrentDirectory() + @"\MH - LOADER.zip";
        public Updater()
        {
            InitializeComponent();
        }

        public static void DownloadExe(string LoaderExeLink)
        {
            try
            {
                WebClient LoginWebClient = new WebClient();
                LoginWebClient.Proxy = new WebProxy();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = Authenticator.ValidateRemoteCertificate;
                LoginWebClient.DownloadFile(LoaderExeLink, ExePath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Updater_Load(object sender, EventArgs e)
        {
            this.Text = RandomString(25);

            if (Authenticator.LoaderExeUpdate != "")
            {
                DownloadExe(Authenticator.LoaderExeUpdate);
                if (File.Exists(ExePath))
                {
                    System.Windows.Forms.MessageBox.Show("Download Completed to the following location:\n" + ExePath, Authenticator.MessagesTitle, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    Environment.Exit(0);
                }
            }
        }

        private void Updater_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
