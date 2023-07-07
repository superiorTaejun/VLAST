using System;
using System.Globalization;
using System.IO;

namespace CloudSystemMaintenance
{
	class Program
	{
		static void Main()
		{
			// 현재 파일의 경로에서 한 단계 부모 경로로 이동
			string currentFolder = Directory.GetCurrentDirectory();
			string parentFolder = Path.GetDirectoryName(currentFolder);

			// SharedDerivedDataCacheSetter 실행
			SharedDerivedDataCacheSetter.UpdateEditorSettings();

			// 하위 경로를 순회하며 INI 파일 검색 및 수정
			DataSourceFolderSetter.ProcessSubdirectories(parentFolder);

			// DataSourceFolderSetter 실행
			DataSourceFolderSetter.UpdateSubfolders(parentFolder);

			// FolderStructureCopier 실행
			FolderStructureCopier.ExcuteAfterCheckTerm();

			Console.WriteLine("작업이 완료되었습니다.");
			Console.ReadLine();
		}
	}
}