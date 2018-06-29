using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CommonUtils {
    public static class Utils {
        public class GenericComparer<T> : IEqualityComparer<T> {
            private readonly Func<T, object> _funcDistinct;
            public GenericComparer(Func<T, object> funcDistinct) { this._funcDistinct = funcDistinct; } 
            public bool Equals(T x, T y) { return _funcDistinct(x).Equals(_funcDistinct(y)); }
            public int GetHashCode(T obj) { return this._funcDistinct(obj).GetHashCode(); }
        }

        private static Random passwordGenerationRandomizer = new Random();

        public static bool NullEmptyCheck(object obj) {
            bool bNullOrEmpty = false;
            if (obj == null)
                bNullOrEmpty = true;
            else if (obj is string)
                bNullOrEmpty = string.IsNullOrEmpty(obj.ToString());
            else if (obj is int)
                bNullOrEmpty = ((int)obj == -1 || (int)obj == int.MinValue);
            else if (obj is long)
                bNullOrEmpty = ((long)obj == -1 || (long)obj == long.MinValue);
            else if (obj is decimal)
                bNullOrEmpty = ((decimal)obj == -1 || (decimal)obj == decimal.MinValue);
            else if (obj is DateTime)
                bNullOrEmpty = ((DateTime)obj == DateTime.MinValue);
            else if (obj.ToString().Equals("-1"))
                bNullOrEmpty = true;

            return bNullOrEmpty;
        }

        /// <summary>
        /// Get user ID from a SharePoint login name. Perform formating to get just username.
        /// </summary>
        /// <param name="_myTeamSite"></param>
        /// <returns></returns>
        public static string GetUserID(string strCurrentUserName) {
            char[] delimiterChars = { '\\' };
            char[] delimiterChars2 = { ':' };

            try {
                if (strCurrentUserName != string.Empty) {
                    //Apply '\\'
                    string[] arrUserID = strCurrentUserName.Split(delimiterChars);
                    if (arrUserID.GetLength(0) == 2) {
                        strCurrentUserName = arrUserID[1].ToString();
                    }
                    //Apply ':'
                    string[] arrUserID2 = strCurrentUserName.Split(delimiterChars2);
                    if (arrUserID2.GetLength(0) == 2) {
                        strCurrentUserName = arrUserID2[1].ToString();
                    }
                }
            } catch (Exception) { /* TODO: Do something with this error */ }

            return strCurrentUserName;
        }

        public static void WriteToEventLog(string source, string message, EventLogEntryType entryType) {
            try { EventLog.WriteEntry(source, message, entryType); } catch { /* Ignore */ }
        }

        public static string HashtableToString(Hashtable hash) {
            string retVal = "";
            if (hash != null) {
                foreach (string key in hash.Keys) {
                    retVal += "[" + key + "=" + ((hash[key] != null) ? hash[key].ToString() : "null") + "]";
                }
            }
            return retVal;
        }

        public static XmlDocument GetParametersXmlDocument(params object[] paramArray) {
            StackFrame frame = new StackFrame(1);
            MethodBase method = frame.GetMethod();
            return GetParametersXmlDocument(method, paramArray);
        }

        public static XmlDocument GetParametersXmlDocument(MethodBase method, params object[] paramArray) {
            ParameterInfo[] paramInfoArray = method.GetParameters();
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            writer.WriteStartDocument();
            writer.WriteStartElement("Method");
            writer.WriteElementString("Name", method.Name);

            writer.WriteStartElement("Parameters");
            if (paramInfoArray.Length == paramArray.Length) {
                for (int i = 0; i < paramArray.Length; i++) {
                    writer.WriteElementString(paramInfoArray[i].Name, paramArray[i].ToString());
                }
            }
            writer.WriteEndElement(); //</Parameters>
            writer.WriteEndElement(); //</Method>
            writer.WriteEndDocument();

            writer.Flush();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sb.ToString());

            return xmlDocument;
        }

        public static XmlDocument GetSerializableObjectXmlDocument<T>(T obj, string xmlNamespace = null) {
            XmlSerializer xs = xmlNamespace != null ? new XmlSerializer(typeof(T), xmlNamespace) : new XmlSerializer(typeof(T));
            StringWriter sw = new StringWriter();
            xs.Serialize(sw, obj);
            XmlDocument xdResponse = new XmlDocument();
            xdResponse.LoadXml(sw.ToString());
            return xdResponse;
        }

        public static T GetXmlDocumentAsSerializableObject<T>(XmlDocument xd, string rootElementName, string xmlNamespace = null) where T : new() {
            T retObj = default(T);
            try {
                XmlRootAttribute xra = xmlNamespace != null ? new XmlRootAttribute() { ElementName = rootElementName, Namespace = xmlNamespace, IsNullable = true } : new XmlRootAttribute() { ElementName = rootElementName, IsNullable = true };
                XmlSerializer xs = new XmlSerializer(typeof(T), xra);
                StringReader sr = new StringReader(xd.OuterXml);
                retObj = (T)xs.Deserialize(sr);
            } catch { /*Ignore*/ }
            return retObj;
        }

        public static T GetBlobAsSerializableObject<T>(byte[] blob, string rootElementName, string xmlNamespace = null) where T : new() {
            XmlDocument xd = new XmlDocument();
            using (MemoryStream ms = new MemoryStream(blob)) { xd.Load(ms); }
            return GetXmlDocumentAsSerializableObject<T>(xd, rootElementName, xmlNamespace);
        }

        public static T GetFileAsSerializableObject<T>(string filePath, string rootElementName, string xmlNamespace = null) where T : new() {
            XmlDocument xd = new XmlDocument();
            xd.Load(filePath);
            return GetXmlDocumentAsSerializableObject<T>(xd, rootElementName, xmlNamespace);
        }

        public static string CreateRandomToken(int tokenLength) {
            if (tokenLength < 6)
                throw new Exception("Token Length must be greater than or equal to 6");
            return CreateRandomString(tokenLength, false, false);
        }

        public static string CreateRandomString(
            int stringLength,
            bool bAllowLowerCase = true,
            bool bAllowSpecialChars = true,
            string allowedUpperCase = null,
            string allowedLowerCase = null,
            string allowedDigits = null,
            string allowedSpecialChars = null
            ) {
            allowedUpperCase = string.IsNullOrEmpty(allowedUpperCase) ? "ABCDEFGHJKLMNPQRSTUVWXYZ" : allowedUpperCase;
            allowedLowerCase = string.IsNullOrEmpty(allowedLowerCase) ? "abcdefghijkmnpqrstuvwxyz" : allowedLowerCase;
            allowedDigits = string.IsNullOrEmpty(allowedDigits) ? "123456789" : allowedDigits;
            allowedSpecialChars = string.IsNullOrEmpty(allowedSpecialChars) ? "!@$?~#%^&*" : allowedSpecialChars;
            //string allowedUpperCase = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            //string allowedLowerCase = "abcdefghijkmnpqrstuvwxyz";
            //string allowedSpecialChars = "!@$?~#%^&*";
            //string allowedDigits = "123456789";
            char[] chars = new char[stringLength];
            if (passwordGenerationRandomizer == null)
                passwordGenerationRandomizer = new Random();

            //make first character upper-case
            chars[0] = allowedUpperCase[passwordGenerationRandomizer.Next(0, allowedUpperCase.Length)];
            //make remaining but last 2 characters alternate between lower-case and digits
            bool bDigit = false;
            for (int i = 1; i < stringLength - 2; i++) {
                if (bDigit) {
                    chars[i] = allowedDigits[passwordGenerationRandomizer.Next(0, allowedDigits.Length)];
                    bDigit = false;
                } else {
                    string charSourceString = bAllowLowerCase ? allowedLowerCase : allowedUpperCase;
                    chars[i] = charSourceString[passwordGenerationRandomizer.Next(0, allowedLowerCase.Length)];
                    bDigit = true;
                }
            }
            //make second-last character a special char
            string specialSourceString = bAllowSpecialChars ? allowedSpecialChars : allowedDigits;
            chars[stringLength - 2] = specialSourceString[passwordGenerationRandomizer.Next(0, specialSourceString.Length)];
            //make last character a digit
            chars[stringLength - 1] = allowedDigits[passwordGenerationRandomizer.Next(0, allowedDigits.Length)];

            return new string(chars);
        }

        public static Dictionary<string, int> GetDictionaryFromEnum(Type enumType) {
            //container to be returned
            Dictionary<string, int> items = new Dictionary<string, int>();
            try {
                // break down the enumerator items into key/value pairs
                string[] names = Enum.GetNames(enumType);
                Array values = Enum.GetValues(enumType);
                object o = null;
                int enumIntVal = -1;
                // piece together the key/value pairs into the dictionary
                for (int i = 0; i <= names.Length - 1; i++) {
                    if ((o = values.GetValue(i)) != null && (enumIntVal = (int)o) >= 0)
                        items.Add(names[i], enumIntVal);
                }
            } catch { }
            return items;
        } //function

        public static bool IsValidEmail(string stringToCheck) {
            bool bRetVal = true;
            string[] emails = stringToCheck.Trim().Replace(";", ",").Split(',');
            foreach (string email in emails) {
                bRetVal &= System.Text.RegularExpressions.Regex.IsMatch(email, @"^[\w\.-]+\@[\w\.-]+\w\.\w{2,3}$");
            }
            return bRetVal;
        }

        public static string CleanEmail(string stringToClean) { return stringToClean.Trim().Replace(";", ","); }

        public static List<string> GetEmailList(string stringWithEmails) { return CleanEmail(stringWithEmails).Split(',').Distinct().ToList(); }

        public static IEnumerable<T>[] SplitList<T>(this IEnumerable<T> list, int numSplits) {
            int i = 0;
            return list.GroupBy(x => (i++ % numSplits)).Select(group => group.AsEnumerable()).ToArray();
        }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action) {
            foreach (T item in enumeration.ToList()) {
                action(item);
            }
        }

        public static IEnumerable<T> TakeAny<T>(this IEnumerable<T> source, int? count) { return !count.HasValue ? source : source.Take(count.Value); }

        public static int GetQuarter(this DateTime dt) { return ((dt.Month + 2) / 3); }

        public static T GetAttribute<T>(this Enum enumValue) where T : Attribute {
            return enumValue
                .GetType()
                .GetTypeInfo()
                .GetDeclaredField(enumValue.ToString())
                .GetCustomAttribute<T>();
        }

        public static Dictionary<Enum, string> GetEnumDictionary(Type enumType) { return Enum.GetValues(enumType).Cast<Enum>().ToDictionary(x => x, x => x.GetAttribute<DisplayTextAttribute>()?.DisplayText ?? x.ToString()); }

        public static ParallelLoopResult TryParallelForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body) {
            ParallelLoopResult result;
            try {
                result = Parallel.ForEach<TSource>(source, body);
            } catch (AggregateException aggrEx) {
                string exceptionMessage = string.Join(";", aggrEx.InnerExceptions?.Select(x => x.Message));
                throw aggrEx.InnerExceptions?.Count == 1 ? aggrEx.InnerExceptions[0] : new Exception(exceptionMessage);
            }
            return result;
        }
    }
}
