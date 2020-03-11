using System.IO;
using System.Linq;

namespace FileDateTimeModifier.Domain
{
    public static class Mp4FileProcessor
    {
        public static void Process(ProcessMethod method, string baseFolderPath)
        {
            switch (method)
            {
                case ProcessMethod.ByDateModified:
                    ByDateModified(baseFolderPath);
                    break;
                case ProcessMethod.ByFileName:
                    ByFileName(baseFolderPath);
                    break;
            }
        }

        private static void ByFileName(string baseFolderPath)
        {
            foreach (var fileName in Directory.EnumerateFiles(baseFolderPath).Where(f => f.ToLower().Contains(".mp4")))
            {
                var fi = new FileInfo(fileName);

                var dateTaken = DateTimeExtractor.FromFileName(fi.Name, ' ');

                var newFileName = dateTaken.ToString("yyyyMMdd_HHmmss");

                var newFilePath = $"{fi.DirectoryName}\\{newFileName}.mp4";

                File.SetCreationTime(fileName, dateTaken);
                File.SetLastWriteTime(fileName, dateTaken);
                File.SetLastAccessTime(fileName, dateTaken);
                File.Move(fileName, newFilePath);
            }
        }

        private static void ByDateModified(string baseFolderPath)
        {
            foreach (var fileName in Directory.EnumerateFiles(baseFolderPath).Where(f => f.ToLower().Contains(".mp4")))
            {
                var fi = new FileInfo(fileName);
                var correctDateTime = fi.LastWriteTime;
                var newFileName = correctDateTime.ToString("yyyyMMdd_HHmmss");
                var newFilePath = $"{fi.DirectoryName}\\{newFileName}.mp4";

                File.SetCreationTime(fileName, correctDateTime);
                File.SetLastWriteTime(fileName, correctDateTime);
                File.SetLastAccessTime(fileName, correctDateTime);
                File.Move(fileName, newFilePath);
            }
        }
    }
}