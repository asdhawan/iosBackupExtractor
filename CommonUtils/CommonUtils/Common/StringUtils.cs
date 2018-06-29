using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CommonUtils {
    public static class StringUtils {

        public static string Truncate(this string source, int length) { return string.IsNullOrEmpty(source) || source.Length <= length ? source : source.Substring(0, length); }

        public static bool Contains(this string source, string toCheck, StringComparison comp) { return !string.IsNullOrEmpty(source) && source.IndexOf(toCheck, comp) >= 0; }

        public static bool StartsWith(this string source, string toCheck, StringComparison comp) { return !string.IsNullOrEmpty(source) && source.IndexOf(toCheck, comp) == 0; }

        public static string AddSeparatedValue(this string source, string valueToAdd, char separator) {
            string retVal = source;
            try {
                Dictionary<string, string> valuesDict = source.ToSeparatedValuesDict(separator);
                if (!ContainsSeparatedValue(source, valueToAdd, separator))
                    valuesDict.Add(valueToAdd, valueToAdd);
                retVal = string.Join(separator.ToString(), valuesDict.Values);
            } catch {/*Ignore*/}
            return retVal;
        }
        public static string RemoveSeparatedValue(this string source, string valueToRemove, char separator) {
            string retVal = source;
            try {
                Dictionary<string, string> valuesDict = source.ToSeparatedValuesDict(separator);
                if (ContainsSeparatedValue(source, valueToRemove, separator))
                    valuesDict.Remove(valueToRemove);
                retVal = string.Join(separator.ToString(), valuesDict.Values);
            } catch {/*Ignore*/}
            return retVal;
        }
        public static bool ContainsSeparatedValue(this string source, string valueToFind, char separator) {
            bool retVal = false;
            try {
                string existingValue = null;
                if (source.ToSeparatedValuesDict(separator).TryGetValue(valueToFind, out existingValue))
                    retVal = true;
            } catch {/*Ignore*/}
            return retVal;
        }
        public static Dictionary<string, string> ToSeparatedValuesDict(this string source, char separator) {
            Dictionary<string, string> retVal = null;
            try { retVal = source.Split(separator).ToDictionary(x => x, StringComparer.InvariantCultureIgnoreCase); } catch {/*Ignore*/}
            return retVal;
        }

        public static string Left(this string value, int maxLength) { return value.Truncate(maxLength); }

        public static string Right(this string value, int length) { return string.IsNullOrEmpty(value) || value.Length <= length ? value : value.Substring(value.Length - length, length); }
        
        public static string ToUpperNotNull(this string value) { return string.IsNullOrEmpty(value) ? string.Empty : value.ToUpper(); }

        public static string ToLowerNotNull(this string value) { return string.IsNullOrEmpty(value) ? string.Empty : value.ToLower(); }

        public static string ToTitleCase(this string value) {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(value.ToLower());
        }

        public static void TryAppend(this StringBuilder sb, Func<string> returnsStringDelegate, string appendOnError = null) {
            try { sb.Append(returnsStringDelegate.Invoke()); } catch { sb.Append(!string.IsNullOrEmpty(appendOnError) ? appendOnError : ""); }
        }

        public static string HtmlEncodeCRLF(this string value) { return value.Replace("\r\n", "<BR>"); }

        public static string RemoveSpecialCharacters(this string source, bool removeSpaces = true, bool removeDirectorySeparator = true) {
            string strClean = source.Replace('/', ' ');
            strClean = strClean.Replace("*", "");
            strClean = strClean.Replace("~", "");
            strClean = strClean.Replace("#", "");
            strClean = strClean.Replace("%", "");
            strClean = strClean.Replace("&", "");
            strClean = strClean.Replace("{", "");
            strClean = strClean.Replace("}", "");
            strClean = strClean.Replace("+", "");
            strClean = strClean.Replace("\"", "");
            strClean = strClean.Replace(":", "");
            strClean = strClean.Replace("?", "");
            strClean = strClean.Replace("<", "");
            strClean = strClean.Replace(">", "");
            strClean = strClean.Replace("|", "");

            if (removeDirectorySeparator)
                strClean = strClean.Replace("\\", "");

            if (removeSpaces) {
                strClean = strClean.Replace(" ", "_");
                strClean = strClean.Replace('\t', '_');
            }

            if (strClean.EndsWith("."))
                strClean = strClean.TrimEnd('.');

            return strClean;
        }

        public static string ToBase64String(this string stringToConvert) {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(stringToConvert);
            return Convert.ToBase64String(toEncodeAsBytes);
        }
        public static string FromBase64String(this string stringToConvert) {
            byte[] toDecodeAsBytes = Convert.FromBase64String(stringToConvert);
            return Encoding.ASCII.GetString(toDecodeAsBytes);
        }
        public static int? ToNullableInt(this string value, int ignoreVal = -1) {
            int? retVal = null;
            int parsedVal = ignoreVal;
            if (int.TryParse(value, System.Globalization.NumberStyles.Any, null, out parsedVal) && parsedVal != ignoreVal)
                retVal = parsedVal;
            return retVal;
        }
        public static string ToShortCurrencyString(this decimal num) {
            // Ensure number has max 3 significant digits (no rounding up can happen)
            long i = (long)Math.Pow(10, (int)Math.Max(0, Math.Log10((double)num) - 2));
            num = num / i * i;

            if (num >= 1000000000)
                return (num / 1000000000).ToString("0.##") + "B";
            if (num >= 1000000)
                return (num / 1000000).ToString("0.##") + "M";
            if (num >= 1000)
                return (num / 1000).ToString("0.##") + "K";

            return num.ToString("#,0");
        }
    }
}