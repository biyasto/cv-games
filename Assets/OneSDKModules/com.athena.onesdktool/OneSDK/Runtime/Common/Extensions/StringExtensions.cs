
using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static System.String;

namespace OneSDK
{
    public static class StringExtensions
    {
        public static string RemoveWhitespaces(this string target) =>
            Regex.Replace(target, @"\s+", "");

        public static string ConvertWhitespacesToSingleSpaces(this string target) =>
            Regex.Replace(target, @"\s+", " ");

        
        public static string ReverseSlash(this string target, int direction)
        {
            switch (direction)
            {
                case 0:
                    return target.Replace(@"/", @"\");
                case 1:
                    return target.Replace(@"\", @"/");
                default:
                    return target;
            }
        }

        
        public static string LeftOf(this string target, char c)
        {
            int ndx = target.IndexOf(c);
            return ndx >= 0
                ? target.Substring(0, ndx)
                : target;
        }

        public static string RightOf(this string target, char c)
        {
            int ndx = target.IndexOf(c);
            return ndx == -1
                ? target
                : target.Substring(ndx + 1);
        }

        
        public static string RemoveLastCharacter(this string target) =>
            target.Length > 0 ? target.Substring(0, target.Length - 1) : target;

        
        public static string RemoveLast(this string target, int numberOfCharactersToRemove) =>
            target.IsNullOrEmpty() ? string.Empty : target.Substring(0, target.Length - numberOfCharactersToRemove);

        
        public static string RemoveFirstCharacter(this string target) =>
            target.Substring(1);

        public static string RemoveFirst(this string target, int numberOfCharactersToRemove) =>
            target.Substring(numberOfCharactersToRemove);

        
        public static string RemoveAllSpecialCharacters(this string target)
        {
            var sb = new StringBuilder(target.Length);
            foreach (char c in target.Where(char.IsLetterOrDigit)) sb.Append(c);
            return sb.ToString();
        }

        
        public static string RemoveAllEmptyLines(this string target) =>
            Regex.Replace(target, @"^\s*$\n|\r", Empty, RegexOptions.Multiline).TrimEnd();

        
        public static string ReplaceLineFeeds(this string target) =>
            Regex.Replace(target, @"^[\r\n]+|\.|[\r\n]+$", "");

        
        public static bool IsNull(this string target) =>
            target == null;

        
        public static bool IsNullOrEmpty(this string target) =>
            string.IsNullOrEmpty(target);

        
        public static bool IsMinLength(this string target, int minCharLength) =>
            target != null &&
            target.Length >= minCharLength;

        
        public static bool IsMaxLength(this string target, int maxCharLength) =>
            target != null &&
            target.Length <= maxCharLength;

        
        public static bool IsLength(this string target, int minCharLength, int maxCharLength) =>
            target != null &&
            target.Length >= minCharLength &&
            target.Length <= minCharLength;

        
        public static int? GetLength(string target) =>
            target?.Length;


        
        public static string Left(this string target, int length) =>
            String.IsNullOrEmpty(target)
                ? throw new ArgumentNullException(nameof(target))
                : length < 0 || length > target.Length
                    ? throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be higher than total string length or less than 0")
                    : target.Substring(0, length);

        
        public static string Right(this string target, int length) =>
            String.IsNullOrEmpty(target)
                ? throw new ArgumentNullException(nameof(target))
                : length < 0 || length > target.Length
                    ? throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be higher than total string length or less than 0")
                    : target.Substring(target.Length - length);


        
        public static bool DoesNotStartWith(this string target, string prefix) =>
            target == null ||
            prefix == null ||
            !target.StartsWith(prefix, StringComparison.InvariantCulture);

        
        public static bool DoesNotEndWith(this string target, string suffix) =>
            target == null ||
            suffix == null ||
            !target.EndsWith(suffix, StringComparison.InvariantCulture);

        
        public static string RemovePrefix(this string target, string prefix, bool ignoreCase = true) =>
            !String.IsNullOrEmpty(target) && (ignoreCase
                ? target.StartsWithIgnoreCase(prefix)
                : target.StartsWith(prefix))
                ? target.Substring(prefix.Length, target.Length - prefix.Length)
                : target;

        
        public static string RemoveSuffix(this string target, string suffix, bool ignoreCase = true) =>
            !String.IsNullOrEmpty(target) && (ignoreCase
                ? target.EndsWithIgnoreCase(suffix)
                : target.EndsWith(suffix))
                ? target.Substring(0, target.Length - suffix.Length)
                : Empty;

        
        public static string AppendSuffixIfMissing(this string target, string suffix, bool ignoreCase = true) =>
            String.IsNullOrEmpty(target) || (ignoreCase
                ? target.EndsWithIgnoreCase(suffix)
                : target.EndsWith(suffix))
                ? target
                : target + suffix;

        
        public static string AppendPrefixIfMissing(this string target, string prefix, bool ignoreCase = true) =>
            String.IsNullOrEmpty(target) || (ignoreCase
                ? target.StartsWithIgnoreCase(prefix)
                : target.StartsWith(prefix))
                ? target
                : prefix + target;

        
        public static string Capitalize(this string target) =>
            target.Length == 0 ? target : target.Substring(0, 1).ToUpper() + target.Substring(1).ToLower();

       
        public static string FirstCharacter(this string target) =>
            !String.IsNullOrEmpty(target)
                ? target.Length >= 1
                    ? target.Substring(0, 1)
                    : target
                : null;

        
        public static string LastCharacter(this string target) =>
            !String.IsNullOrEmpty(target)
                ? target.Length >= 1
                    ? target.Substring(target.Length - 1, 1)
                    : target
                : null;

        
        public static bool EndsWithIgnoreCase(this string target, string suffix) =>
            target == null
                ? throw new ArgumentNullException(nameof(target), "Target parameter is null")
                : suffix == null
                    ? throw new ArgumentNullException(nameof(suffix), "Suffix parameter is null")
                    : target.Length >= suffix.Length && target.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase);

        
        public static bool StartsWithIgnoreCase(this string target, string prefix) =>
            target == null
                ? throw new ArgumentNullException(nameof(target), "Target parameter is null")
                : prefix == null
                    ? throw new ArgumentNullException(nameof(prefix), "Prefix parameter is null")
                    : target.Length >= prefix.Length && target.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase);

        
        public static string Replace(this string target, params char[] chars) =>
            chars.Aggregate(target, (current, c) => current.Replace(c.ToString(CultureInfo.InvariantCulture), ""));

        
        public static string RemoveChars(this string target, params char[] chars)
        {
            var sb = new StringBuilder(target.Length);
            foreach (char c in target.Where(c => !chars.Contains(c))) sb.Append(c);
            return sb.ToString();
        }

        
        public static bool IsEmailAddress(this string target)
        {
            const string pattern = "^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$";
            return Regex.Match(target, pattern).Success;
        }

