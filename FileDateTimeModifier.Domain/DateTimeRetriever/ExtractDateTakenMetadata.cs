using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Linq;

namespace FileDateTimeModifier.Domain.DateTimeRetriever
{
    /// <summary>
    /// Gets the image's date from the Date Taken metadata value
    /// Uses SixLabors ImageSharp NuGet Package
    /// </summary>
    public class ExtractImageDateTakenMetadata : IDateTimeRetriever
    {
        /// <summary>
        /// Get the date
        /// </summary>
        /// <param name="fullFilePath">Full path of the image file</param>
        /// <returns>Datetime value stored in the Date Taken metadata field</returns>
        public DateTime? RetrieveDateTime(string fullFilePath)
        {
            using (var fs = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var myImage = Image.Load(fs))
                {
                    // Results in a value like YYYY/MM/DD HH:MM:SS
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
    }
}
