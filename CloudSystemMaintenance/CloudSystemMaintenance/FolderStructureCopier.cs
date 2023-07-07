using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSystemMaintenance
{
	class FolderStructureCopier
	{
		static HashSet<string> folders = new HashSet<string>();
		public static void ExcuteAfterCheckTerm()
		{
			string[] pluginsList = { "PlaveArtworks23", "PlaveLive", "PlaveCore", "VlastGeneric", "VlastCinematics", "VlastMaterialLibrary",
				"VlastCommonArtAssets", "VlastLive" };  // 1. 복제할 플러그인 목록
			string currentPath = AppDomain.CurrentDomain.BaseDirectory;  // 2. 원본 폴더 경로
			string srcPath = Path.GetDirectoryName(currentPath);
			srcPath = Path.GetDirectoryName(srcPath);
			Console.WriteLine("복사를 시작할 경로: " + srcPath);
			// string dstPath = Environment.GetEnvironmentVariable("UE Data Source Cloud"); // %UE Data Source Cloud%의 값인 경로를 저장
			string dstPath = Environment.GetEnvironmentVariable("Test Source Cloud"); // !!!테스트용!!! %Test Source Cloud%의 값인 경로를 저장

			// 환경변수에 등록된 경로 확인
			if (dstPath == null)
			{
				Console.WriteLine("UE Data Source Cloud가 존재하지 않거나, 구글 드라이브가 정상적으로 설치되지 않았습니다. 아무 키나 입력해 프로그램을 종료합니다.");
				Console.ReadLine();
				Environment.Exit(0);
			}

			// 마지막 업데이트로부터 4시간이 지났는 지 확인
			DateTime timeNow = DateTime.Now;
			string timeNowStr = timeNow.ToString();
			string timeLastUpdatedStr = File.ReadAllText(dstPath + "\\LastUpdatedTime.txt");
			// DateTime timeLastUpdated = DateTime.ParseExact(timeLastUpdatedStr, "yyyy-MM-dd tt HH:mm:ss", CultureInfo.GetCultureInfo("ko-KR"));
			DateTime timeLastUpdated = DateTime.Parse(timeLastUpdatedStr);
			Console.WriteLine("마지막으로 복제가 이루어진 시각: " + timeLastUpdated.ToString());
			TimeSpan timeDifference = timeNow - timeLastUpdated;

			/*if (timeDifference.TotalMinutes < 240)
			{
				Console.WriteLine("마지막 경로 복제가 이루어진 이후 4시간이 지나지 않아 복제는 진행하지 않습니다.");
				Console.WriteLine("다음 경로 복제까지 남은 시간: " + (int)(240 - timeDifference.TotalMinutes) + "분");
				goto deleteDormentFolder;
				*//*Console.WriteLine("아무 키나 입력해 프로그램을 종료합니다.");
				Console.ReadLine();
				Environment.Exit(0);*//*
			}*/

			Console.WriteLine(timeLastUpdatedStr);

			// 폴더 구조 복제
			FolderStructureCopier.CopyFolderStructure(srcPath, dstPath, pluginsList);
			File.WriteAllText(dstPath + "\\LastUpdatedTime.txt", timeNowStr);
			Console.WriteLine("폴더 구조 복제가 완료되었습니다.");

		deleteDormentFolder:;
			// UE Data Source Cloud 내 필요 없는 폴더 삭제
			DeleteDormentFolders();
		}

		private static void CopyFolderStructure(string srcPath, string dstPath, string[] pluginsList)
		{
			DirectoryInfo srcDir = new DirectoryInfo(srcPath);

			// 현재 폴더의 하위 폴더들을 탐색
			foreach (DirectoryInfo subDir in srcDir.GetDirectories())
			{
				if (subDir.Attributes.HasFlag(FileAttributes.Hidden))
				{
					continue;  // 숨김 폴더는 무시
				}

				string folderName = subDir.Name;
				string newFolderPath = dstPath;

				// Plugins 폴더를 탐색
				string pluginsFolderPath = Path.Combine(subDir.FullName, "Plugins");
				if (Directory.Exists(pluginsFolderPath))
				{
					// 폴더 내의 동일한 이름의 폴더 확인
					foreach (string pluginName in pluginsList)
					{
						string pluginFolderPath = Path.Combine(pluginsFolderPath, pluginName);
						if (Directory.Exists(pluginFolderPath))
						{
							// Content 폴더 하위 폴더 구조 복사
							string contentFolderPath = Path.Combine(pluginFolderPath, "Content");
							if (Directory.Exists(contentFolderPath))
							{
								CopyChildFolderStructure(contentFolderPath, newFolderPath);
							}
						}
					}

					// Plugins 폴더 안에 폴더가 있을 경우 재귀 호출
					CopyFolderStructure(pluginsFolderPath, newFolderPath, pluginsList);
				}

				// 재귀 호출
				CopyFolderStructure(subDir.FullName, newFolderPath, pluginsList);
			}
		}

		private static void CopyChildFolderStructure(string srcPath, string dstPath)
		{
			DirectoryInfo srcDir = new DirectoryInfo(srcPath);

			// 현재 폴더의 하위 폴더들을 탐색
			foreach (DirectoryInfo subDir in srcDir.GetDirectories())
			{
				string folderName = subDir.Name;
				string newFolderPath = Path.Combine(dstPath, folderName);

				// 새로운 폴더 생성
				Directory.CreateDirectory(newFolderPath);
				Console.WriteLine("복제된 폴더: " + newFolderPath);

				// 해시셋에 추가
				folders.Add(subDir.FullName);

				// 하위 폴더들에 대해 재귀 호출
				CopyChildFolderStructure(subDir.FullName, newFolderPath);
			}
		}

		private static void DeleteDormentFolders()
		{
			RecursiveDeleteDormentFolders(Environment.GetEnvironmentVariable("Test Source Cloud"));
		}

		private static void RecursiveDeleteDormentFolders(string currentPath)
		{
			if (!Directory.Exists(currentPath))
			{
				return;
			}

			string[] subdirectories = Directory.GetDirectories(currentPath);
			foreach (string subdirectory in subdirectories)
			{
				RecursiveDeleteDormentFolders(subdirectory);
			}
			string folderName = Path.GetFileName(currentPath);
			if (folders.Contains(folderName))
			{
				// folders에 담겨있을 경우 다음 폴더로 넘어감
				return;
			}

			string[] files = Directory.GetFiles(currentPath);
			string[] childDirectories = Directory.GetDirectories(currentPath);
			if (files.Length == 0 && childDirectories.Length == 0)
			{
				Console.WriteLine($"삭제된 폴더: {currentPath}");
				Directory.Delete(currentPath);
			}
		}
	}
}
