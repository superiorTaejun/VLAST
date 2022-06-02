using System.Text;

using System.Runtime.InteropServices;

namespace settingChanger
{
    class SettingChanger
    {
        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        public static bool changeSetting(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }
            Console.WriteLine(Path.GetFileName(path));
            if (Path.GetFileName(path) == "BaseEditorPerProjectUserSettings.ini" || Path.GetFileName(path) == "EditorPerProjectUserSettings.ini")
            {
                WritePrivateProfileString("/Script/UnrealEd.EditorLoadingSavingSettings", "bSCCAutoAddNewFiles", "False", path);
            } else
            {
                return false;
            }

            return true;
        }
    }
}
