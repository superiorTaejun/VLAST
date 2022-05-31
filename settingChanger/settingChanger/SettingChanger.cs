using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace settingChanger
{
    class SettingChanger
    {
        public static string settingFalse(StreamReader sr)
        {
            string fileContents = sr.ReadToEnd();

            int idx = fileContents.IndexOf("bSCCAutoAddNewFiles=True");
            if (idx == -1)
            {
                return fileContents;
            }
            idx += 20;
            char[] chars = fileContents.ToCharArray();
            chars[idx++] = 'F';
            chars[idx++] = 'a';
            chars[idx++] = 'l';
            chars[idx++] = 's';
            fileContents = new string(chars).Insert(idx, "e");

            return fileContents;
        }

        public static bool changeBaseEditor(string directory)
        {
            StreamReader? sr;
            if ((sr = DirectoryChecker.checkDirectoryOrNull(directory)) == null)
            {
                return false;
            }
            string fileContents = settingFalse(sr);
            File.WriteAllText(directory, fileContents);

            return true;
        }

        public static bool changeEditor(string directory)
        {
            StreamReader? sr;
            if ((sr = DirectoryChecker.checkDirectoryOrNull(directory)) == null)
            {
                return false;
            }
            string fileContents = sr.ReadToEnd();
            if (fileContents.Contains("bSCCAutoAddNewFiles=") == false) {
                int idx = fileContents.IndexOf("UnrealEd.EditorLoadingSavingSettings]") + 37;
                fileContents = fileContents.Insert(idx, "\r\nbSCCAutoAddNewFiles=False");
                File.WriteAllText(directory, fileContents);

                return true;
            }
            fileContents = settingFalse(sr);
            File.WriteAllText(directory, fileContents);

            return true;
        }
    }
}
