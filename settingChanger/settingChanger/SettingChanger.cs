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
        static void Main()
        {
            changeSetting();
            deleteFolder();
            
        }

        public static void changeSetting()
        {
            string sourceDirectory = @"C:\Users\Bmo\Documents\github\VLAST\settingChanger\settingChanger\Debug\net6.0\testTrue.txt";
            //string sourceDirectory = @"C:\Users\Bmo\Documents\github\VLAST\settingChanger\settingChanger\Debug\net6.0\testFalse.txt"; // For test

            //@"C:\Program Files\Epic Games\UE_5.0\Engine\Config\BaseEditorPerProjectUserSettings.txt" // For " UE_5.0 "
            System.IO.StreamReader sr = new StreamReader(sourceDirectory);
            string text = sr.ReadToEnd();
            int idx = text.IndexOf("bSCCAutoAddNewFiles=True");
            if (idx == -1)
            {
                Console.WriteLine("Already \"False\"");
                return;
            }
            idx += 20;
            char[] chars = text.ToCharArray();
            chars[idx++] = 'F';
            chars[idx++] = 'a';
            chars[idx++] = 'l';
            chars[idx++] = 's';
            text = new string(chars).Insert(idx, "e");
            File.WriteAllText(@"C:\Users\Bmo\Documents\github\VLAST\settingChanger\settingChanger\Debug\net6.0\new.txt", text);
            Console.WriteLine("Changed successfully");
        }

        public static void deleteFolder()
        {
            string directory = Directory.GetCurrentDirectory() + @"\test"; // For test
            // string directory = Directory.GetCurrentDirectory() + @"Saved";
            Directory.Delete(directory, true);
        }
    }
}
