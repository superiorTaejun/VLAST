#define SETTER

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



namespace settingChanger
{

	class SettingChanger
	{
		static int MAX_LINECOUNTS = 1024;

		[System.Runtime.InteropServices.DllImport("kernel32")]
        static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [System.Runtime.InteropServices.DllImport("kernel32")]
        static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static string[] ReadIni(string path)
		{
			string[] sectionKeyValue = new string[MAX_LINECOUNTS];
			int idx = 0;

            StreamReader iniSr = new StreamReader(path);
            StringBuilder stringBuilder = new StringBuilder();

            char c;

            while (!iniSr.EndOfStream)
			{
				c = (char)iniSr.Read();
				if (c == '\r' || c == '\n')
				{
					if (stringBuilder.Length > 0) {
						sectionKeyValue[idx++] = stringBuilder.ToString();
						stringBuilder.Length = 0;
					}

					continue;
				}

				if (c == '=')
				{
					sectionKeyValue[idx++] = stringBuilder.ToString();
					stringBuilder.Length = 0;

					if (iniSr.Peek() == '(')
					{
						stringBuilder.Append(iniSr.ReadLine());

						sectionKeyValue[idx++] = stringBuilder.ToString();
						stringBuilder.Length = 0;
					}

					continue;
				}

				stringBuilder.Append(c);
			}

			if (stringBuilder.Length > 0)
			{
				sectionKeyValue[idx] = stringBuilder.ToString();
			}

			iniSr.Close();
            
            return sectionKeyValue;
        }

		public static bool SetConfig(string[] sectionKeyValue, string destFile)
		{
			Console.WriteLine("Set config: " + destFile);

			string section = "";

			int idx = 0;
			while (sectionKeyValue[idx] != null)
			{
				if (sectionKeyValue[idx].ToCharArray()[0] == '\r')
				{
					idx++;
					continue;
				}
				if (sectionKeyValue[idx].ToCharArray()[0] == '[')
				{
					section = sectionKeyValue[idx].Substring(1, sectionKeyValue[idx].IndexOf(']') - 1);
					Console.WriteLine("section: " + section);

					idx++;
					continue;
				}
				WritePrivateProfileString(section, sectionKeyValue[idx], sectionKeyValue[idx + 1], destFile);
				idx += 2;
			}

			return true;
        }
	}
}
