using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices ;
using System.Security.Cryptography;
using System.IO;

namespace HiddenCheats_Loader2
{   
    public class AES
    {
        public static string Decrypt(string input, string pass)
        {
            string str="";
            RijndaelManaged managed = new RijndaelManaged();
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            try
            {
                byte[] buffer = new byte[0x20];
                byte[] buffer2 = provider.ComputeHash(Encoding.ASCII.GetBytes(pass));
                Array.Copy(buffer2, 0, buffer, 0, 0x10);
                Array.Copy(buffer2, 0, buffer, 15, 0x10);
                managed.Key = buffer;
                managed.Mode = (CipherMode)CipherMode.ECB;
                ICryptoTransform transform = managed.CreateDecryptor();
                byte[] inputBuffer = Convert.FromBase64String(input);
                str = Encoding.ASCII.GetString(transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length));
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                ProjectData.ClearProjectError();
            }
            return str;
        }

        public static string Encrypt(string input, string pass)
        {
            string str = "";
            RijndaelManaged managed = new RijndaelManaged();
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            try
            {
                byte[] buffer = new byte[0x20];
                byte[] buffer2 = provider.ComputeHash(Encoding.ASCII.GetBytes(pass));
                Array.Copy(buffer2, 0, buffer, 0, 0x10);
                Array.Copy(buffer2, 0, buffer, 15, 0x10);
                managed.Key = buffer;
                managed.Mode = (CipherMode)CipherMode.ECB;
                ICryptoTransform transform = managed.CreateEncryptor();
                byte[] bytes = Encoding.ASCII.GetBytes(input);
                str = Convert.ToBase64String(transform.TransformFinalBlock(bytes, 0, bytes.Length));
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                ProjectData.ClearProjectError();
            }
            return str;
        }
    }
}
