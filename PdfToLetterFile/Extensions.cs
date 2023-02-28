namespace PdfToLetterFile
{
    public static class Extensions
    {
        public static DateTime ConvertToCentralTimeFromUtc(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
        }

        public static string ToTimerString(this TimeSpan span, Boolean includeMilliseconds = false)
        {
            var timerStr = $"{span.Minutes:00}m:{span.Seconds:00}s";
            return includeMilliseconds ? $"{timerStr}{span.Milliseconds:000}ms" : timerStr;
        }
    }
}
