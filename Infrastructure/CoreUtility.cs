using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;

namespace SayHenTai_WebApp.Infrastructure
{
    public static class CoreUtility
    {
        private static readonly string base64Character62 = "+";
        private static readonly string base64Character63 = "/";
        private static readonly string base64UrlCharacter62 = "-qi-";
        private static readonly string _base64UrlCharacter63 = "_qi_";

        /// <summary>
        /// Hàm cắt ngắn một chuỗi
        /// Nếu nẻ một chữ thì bỏ chữ đó cho đến dấu khoảng cách cuối cùng
        /// </summary>
        /// <param name="sentence">Chuỗi cần cắt</param>
        /// <param name="len">Độ dài</param>
        /// <param name="expanded"></param>
        /// <returns>Chuỗi cộng thêm sau khi cắt ngắn</returns>
        public static string TruncateString(string sentence, int len, string expanded = "...")
        {
            if (sentence == null)
            {
                return string.Empty;
            }

            len -= expanded?.Length ?? 0;

            if (sentence.Length > len)
            {
                sentence = sentence.Substring(0, len);
                int pos = sentence.LastIndexOf(' ');
                if (pos > 0)
                {
                    sentence = sentence.Substring(0, pos);
                }

                return sentence + expanded;
            }
            return sentence;
        }

        /// <summary>
        /// Tạo url bằng một đoạn unicode string
        /// </summary>
        /// <param name="text">Chuỗi string</param>
        /// <param name="maxLength">Độ dài url tối đa</param>
        /// <returns>string</returns>
        public static string UrlFromUnicode(string text, int maxLength = 150)
        {
            if (text == null) return "";
            var normalizedString = text.ToUpperInvariant().Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            var stringLength = normalizedString.Length;
            var prevdash = false;
            var trueLength = 0;
            char c;
            for (int i = 0; i < stringLength; i++)
            {
                c = normalizedString[i];
                switch (CharUnicodeInfo.GetUnicodeCategory(c))
                {
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        if (c < 128)
                            stringBuilder.Append(c);
                        else
                            stringBuilder.Append(RemapInternationalCharToAscii(c));
                        prevdash = false;
                        trueLength = stringBuilder.Length;
                        break;
                    case UnicodeCategory.SpaceSeparator:
                    case UnicodeCategory.ConnectorPunctuation:
                    case UnicodeCategory.DashPunctuation:
                    case UnicodeCategory.OtherPunctuation:
                    case UnicodeCategory.MathSymbol:
                        if (!prevdash)
                        {
                            stringBuilder.Append('-');
                            prevdash = true;
                            trueLength = stringBuilder.Length;
                        }
                        break;
                }
                if (maxLength > 0 && trueLength >= maxLength)
                    break;
            }
            var result = stringBuilder.ToString().Trim('-').ToLower(CultureInfo.InvariantCulture);
            return maxLength <= 0 || result.Length <= maxLength ? result : result.Substring(0, maxLength);
        }

