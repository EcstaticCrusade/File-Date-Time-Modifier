using System.IO;
using System.Linq;

namespace FileDateTimeModifier.Domain
{
    public static class Mp4FileProcessor
    {
        public static void ByFileName(string baseFolderPath)
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
    }
}