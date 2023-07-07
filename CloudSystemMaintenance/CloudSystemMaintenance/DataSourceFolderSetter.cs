using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CloudSystemMaintenance
{
	class DataSourceFolderSetter
	{
		[DllImport("kernel32")]
		static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

		[DllImport("kernel32")]
		static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

		public static void ProcessSubdirectories(string folder)
		{
			// 현재 폴더의 INI 파일 검색
			string iniFilePath = Path.Combine(folder, "Saved", "Config", "WindowsEditor", "EditorPerProjectUserSettings.ini");
			if (File.Exists(iniFilePath))
			{
				// INI 파일에서 [/Script/UnrealEd.EditorPerProjectUserSettings] 섹션 검색
				string section = "/Script/UnrealEd.EditorPerProjectUserSettings";
				bool sectionFound = IniSectionExists(iniFilePath, section);

				if (sectionFound)
				{
					// DataSourceFolder 키가 존재하는지 확인
					string key = "DataSourceFolder";
					bool keyExists = IniKeyExists(iniFilePath, section, key);

					// INI 파일 수정
					ModifyIniFile(iniFilePath, section, key, keyExists);
				}
			}

			// 하위 폴더 순회
			string[] subdirectories = Directory.GetDirectories(folder);
			foreach (string subdirectory in subdirectories)
			{
				ProcessSubdirectories(subdirectory);
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
				// 새로운 DataSourceFolder=(Path="%UE Data Source Cloud%") 추가
				string path = Environment.GetEnvironmentVariable("UE Data Source Cloud");
				if (string.IsNullOrEmpty(path)) // 사용자 환경변수 %UE Data Source Cloud%가 존재하지 않는다면
				{
					string examplePathEN = "G:\\Shared drives\\UE Data Source Cloud";
					string examplePathKR = "G:\\공유 드라이브\\UE Data Source Cloud";

					if (Directory.Exists(examplePathEN)) // 사용자 PC에 G:\\Shared drives\\UE Data Source Cloud 경로가 존재할 경우
					{
						examplePathEN = examplePathEN.Replace("\\", "/");
						WritePrivateProfileString(section, "DataSourceFolder", "(Path=\"" + examplePathEN + "\")", filePath);
						Console.WriteLine("DataSourceFolder 값이 변경되었습니다: " + filePath);
					}
					else if (Directory.Exists(examplePathKR)) // 사용자 PC에 G:\\공유 드라이브\\UE Data Source Cloud 경로가 존재할 경우
					{
						examplePathKR = examplePathKR.Replace("\\", "/");
						WritePrivateProfileString(section, "DataSourceFolder", "(Path=\"" + examplePathKR + "\")", filePath);
						Console.WriteLine("DataSourceFolder 값이 변경되었습니다: " + filePath);
					}
				}
				else
				{
					path = path.Replace("\\", "/");
					WritePrivateProfileString(section, "DataSourceFolder", "(Path=\"" + path + "\")", filePath);
					Console.WriteLine("DataSourceFolder 값이 변경되었습니다: " + filePath);
				}
			}
			else
			{
				// 섹션 안에 DataSourceFolder=(Path="") 추가
				WritePrivateProfileString(section, key, "(Path=\"\")", filePath);
				Console.WriteLine("DataSourceFolder 값이 존재하지 않습니다: " + filePath);
			}
		}

		public static bool IsRunAsAdministrator()
		{
			using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
			{
				var principal = new System.Security.Principal.WindowsPrincipal(identity);
				return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
			}
		}

		public static void UpdateSubfolders(string rootFolder)
		{
			string[] subFolders = Directory.GetDirectories(rootFolder);

			foreach (string subFolder in subFolders)
			{
				Console.WriteLine();
				Console.WriteLine("Updating " + Path.GetFileName(subFolder) + " folder...");
				Directory.SetCurrentDirectory(subFolder);
				RunCommand("cm", "/C chcp 65001 & cm partial update", subFolder);
				Console.WriteLine();
			}

			Console.WriteLine("모든 폴더가 업데이트되었습니다.");
			Console.WriteLine();
		}

		private static void RunCommand(string command, string arguments, string workingDirectory)
		{
			Process process = new Process();
			process.StartInfo.FileName = "cmd.exe";
			process.StartInfo.Arguments = arguments;
			process.StartInfo.WorkingDirectory = workingDirectory;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
			process.OutputDataReceived += Process_OutputDataReceived;
			process.Start();

			// 프로세스 출력 비동기 읽기 시작
			process.BeginOutputReadLine();


			process.WaitForExit();
			process.Close();
		}

		private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			// 프로세스 출력을 실시간으로 화면에 출력
			if (!string.IsNullOrEmpty(e.Data))
			{
				Console.WriteLine(e.Data);
			}
		}
	}
}