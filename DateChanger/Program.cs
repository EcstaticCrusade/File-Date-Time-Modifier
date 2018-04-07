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

            ProcessJpgFiles(files);
            ProcessGifFiles(baseFolderPath);
            ProcessMp4Files(baseFolderPath);
            //ProcessMp4Files(files, new DateTime(2014, 12, 25, 08, 12, 04));
        }
        
        public static string GetImageFileName(FileInfo fi, DateTime fileDateTime)
        {
            var newFileName = fi.DirectoryName + @"\" + fileDateTime.ToString("yyyyMMdd_hhmmss") + fi.Extension; //" " + fi.Name;    // 
            var index = 1;

            while (new FileInfo(newFileName).Exists)
            {
                newFileName = fi.DirectoryName + @"\" + fileDateTime.ToString("yyyyMMdd_hhmmss") + index.ToString("_000") + fi.Extension;   // " " + fi.Name;    // 
                index++;
            }

            return newFileName;
        }

        /// <summary>
        /// Update the Create / Last Write / Last Access date time for all of the jpgs in the file IEnumerable
        /// </summary>
        /// <param name="files">Files to process</param>
        public static void ProcessJpgFiles(IEnumerable<string> files)
        {
            foreach (var file in files.Where(f => f.Contains(".jpg")))
            {
                var fi = new FileInfo(file);
                var correctDateTime = GetDateFromDateTakenMetaData(file);
                var newFileName = GetImageFileName(fi, correctDateTime);

                File.SetCreationTime(file, correctDateTime);
                File.SetLastWriteTime(file, correctDateTime);
                File.SetLastAccessTime(file, correctDateTime);

                File.Move(file, newFileName);
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
        public static DateTime GetDateFromDateTakenMetaData(string imageFilePath)
        {
            using (var fs = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var myImage = Image.Load(fs))
                {
                    // Results in a value like YYYY/MM/DD HH:MM:SS
                    var dateTakenAsString = myImage.MetaData.ExifProfile.Values.ToList().First(v => v.Tag == SixLabors.ImageSharp.MetaData.Profiles.Exif.ExifTag.DateTimeOriginal).Value.ToString();

                    var splitDateTime = dateTakenAsString.Split(' ');

                    var date = splitDateTime[0].Replace(':', '/');
                    var time = TimeSpan.Parse(splitDateTime[1]);

                    var reconstitutedDateTime = DateTime.Parse(date);
                    reconstitutedDateTime = reconstitutedDateTime.Add(time);
                    
                    return reconstitutedDateTime;
                }
            }
        }

        //public static DateTime GetDateTakenFromImage(string path)
        //{
        //    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        //    {
        //        using (var myImage = Image.Load(fs))
        //        {
        //            var dateTakenAsString = myImage.MetaData.ExifProfile.Values.ToList().First(v => v.Tag == SixLabors.ImageSharp.MetaData.Profiles.Exif.ExifTag.DateTimeOriginal).Value.ToString();

        //            var splitDateTime = dateTakenAsString.Split(' ');

        //            var date = splitDateTime[0].Replace(':', '/');
        //            var time = splitDateTime[1].Split(':') ;
        //            var hours = int.Parse(time[0]);
        //            var minutes = int.Parse(time[1]);
        //            var seconds = int.Parse(time[2]);

        //            var reconstitudedDateTime = DateTime.Parse(date);
        //            reconstitudedDateTime = reconstitudedDateTime.AddHours(hours);
        //            reconstitudedDateTime = reconstitudedDateTime.AddMinutes(minutes);
        //            reconstitudedDateTime = reconstitudedDateTime.AddSeconds(seconds);

        //            return reconstitudedDateTime;

        //            //dateTakenAsString.Replace()

        //            return DateTime.Parse(dateTakenAsString);
        //            //return actualDateTime.ToString("yyyyMMdd_hhmmss");
        //        }
        //    }
            
        //    //using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        //    //using (Image myImage = Image.FromStream(fs, false, false))
        //    //{
        //    //    PropertyItem propItem = myImage.GetPropertyItem(36867);
        //    //    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
        //    //    return DateTime.Parse(dateTaken);
        //    //}
        //}

        public static void ProcessMp4Files(string baseFolderPath)
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

                foreach (var fileName in Directory.EnumerateFiles(folderName).Where(f => f.Contains(".mp4")))
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

        //public static void ProcessMp4Files(IEnumerable<string> files, DateTime? newDateTime)
        //{
        //    foreach (var file in files.Where(f => f.Contains(".mp4")))
        //    {
        //        var fi = new FileInfo(file);

        //        if (newDateTime.HasValue)
        //        {
        //            var newFileName = fi.DirectoryName + @"\" + newDateTime.Value.ToString("yyyyMMdd_hhmmss") + fi.Extension;
        //            File.SetCreationTime(file, newDateTime.Value);
        //            File.SetLastWriteTime(file, newDateTime.Value);
        //            File.SetLastAccessTime(file, newDateTime.Value);
        //            File.Move(file, newFileName);
        //        }
        //    }
        //}
    }
}
