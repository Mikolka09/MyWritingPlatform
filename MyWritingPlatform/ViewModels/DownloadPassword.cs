using System;
using System.IO;
using System.Text;

namespace MyWritingPlatform.ViewModels
{
    public class DownloadPassword
    {
        string pathAdmin = @"d:\Users\MIKOLKA\MyWritingPlatform\MyWritingPlatform\passAdmin.txt";
        string pathSmtp = @"d:\Users\MIKOLKA\MyWritingPlatform\MyWritingPlatform\passSmtp.txt";
        public string[] DeobfuscateAdmin()
        {
            var bytes = Convert.FromBase64String(File.ReadAllText(pathAdmin));
            for (int i = 0; i < bytes.Length; i++) bytes[i] ^= 0x5a;
            return Encoding.UTF8.GetString(bytes).Split("|");
        }
        public string[] DeobfuscateSmtp()
        {
            var bytes = Convert.FromBase64String(File.ReadAllText(pathSmtp));
            for (int i = 0; i < bytes.Length; i++) bytes[i] ^= 0x5a;
            return Encoding.UTF8.GetString(bytes).Split("|");
        }

    }

}