                public static string Reverse(this string target)
        {
            char[] chars = new char[target.Length];
            for (int i = target.Length - 1, j = 0; i >= 0; --i, ++j) chars[j] = target[i];
            target = new string(chars);
            return target;
        }

        
        public static int CountOccurrences(this string target, string stringToMatch) =>
            Regex.Matches(target, stringToMatch, RegexOptions.IgnoreCase).Count;


       
        public static bool IsAlpha(this string target) =>
            !String.IsNullOrEmpty(target) && target.Trim()
                .Replace(" ", "")
                .All(char.IsLetter);

        
        public static bool IsAlphaNumeric(this string target) =>
            !String.IsNullOrEmpty(target) && target.Trim()
                .Replace(" ", "")
                .All(char.IsLetterOrDigit);

        
        public static string Encrypt(this string target, string key)
        {
            var cspParameter = new CspParameters { KeyContainerName = key };
            var rsaServiceProvider = new RSACryptoServiceProvider(cspParameter) { PersistKeyInCsp = true };
            byte[] bytes = rsaServiceProvider.Encrypt(Encoding.UTF8.GetBytes(target), true);
            return BitConverter.ToString(bytes);
        }


        
        public static string Decrypt(this string target, string key)
        {
            var cspParameters = new CspParameters { KeyContainerName = key };
            var rsaServiceProvider = new RSACryptoServiceProvider(cspParameters) { PersistKeyInCsp = true };
            string[] decryptArray = target.Split(new[] { "-" }, StringSplitOptions.None);
            byte[] decryptByteArray = Array.ConvertAll(decryptArray, s => Convert.ToByte(byte.Parse(s, NumberStyles.HexNumber)));
            byte[] bytes = rsaServiceProvider.Decrypt(decryptByteArray, true);
            string result = Encoding.UTF8.GetString(bytes);
            return result;
        }

        
        public static int GetByteSize(this string target, Encoding encoding) =>
            target == null
                ? throw new ArgumentNullException(nameof(target))
                : encoding == null
                    ? throw new ArgumentNullException(nameof(encoding))
                    : encoding.GetByteCount(target);
    }
}
