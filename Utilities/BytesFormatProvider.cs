using System;

namespace OpenHardwareMonitor.Utilities
{
    class BytesFormatProvider : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter)) return this;
            return null;
        }

        private static BytesFormatProvider instance = new BytesFormatProvider();
        public static BytesFormatProvider Instance { get { return instance; } }

        private const string fileSizeFormat = "bf";
        private const int OneKiloByte = 1024;
        private const int OneMegaByte = OneKiloByte * 1024;
        private const int OneGigaByte = OneMegaByte * 1024;

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format == null || !format.StartsWith(fileSizeFormat))
                return defaultFormat(format, arg, formatProvider);

            if (arg is string)
                return defaultFormat(format, arg, formatProvider);

            Decimal size;

            try
            {
                size = Convert.ToDecimal(arg);
            }
            catch (InvalidCastException)
            {
                return defaultFormat(format, arg, formatProvider);
            }

            string suffix;
            string precision = format.Substring(2);
            if (String.IsNullOrEmpty(precision)) precision = "1";

            if (size > OneGigaByte)
            {
                size /= OneGigaByte;
                suffix = " GB";
            }
            else if (size > OneMegaByte)
            {
                size /= OneMegaByte;
                suffix = " MB";
            }
            else if (size > OneKiloByte)
            {
                size /= OneKiloByte;
                suffix = " kB";
            }
            else
            {
                suffix = " B";
                precision = "0";
            }

            return String.Format("{0:N" + precision + "}{1}", size, suffix);

        }

        private static string defaultFormat(string format, object arg, IFormatProvider formatProvider)
        {
            IFormattable formattableArg = arg as IFormattable;
            if (formattableArg != null)
                return formattableArg.ToString(format, formatProvider);

            return arg.ToString();
        }
    }
}
