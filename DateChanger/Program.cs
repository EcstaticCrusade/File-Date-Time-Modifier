using FileDateTimeModifier.Domain;
using System;
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

            Mp4FileProcessor.Process(ProcessMethod.ByFileName, baseFolderPath);
            //JpgFileProcessor.Process(ProcessMethod.ByDateModified, baseFolderPath);
            //ProcessGifFiles(baseFolderPath);
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
    }
}