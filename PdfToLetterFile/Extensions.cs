namespace PdfToLetterFile
{
    public static class Extensions
    {
        public static DateTime ConvertToCentralTimeFromUtc(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
        }

        public static string ToReadableString(this TimeSpan span)
        {
            //if minutes > 0, don't show milliseconds
            string formatted = span.Minutes > 0 ? string.Format("{0}{1}{2}{3}",
                                                    span.Days == 1 ? string.Format("{0:0} day, ", span.Days) : span.Days > 0 ? string.Format("{0:0} days, ", span.Days) : string.Empty,
                                                    span.Hours == 1 ? string.Format("{0:0} hour, ", span.Hours) : span.Hours > 0 ? string.Format("{0:0} hours, ", span.Hours) : string.Empty,
                                                    span.Minutes == 1 ? string.Format("{0:0} minute, ", span.Minutes) : span.Minutes > 0 ? string.Format("{0:0} minutes, ", span.Minutes) : string.Empty,
                                                    span.Seconds == 1 ? string.Format("{0:0} second, ", span.Seconds) : span.Seconds > 0 ? string.Format("{0:0} seconds, ", span.Seconds) : string.Empty)

                                                    :

                                                    string.Format("{0}{1}{2}{3}{4}",
                                                    span.Days == 1 ? string.Format("{0:0} day, ", span.Days) : span.Days > 0 ? string.Format("{0:0} days, ", span.Days) : string.Empty,
                                                    span.Hours == 1 ? string.Format("{0:0} hour, ", span.Hours) : span.Hours > 0 ? string.Format("{0:0} hours, ", span.Hours) : string.Empty,
                                                    span.Minutes == 1 ? string.Format("{0:0} minute, ", span.Minutes) : span.Minutes > 0 ? string.Format("{0:0} minutes, ", span.Minutes) : string.Empty,
                                                    span.Seconds == 1 ? string.Format("{0:0} second, ", span.Seconds) : span.Seconds > 0 ? string.Format("{0:0} seconds, ", span.Seconds) : string.Empty,
                                                    span.Milliseconds == 1 ? string.Format("{0:0} millisecond", span.Milliseconds) : span.Milliseconds > 0 ? string.Format("{0:0} millseconds", span.Milliseconds) : string.Empty);


            if (formatted.EndsWith(", ")) formatted = formatted.Substring(0, formatted.Length - 2);

            return formatted;
        }
    }
}
