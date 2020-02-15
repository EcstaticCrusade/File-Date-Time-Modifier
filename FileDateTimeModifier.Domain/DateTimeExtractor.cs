using System;

namespace FileDateTimeModifier.Domain
{
    public static class DateTimeExtractor
    {
        public static DateTime FromFileName(string fileName, char characterToSplitOn)
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
