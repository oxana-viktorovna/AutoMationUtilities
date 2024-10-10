using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SharedCore.StringUtilities
{
    public static class StringParser
    {
        public static string GetTestCaseNumber(this string testMethod)
        {
            var regex = new Regex(@"T\d+_");
            var number = regex.Match(testMethod).Groups[0].Value.Replace("T", "").Replace("_", "");

            return number;
        }

        public static int GetTcNumber(this string testMethod)
        {
            var regex = new Regex(@"T\d+_");
            var number = regex.Match(testMethod).Groups[0].Value.Replace("T", "").Replace("_", "");

            return Convert.ToInt32(number);
        }

        public static string GetTestMethodName(this string testMethod)
            => Regex.Replace(testMethod,@"([TP]\d*_)+","");

        public static string RemoveTextFromTo(string input, string from, string to)
        {
            StringBuilder result = new StringBuilder(input);
            int startIndex = result.ToString().IndexOf(from, StringComparison.Ordinal);

            while (startIndex != -1)
            {
                int endIndex = result.ToString().IndexOf(to, startIndex, StringComparison.Ordinal);
                if (endIndex == -1)
                    break;

                endIndex += to.Length; // move past the "False"

                int lengthToRemove = endIndex - startIndex;
                result.Remove(startIndex, lengthToRemove);

                startIndex = result.ToString().IndexOf(from, StringComparison.Ordinal);
            }

            return result.ToString();
        }
    }
}
