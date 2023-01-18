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


namespace HiddenCheats_Loader2
{
    public partial class ActivateKey : Form
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
        public ActivateKey()
        {
            InitializeComponent();
        }

        private void ActivateKey_Load(object sender, EventArgs e)
        {
            this.Text = RandomString(25);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Authenticator.DeData99 = Authenticator.ActivateKeyFunc(textBox1.Text, Authenticator.DisplayName);
                if (Authenticator.DeData99 == "Something Went Wrong! (99)")
                {
                    MessageBox.Show("Something Went Wrong! (99)", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
                else
                {
                    try
                    {
                        string LoginRespond = Authenticator.DeData99;
                        if (LoginRespond.Contains("MAX_USES"))
                            MessageBox.Show("Key is used before!", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else if (LoginRespond.Contains("OKAY"))
                            MessageBox.Show("Congratulations! Key is activated.", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else if (LoginRespond.Contains("BAD_KEY_OR_ID"))
                            MessageBox.Show("Wrong key or ID!", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            MessageBox.Show("Unknown Error!", Authenticator.MessagesTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    catch (Exception es)
                    {
                        //MessageBox.Show(es.Message);
                        Environment.Exit(0);
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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var NextForm = new Main();
            NextForm.Show();

            this.Close();
        }
    }
}
