using System;
using System.Text;

namespace Common.Utility
{
    public static class DateTimeUtil
    {
        private static StringBuilder helperStringBuilder = new StringBuilder();
        
        public static long GetEpochTimeSeconds()
        {
            DateTimeOffset offset =  DateTime.UtcNow;
            return offset.ToUnixTimeSeconds();
        }
        
        public static string Format(long timeInSeconds)
        {
            var hours = (int)(timeInSeconds / 3600);
            if (hours > 0)
            {
                helperStringBuilder.Append(hours.ToString("D2"));
                helperStringBuilder.Append(":");
            }
            timeInSeconds = timeInSeconds % 3600;
            
            var minutes = (int)(timeInSeconds / 60);
            helperStringBuilder.Append(minutes.ToString("D2"));
            helperStringBuilder.Append(":");
            
            var seconds = (int)(timeInSeconds % 60);
            helperStringBuilder.Append(seconds.ToString("D2"));

            var str = helperStringBuilder.ToString();
            helperStringBuilder.Clear();
            return str;
        }
    }
}