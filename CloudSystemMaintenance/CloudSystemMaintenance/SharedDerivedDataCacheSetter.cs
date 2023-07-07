using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CloudSystemMaintenance
{
	class SharedDerivedDataCacheSetter
	{
		[DllImport("kernel32")]
		static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

		[DllImport("kernel32")]
		static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

		public static void UpdateEditorSettings()
		{
			string unrealEngineFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UnrealEngine");

			// 모든 폴더 경로 저장
			string[] folders = Directory.GetDirectories(unrealEngineFolderPath);

			foreach (string folder in folders)
			{
				string iniFilePath = Path.Combine(folder, "Saved", "Config", "WindowsEditor", "EditorSettings.ini");

				if (File.Exists(iniFilePath))
				{
					// INI 파일에서 [/Script/UnrealEd.EditorSettings] 섹션 검색
					string section = "/Script/UnrealEd.EditorSettings";
					bool sectionFound = IniSectionExists(iniFilePath, section);

					if (sectionFound)
					{
						// SharedDerivedDataCache 키가 존재하는지 확인
						string key = "SharedDerivedDataCache";
						bool keyExists = IniKeyExists(iniFilePath, section, key);

						// INI 파일 수정
						ModifyIniFile(iniFilePath, section, key, keyExists);
					}
				}
			}
		}

		static bool IniSectionExists(string filePath, string section)
		{
			const int bufferSize = 2048;
			StringBuilder buffer = new StringBuilder(bufferSize);
			GetPrivateProfileString(section, null, "", buffer, bufferSize, filePath);
			return !string.IsNullOrEmpty(buffer.ToString());
		}

		static bool IniKeyExists(string filePath, string section, string key)
		{
			const int bufferSize = 2048;
			StringBuilder buffer = new StringBuilder(bufferSize);
			GetPrivateProfileString(section, key, "", buffer, bufferSize, filePath);
			return !string.IsNullOrEmpty(buffer.ToString());
		}

		static void ModifyIniFile(string filePath, string section, string key, bool keyExists)
		{
			// INI 파일 수정
			if (keyExists)
			{
				// 새로운 SharedDerivedDataCache=(Path="%OfficeUnrealDDC%") 추가
				// 사용자 환경변수 %OfficeUnrealDDC%가 존재하지 않는다면 SharedDerivedDataCache=(Path="") 입력
				string path = Environment.GetEnvironmentVariable("OfficeUnrealDDC");
				if (path != null)
				{
					path = path.Replace("\\", "/");
					WritePrivateProfileString(section, "SharedDerivedDataCache", "(Path=\"" + path + "\")", filePath);
					Console.WriteLine("SharedDerivedDataCache 값이 변경되었습니다: " + filePath);
				}
				else
				{
					// 섹션 안에 SharedDerivedDataCache=(Path="") 추가
					WritePrivateProfileString(section, key, "(Path=\"\")", filePath);
					Console.WriteLine("SharedDerivedDataCache 값이 존재하지 않습니다: " + filePath);
				}
			}
			else
			{
				// 섹션 안에 SharedDerivedDataCache=(Path="") 추가
				WritePrivateProfileString(section, key, "(Path=\"\")", filePath);
				Console.WriteLine("SharedDerivedDataCache 값이 존재하지 않습니다: " + filePath);
			}
		}
	}
}
