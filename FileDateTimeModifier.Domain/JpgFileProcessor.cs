using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Linq;

namespace FileDateTimeModifier.Domain
{
    public static class JpgFileProcessor
    {
        public static void Process(ProcessMethod method, string baseFolderPath)
        {
            switch (method)
            {
                case ProcessMethod.ByDateTaken:
                    ByDateTaken(baseFolderPath);
                    break;
                case ProcessMethod.ByDateModified:
                    ByDateModified(baseFolderPath);
                    break;
            }
        }

        private static void ByDateTaken(string baseFolderPath)
        {
            foreach (var fileName in Directory.GetFiles(baseFolderPath).Where(f => f.ToLower().Contains(".jpg")))
            {
                var fi = new FileInfo(fileName);
                var correctDateTime = GetDateFromDateTakenMetaData(fileName);

                if (!correctDateTime.HasValue)
                    continue;

                var newFileName = GetImageFileName(fi, correctDateTime.Value);

                File.SetCreationTime(fileName, correctDateTime.Value);
                File.SetLastWriteTime(fileName, correctDateTime.Value);
                File.SetLastAccessTime(fileName, correctDateTime.Value);

                File.Move(fileName, newFileName);
            }
        }

        private static void ByDateModified(string baseFolderPath)
        {
            foreach (var fileName in Directory.GetFiles(baseFolderPath).Where(f => f.ToLower().Contains(".jpg")))
            {
                var fi = new FileInfo(fileName);
                var correctDateTime = fi.LastWriteTime;
                var newFileName = GetImageFileName(fi, correctDateTime);

                File.SetCreationTime(fileName, correctDateTime);
                File.SetLastWriteTime(fileName, correctDateTime);
                File.SetLastAccessTime(fileName, correctDateTime);

                File.Move(fileName, newFileName);
            }
        }

        /// <summary>
        /// Gets the images date from the Date Taken metadata value
        /// Uses SixLabors ImageSharp NuGet Package
        /// </summary>
        /// <param name="imageFilePath">Full path of the image file</param>
        /// <returns></returns>
        private static DateTime? GetDateFromDateTakenMetaData(string imageFilePath)
        {
            using (var fs = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var myImage = Image.Load(fs))
                {
                    try
                    {
                        // Results in a value like YYYY/MM/DD HH:MM:SS or NULL
                        var dateTakenAsString = myImage.Metadata.ExifProfile.Values.ToList().FirstOrDefault(v => v.Tag == SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifTag.DateTimeOriginal)?.Value.ToString();

                        if (string.IsNullOrEmpty(dateTakenAsString))
                            return null;

                        var splitDateTime = dateTakenAsString.Split(' ');
                        var date = splitDateTime[0].Replace(':', '/');
                        var time = TimeSpan.Parse(splitDateTime[1]);

                        var reconstitutedDateTime = DateTime.Parse(date);
                        reconstitutedDateTime = reconstitutedDateTime.Add(time);

                        return reconstitutedDateTime;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }
        }

        private static string GetImageFileName(FileInfo fi, DateTime fileDateTime)
        {
            var newFileName = fi.DirectoryName + @"\" + fileDateTime.ToString("yyyyMMdd_HHmmss") + fi.Extension;
            var index = 1;

            while (new FileInfo(newFileName).Exists)
            {
                newFileName = fi.DirectoryName + @"\" + fileDateTime.AddSeconds(index).ToString("yyyyMMdd_HHmmss") + fi.Extension;
                //newFileName = fi.DirectoryName + @"\" + fileDateTime.ToString("yyyyMMdd_HHmmss") + index.ToString("_000") + fi.Extension;
                index++;
            }

            return newFileName;
        }
    }

    public enum ProcessMethod
    {
        ByDateTaken = 1,
        ByDateModified = 2,
        ByFileName = 3
    }
}