        /// <summary>
        /// Chuyển ký tự unicode sang ascii
        /// </summary>
        /// <param name="c">Ký tự</param>
        /// <returns>string</returns>
        public static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString(CultureInfo.InvariantCulture).ToUpperInvariant();
            if ("àåáâäãåą".Contains(s, StringComparison.OrdinalIgnoreCase))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s, StringComparison.OrdinalIgnoreCase))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s, StringComparison.OrdinalIgnoreCase))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s, StringComparison.OrdinalIgnoreCase))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s, StringComparison.OrdinalIgnoreCase))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s, StringComparison.OrdinalIgnoreCase))
            {
                return "c";
            }
            else if ("żźž".Contains(s, StringComparison.OrdinalIgnoreCase))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s, StringComparison.OrdinalIgnoreCase))
            {
                return "s";
            }
            else if ("ñń".Contains(s, StringComparison.OrdinalIgnoreCase))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s, StringComparison.OrdinalIgnoreCase))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s, StringComparison.OrdinalIgnoreCase))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Cắt một bỏ một đoạn ở cuối một chuỗi
        /// </summary>
        /// <param name="input">Chuỗi ban đầu</param>
        /// <param name="suffixToRemove">Chuỗi cần cắt bỏ</param>
        /// <param name="comparisonType">Loại so sánh chuỗi cắt bỏ</param>
        /// <returns>string</returns>
        public static string TrimEnd(this string input, string suffixToRemove, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (input != null && suffixToRemove != null && input.EndsWith(suffixToRemove, comparisonType))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }
            else
            {
                return input;
            }
        }

        /// <summary>
        /// Lấy số ip cuối cho quản trị viện tại trang đăng nhập
        /// </summary>
        /// <returns>string</returns>
        public static string GetLastNumIpThisServer()
        {
            string lastNumberIp = "";
            var hostName = Dns.GetHostName();
            if (!string.IsNullOrEmpty(hostName))
            {
                var ips = Dns.GetHostAddresses(hostName);
                var query = ips.Select(q => q.ToString()).Where(q => q.StartsWith("192.168.1.") || q.StartsWith("172.16.40.") || q.StartsWith("172.17.") || q.StartsWith("120.72.117."));
                lastNumberIp = string.Join(", ", query);
            }
            return lastNumberIp;
        }

        /// <summary>
        /// Generate short guid code
        /// </summary>
        /// <returns>string</returns>
        public static string GenerateShortGuidCode()
        {
            return Shorter(Convert.ToBase64String(Guid.NewGuid().ToByteArray()));
            static string Shorter(string base64String)
            {
                base64String = base64String.Split('=')[0];
                base64String = base64String.Replace('+', '_');
                base64String = base64String.Replace('/', '_');
                return base64String;
            }
        }

        /// <summary>
        /// Get school year
        /// </summary>
        /// <returns>int</returns>
        public static int GetSchoolYear()
        {
            var schoolYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            if (currentMonth <= 6)
            {
                schoolYear--;
            }
            return schoolYear;
        }



        /// <summary>
        /// Hàm loại bỏ các ký tự unicode sang các kí tự thường.
        /// </summary>
        /// <param name="value">Chuỗi unicode</param>
        /// <returns>string</returns>
        public static string RemoveUnicode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            value = Regex.Replace(value, @"\s+", " ", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            string nfd = value.Normalize(NormalizationForm.FormD);
            StringBuilder retval = new(nfd.Length);
            foreach (char ch in nfd)
            {
                if (ch >= '\u0300' && ch <= '\u036f')
                {
                    continue;
                }

                if (ch >= '\u1dc0' && ch <= '\u1de6')
                {
                    continue;
                }

                if (ch >= '\ufe20' && ch <= '\ufe26')
                {
                    continue;
                }

                if (ch >= '\u20d0' && ch <= '\u20f0')
                {
                    continue;
                }

                retval.Append(ch);
            }
            return retval.ToString();
        }


        /// <summary>
        /// Mã hóa url enetviet
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        public static string EncodeUrlEnv(string sData)
        {
            string sBase64 = Base64Encode(sData);
            string characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int charactersLength = characters.Length;
            string randomString1 = "";
            Random random = new Random();
            for (int i = 0; i < 9; i++)
            {
                randomString1 += characters[random.Next(0, characters.Length - 1)];
            }
            string randomString2 = "";
            for (int i = 0; i < 9; i++)
            {
                randomString2 += characters[random.Next(0, characters.Length - 1)];
            }
            string randomString3 = "";
            for (int i = 0; i < 9; i++)
            {
                randomString3 += characters[random.Next(0, characters.Length - 1)];
            }
            string s1 = sBase64.Substring(0, 8);
            string s2 = sBase64.Substring(8, 8);
            string s3 = sBase64.Substring(16, 6);
            string s4 = sBase64.Substring(22, sBase64.Length - 22);
            string kq = s1 + randomString1 + s2 + randomString2 + s3 + randomString3 + s4;
            return kq.Replace(base64Character62, base64UrlCharacter62).Replace(base64Character63, _base64UrlCharacter63);
        }

        /// <summary>
        /// Base64 encode
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Giải mã url enetviet
        /// </summary>
        /// <param name="strEncrypt"></param>
        /// <returns></returns>
        public static string DecodeUrlEnv(string strEncrypt)
        {
            string strDecode = string.Empty;

            try
            {
                string s1 = "";
                string s2 = "";
                string s3 = "";
                for (int i = 0; i < strEncrypt.Length; i++)
                {
                    if (i >= 8 && i <= 16)
                    {
                        s1 += strEncrypt[i];
                    }
                    if (i >= 25 && i <= 33)
                    {
                        s2 += strEncrypt[i];
                    }
                    if (i >= 40 && i <= 48)
                    {
                        s3 += strEncrypt[i];
                    }
                }
                strDecode = strEncrypt.Replace(s1, "").Replace(s2, "").Replace(s3, "");
                var decode = System.Convert.FromBase64String(strDecode);
                strDecode = System.Text.Encoding.UTF8.GetString(decode);
            }
            catch
            {

            }
            return strDecode;
        }

        /// <summary>
        /// Trả về param sau khi giải mã url
        /// </summary>
        /// <param name="decodeParam"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ReturnParam(string[] decodeParam, string key)
        {
            string kq = "";
            foreach (var item in decodeParam)
            {
                if (item.Contains(key + "="))
                {
                    kq = item.Replace(key + "=", "");
                }
            }
            return kq;
        }

        /// <summary>
        /// Convert mã cấp học env gửi sang thành mã cấp học csdl
        /// </summary>
        /// <param name="ma_cap_hoc_env"></param>
        /// <returns></returns>
        public static string ConvertSchoolCodeEnv(string ma_cap_hoc_env)
        {
            switch (ma_cap_hoc_env)
            {
                case "0":
                    return "01";
                case "1":
                    return "02";
                case "2":
                    return "03";
                case "3":
                    return "04";
                case "4":
                    return "05";
                default:
                    return "01";
            }
        }
        /// <summary>
        /// Mã hóa url tra cứu monitor
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        public static string EncodeUrl(string sData)
        {
            string sBase64 = Base64Encode(sData);
            string characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int charactersLength = characters.Length;
            string randomString1 = "";
            Random random = new Random();
            for (int i = 0; i < 9; i++)
            {
                randomString1 += characters[random.Next(0, characters.Length - 1)];
            }
            string randomString2 = "";
            for (int i = 0; i < 9; i++)
            {
                randomString2 += characters[random.Next(0, characters.Length - 1)];
            }
            string randomString3 = "";
            for (int i = 0; i < 9; i++)
            {
                randomString3 += characters[random.Next(0, characters.Length - 1)];
            }
            string s1 = sBase64.Substring(0, 8);
            string s2 = sBase64.Substring(8, 8);
            string s3 = sBase64.Substring(16, 6);
            string s4 = sBase64.Substring(22, sBase64.Length - 22);
            string kq = s1 + randomString1 + s2 + randomString2 + s3 + randomString3 + s4;
            return kq.Replace(base64Character62, base64UrlCharacter62).Replace(base64Character63, _base64UrlCharacter63);
        }
        /// <summary>
        /// Giải mã url tra cứu monitor
        /// </summary>
        /// <param name="strEncrypt"></param>
        /// <returns></returns>
        public static string DecodeUrl(string strEncrypt)
        {
            strEncrypt = strEncrypt.Replace(base64UrlCharacter62, base64Character62).Replace(_base64UrlCharacter63, base64Character63);
            string strDecode = string.Empty;
            string s1 = "";
            string s2 = "";
            string s3 = "";
            for (int i = 0; i < strEncrypt.Length; i++)
            {
                if (i >= 8 && i <= 16)
                {
                    s1 += strEncrypt[i];
                }
                if (i >= 25 && i <= 33)
                {
                    s2 += strEncrypt[i];
                }
                if (i >= 40 && i <= 48)
                {
                    s3 += strEncrypt[i];
                }
            }
            try
            {
                strDecode = strEncrypt.Replace(s1, "").Replace(s2, "").Replace(s3, "");
            }
            catch
            {

            }

            var decode = System.Convert.FromBase64String(strDecode);
            strDecode = System.Text.Encoding.UTF8.GetString(decode);

            return strDecode;
        }
    }
}
