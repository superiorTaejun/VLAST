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
        /*public static void changeBaseEditor(string directory) // searching directory ver
        {
            string sourceDirectory = Directory.GetCurrentDirectory();
            string[] files = Directory.GetDirectories(@"C:\");
            foreach (string file in files)
            {
                Console.WriteLine(file);
            }
        }*/

        public static bool changeBaseEditor(string directory)
        {
            string sourceDirectory = directory;
            StreamReader? sr;
            if ((sr = DirectoryChecker.checkDirectoryOrNull(directory)) == null)
            {
                return false;
            }
            string fileContents = sr.ReadToEnd();
            int idx = fileContents.IndexOf("bSCCAutoAddNewFiles=True");
            if (idx == -1)
            {
                return true;
            }
            idx += 20;
            char[] chars = fileContents.ToCharArray();
            chars[idx++] = 'F';
            chars[idx++] = 'a';
            chars[idx++] = 'l';
            chars[idx++] = 's';
            fileContents = new string(chars).Insert(idx, "e");
            File.WriteAllText(@"C:\Users\Bmo\Documents\github\VLAST\settingChanger\settingChanger\test1\new.txt", fileContents);
            return true;
        }

        public static bool changeEditor()
        {
            string directory = Directory.GetCurrentDirectory() + @"\test.text"; // For test
            // string directory = Directory.GetCurrentDirectory() + @"\EditorPerProjectUserSettings.ini"

            return true;
        }
    }
}
