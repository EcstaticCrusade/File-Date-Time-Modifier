using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DateChanger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Full Folder Path Of Files To Process:");
            var baseFolderPath = Console.ReadLine();
            var files = Directory.EnumerateFiles(baseFolderPath);

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
                    var dateTakenAsString = myImage.MetaData.ExifProfile.Values.ToList().FirstOrDefault(v => v.Tag == SixLabors.ImageSharp.MetaData.Profiles.Exif.ExifTag.DateTimeOriginal)?.Value.ToString();

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

        //public static void ProcessMovFiles(IEnumerable<string> files, DateTime? dateTimeToUse)
        //{
        //    var index = 0;
        //    foreach (var file in files.Where(f => f.ToLower().Contains(".mp4")))
        //    {
        //        var fi = new FileInfo(file);
        //        var correctDateTime = dateTimeToUse.HasValue ? dateTimeToUse.Value.AddMinutes(index) : GetDateFromDateTakenMetaData(file);
        //        var newFileName = GetImageFileName(fi, correctDateTime);

        //        File.SetCreationTime(file, correctDateTime);
        //        File.SetLastWriteTime(file, correctDateTime);
        //        File.SetLastAccessTime(file, correctDateTime);

        //        File.Move(file, newFileName);
        //        index++;
        //    }
        //}

        ////public static void ProcessMp4Files(IEnumerable<string> files, DateTime? newDateTime)
        ////{
        ////    foreach (var file in files.Where(f => f.Contains(".mp4")))
        ////    {
        ////        var fi = new FileInfo(file);

        ////        if (newDateTime.HasValue)
        ////        {
        ////            var newFileName = fi.DirectoryName + @"\" + newDateTime.Value.ToString("yyyyMMdd_hhmmss") + fi.Extension;
        ////            File.SetCreationTime(file, newDateTime.Value);
        ////            File.SetLastWriteTime(file, newDateTime.Value);
        ////            File.SetLastAccessTime(file, newDateTime.Value);
        ////            File.Move(file, newFileName);
        ////        }
        ////    }
        ////}

        //// https://github.com/mono/taglib-sharp
        //public static void Stuff()
        //{
        //    var tfile = TagLib.File.Create(@"C:\My video.avi");
        //    string title = tfile.Tag.Title;
        //    TimeSpan duration = tfile.Properties.Duration;
        //    Console.WriteLine("Title: {0}, duration: {1}", title, duration);

        //    // change title in the file
        //    tfile.Tag.Title = "my new title";
        //    tfile.Save();
        //}


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

                    var dateTaken = GetDateTime_FromFileName(fileName, ' ');

                    var newFileName = $"{fi.DirectoryName}\\{dateTaken.ToString("yyyyMMdd_HHmmss")}{fi.Extension}";

                    using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        using (var myImage = Image.Load(fs))
                        {
                            myImage.MetaData.ExifProfile.SetValue(SixLabors.ImageSharp.MetaData.Profiles.Exif.ExifTag.DateTimeOriginal, dateTaken.ToString());
                            myImage.MetaData.ExifProfile.SetValue(SixLabors.ImageSharp.MetaData.Profiles.Exif.ExifTag.DateTimeDigitized, dateTaken.ToString());
                            myImage.MetaData.ExifProfile.SetValue(SixLabors.ImageSharp.MetaData.Profiles.Exif.ExifTag.DateTime, dateTaken.ToString());
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

        private static DateTime GetDateTime_FromFileName(string fileName, char characterToSplitOn)
        {
            var split = fileName.Split(characterToSplitOn);
            var date = split[0];

            var year = int.Parse(date.Substring(0, 4));
            var month = int.Parse(date.Substring(4, 2));
            var day = int.Parse(date.Substring(6, 2));

            var time = split[1];

            var hour = int.Parse(time.Substring(0, 2));
            var minute = int.Parse(time.Substring(2, 2));
            var second = int.Parse(time.Substring(4, 2));

            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}
