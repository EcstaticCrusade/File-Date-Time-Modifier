using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileDateTimeModifier.Domain
{
    public static class ImageFileProcessor
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
                case ProcessMethod.ByFileName:
                    ByFileName(baseFolderPath);
                    break;
            }
        }

        private static void ByDateTaken(string baseFolderPath)
        {
            var listOfImageTypes = new List<string>() { ".jpg", ".png", ".gif" };

            foreach (var fileName in Directory.GetFiles(baseFolderPath))
            {
                var fi = new FileInfo(fileName);

                if (listOfImageTypes.Contains(fi.Extension) == false)
                    continue;

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
            var listOfImageTypes = new List<string>() { ".jpg", ".png", ".gif" };

            foreach (var fileName in Directory.GetFiles(baseFolderPath))
            {
                var fi = new FileInfo(fileName);

                if (listOfImageTypes.Contains(fi.Extension) == false)
                    continue;

                var correctDateTime = fi.LastWriteTime;
                var newFileName = GetImageFileName(fi, correctDateTime);
                
                using (var image = Image.Load(fi.FullName))
                {   // Update date taken value
                    image.Metadata.ExifProfile.SetValue<string>(SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifTag.DateTimeOriginal, correctDateTime.ToString());
                }
                
                File.SetCreationTime(fileName, correctDateTime);
                File.SetLastWriteTime(fileName, correctDateTime);
                File.SetLastAccessTime(fileName, correctDateTime);

                File.Move(fileName, newFileName);
            }
        }

        private static void ByFileName(string baseFolderPath)
        {
            var listOfImageTypes = new List<string>() { ".jpg", ".png", ".gif" };

            foreach (var fileName in Directory.EnumerateFiles(baseFolderPath))
            {
                var fi = new FileInfo(fileName);

                if (listOfImageTypes.Contains(fi.Extension) == false)
                    continue;

                var dateTaken = DateTimeExtractor.FromFileName(fi.Name, '_');

                var newFileName = dateTaken.ToString("yyyyMMdd_HHmmss");

                var newFilePath = $"{fi.DirectoryName}\\{newFileName}{fi.Extension}";

                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (var myImage = Image.Load(fs))
                    {
                        myImage.Metadata.ExifProfile?.SetValue(SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifTag.DateTimeOriginal, dateTaken.ToString());
                    }
                }


                //using (var image = Image.Load(fi.FullName))
                //{   // Update date taken value
                //    image.Metadata.ExifProfile.SetValue(SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifTag.DateTimeOriginal, dateTaken.ToString());
                //}

                File.SetCreationTime(fileName, dateTaken);
                File.SetLastWriteTime(fileName, dateTaken);
                File.SetLastAccessTime(fileName, dateTaken);
                File.Move(fileName, newFilePath);
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
                        var dateTakenAsString = myImage.Metadata?.ExifProfile?.Values.ToList().FirstOrDefault(v => v.Tag == SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifTag.DateTimeOriginal).GetValue().ToString();

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
}
