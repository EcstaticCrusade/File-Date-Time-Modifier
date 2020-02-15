using FileDateTimeModifier.Domain;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DateChanger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Full Folder Path Of Files To Process:");
            var baseFolderPath = Console.ReadLine();
            //var files = Directory.EnumerateFiles(baseFolderPath);

            Mp4FileProcessor.ByFileName(baseFolderPath);

            //UpdateJpgMetaData_ByDateTaken(files);
            //UpdateJpgMetaData_ByFileName(files, baseFolderPath);
            //ProcessJpgFiles(files, null);
            //ProcessJpgFiles(files, new DateTime(2008, 06, 21, 18, 36, 36));
            //ProcessGifFiles(baseFolderPath);
            //ProcessMp4Files(baseFolderPath);
            //ProcessMp4Files(files, new DateTime(2014, 11, 08, 20, 32, 04));
            //ProcessMovFiles(files, new DateTime(2010, 08, 12, 08, 50, 48));
        }

        public static string GetImageFileName(FileInfo fi, DateTime fileDateTime)
        {
            var newFileName = fi.DirectoryName + @"\" + fileDateTime.ToString("yyyyMMdd_HHmmss") + fi.Extension; //" " + fi.Name;    // 
            var index = 1;

            while (new FileInfo(newFileName).Exists)
            {
                newFileName = fi.DirectoryName + @"\" + fileDateTime.ToString("yyyyMMdd_HHmmss") + index.ToString("_000") + fi.Extension;   // " " + fi.Name;    // 
                index++;
            }

            return newFileName;
        }

        /// <summary>
        /// Update the Create / Last Write / Last Access date time for all of the jpgs in the file IEnumerable
        /// </summary>
        /// <param name="files">Files to process</param>
        public static void ProcessJpgFiles(IEnumerable<string> files, DateTime? dateToUse)
        {
            var index = 0;
            foreach (var file in files.Where(f => f.ToLower().Contains(".jpg")))
            {
                var fi = new FileInfo(file);
                var correctDateTime = dateToUse.HasValue ? dateToUse.Value.AddMinutes(index) : GetDateFromDateTakenMetaData(file);

                if (!correctDateTime.HasValue)
                    continue;

                var newFileName = GetImageFileName(fi, correctDateTime.Value);

                File.SetCreationTime(file, correctDateTime.Value);
                File.SetLastWriteTime(file, correctDateTime.Value);
                File.SetLastAccessTime(file, correctDateTime.Value);

                File.Move(file, newFileName);
                index++;
            }
        }

        public static void ProcessGifFiles(string baseFolderPath)
        {
            var gifFolders = Directory.EnumerateDirectories(baseFolderPath);

            foreach (var folderName in gifFolders)
            {
                foreach (var fileName in Directory.EnumerateFiles(folderName).Where(f => f.Contains(".gif")))
                {
                    var dateTimePortionOfFolderName = folderName.Substring(folderName.LastIndexOf('\\') + 1);
                    var splitDateTime = dateTimePortionOfFolderName.Split(' ');
                    var date = splitDateTime[0].Replace('_', '/');
                    var time = TimeSpan.Parse(splitDateTime[1].Replace('_', ':'));

                    var reconstitutedDateTime = DateTime.Parse(date);
                    reconstitutedDateTime = reconstitutedDateTime.Add(time);

                    var fi = new FileInfo(fileName);
                    var newFileName = fi.DirectoryName + @"\" + reconstitutedDateTime.ToString("yyyyMMdd_HHmmss") + fi.Name;
                    File.SetCreationTime(fileName, reconstitutedDateTime);
                    File.SetLastWriteTime(fileName, reconstitutedDateTime);
                    File.SetLastAccessTime(fileName, reconstitutedDateTime);
                    File.Move(fileName, newFileName);
                }
            }
             
        }

        /// <summary>
        /// Gets the images date from the Date Taken metadata value
        /// Uses SixLabors ImageSharp NuGet Package
        /// </summary>
        /// <param name="imageFilePath">Full path of the image file</param>
        /// <returns></returns>
        public static DateTime? GetDateFromDateTakenMetaData(string imageFilePath)
        {
            using (var fs = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var myImage = Image.Load(fs))
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
            }
        }

        public static void ProcessMp4Files(string baseFolderPath, DateTime? dateTimeToUse)
        {
            var mp4Folders = Directory.EnumerateDirectories(baseFolderPath);

            foreach (var folderName in mp4Folders)
            {
                var dateTimePortionOfFolderName = folderName.Substring(folderName.LastIndexOf('\\') + 1);
                var splitDateTime = dateTimePortionOfFolderName.Split(' ');
                var date = splitDateTime[0].Replace('_', '/');
                var time = TimeSpan.Parse(splitDateTime[1].Replace('_', ':'));

                var reconstitutedDateTime = DateTime.Parse(date);
                reconstitutedDateTime = reconstitutedDateTime.Add(time);

                foreach (var fileName in Directory.EnumerateFiles(folderName).Where(f => f.ToLower().Contains(".mp4")))
                {
                    var fi = new FileInfo(fileName);
                    var newFileName = fi.DirectoryName + @"\" + reconstitutedDateTime.ToString("yyyyMMdd_HHmmss") + " " + fi.Name;
                    File.SetCreationTime(fileName, reconstitutedDateTime);
                    File.SetLastWriteTime(fileName, reconstitutedDateTime);
                    File.SetLastAccessTime(fileName, reconstitutedDateTime);
                    File.Move(fileName, newFileName);
                }
            }
        }

        /// <summary>
        /// Update the Create / Last Write / Last Access date time for all of the jpgs in the file IEnumerable
        /// </summary>
        /// <param name="files">Files to process</param>
        public static void UpdateJpgMetaData_ByDateTaken(IEnumerable<string> files)
        {
            var index = 0;
            foreach (var file in files.Where(f => f.ToLower().Contains(".jpg")))
            {
                var fi = new FileInfo(file);
                var correctDateTime = GetDateFromDateTakenMetaData(file);

                if (!correctDateTime.HasValue)
                    continue;

                var newFileName = GetImageFileName(fi, correctDateTime.Value);

                File.SetCreationTime(file, correctDateTime.Value);
                File.SetLastWriteTime(file, correctDateTime.Value);
                File.SetLastAccessTime(file, correctDateTime.Value);

                File.Move(file, newFileName);
                index++;
            }
        }

        /// <summary>
        /// Update the Create / Last Write / Last Access date time for all of the jpgs in the file IEnumerable
        /// </summary>
        /// <param name="files">Files to process</param>
        public static void UpdateJpgMetaData_ByFileName(IEnumerable<string> files, string baseFolderPath)
        {
            var index = 0;
            var extension = ".jpg";
            foreach (var file in files.Where(f => f.ToLower().Contains(extension)))
            {
                try
                {
                    var fi = new FileInfo(file);
                    
                    var fileName = fi.Name.ToLower().Replace(extension, string.Empty);

                    var dateTaken = DateTimeExtractor.FromFileName(fileName, ' '); //GetDateTime_FromFileName(fileName, ' ');

                    var newFileName = $"{fi.DirectoryName}\\{dateTaken.ToString("yyyyMMdd_HHmmss")}{fi.Extension}";

                    using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        using (var myImage = Image.Load(fs))
                        {
                            myImage.Metadata.ExifProfile.SetValue(SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifTag.DateTimeOriginal, dateTaken.ToString());
                            myImage.Metadata.ExifProfile.SetValue(SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifTag.DateTimeDigitized, dateTaken.ToString());
                            myImage.Metadata.ExifProfile.SetValue(SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifTag.DateTime, dateTaken.ToString());
                            myImage.Save(newFileName);
                        }
                    }
                    
                    File.SetCreationTime(newFileName, dateTaken);
                    File.SetLastWriteTime(newFileName, dateTaken);
                    File.SetLastAccessTime(newFileName, dateTaken);
                    File.Delete(fi.FullName);
                }
                catch (Exception ex)
                {
                    throw;
                }

                index++;
            }
        }
    }
}