
namespace settingChanger
{
    class SettingChanger
    {
        // 여기에 문제가 있다 싶음
        [System.Runtime.InteropServices.DllImport("kernel32")]
        static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        /*[DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);*/
        // 여기까지
        public static bool changeSetting(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }
            if (Path.GetFileName(path) == "BaseEditorPerProjectUserSettings.ini" || Path.GetFileName(path) == "EditorPerProjectUserSettings.ini")
            {
                WritePrivateProfileString("/Script/UnrealEd.EditorLoadingSavingSettings", "bSCCAutoAddNewFiles", "False", path);
                /*IniFile ini = new IniFile();
                ini.*/ // 이 방식으로 고쳐야한다고 봄
            } else
            {
                return false;
            }

            return true;
        }
    }
}
