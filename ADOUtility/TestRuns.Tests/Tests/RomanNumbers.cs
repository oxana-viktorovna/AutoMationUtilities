using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRuns.Tests.Tests
{
    using System.Text;

    public class RomanNumerals
    {
        public RomanNumerals()
        {
            numbersMapper = new Dictionary<int, string>();
            SetNumsMapper(1, 'I', 'V', 'X');
            SetNumsMapper(10, 'X', 'L', 'C');
            SetNumsMapper(100, 'C', 'D', 'M');
            numbersMapper.Add(1000, "M");
            numbersMapper.Add(2000, "MM");
            numbersMapper.Add(3000, "MMM");
        }

        private static Dictionary<int, string> numbersMapper;

        public static string ToRoman(int n)
        {
            var romanNum = new StringBuilder();

            var thousands = n / 1000;
            var leftover = (n % 1000);
            var thousandsNum = numbersMapper[thousands*1000];
            romanNum.Append(thousandsNum);

            var hundreds = leftover / 100;
            leftover = leftover % 100;
            var hundredsNum = numbersMapper[hundreds * 100];
            romanNum.Append(hundredsNum);

            var dozens = leftover / 10;
            leftover = leftover % 10;
            var dozensNum = numbersMapper[dozens * 10];
            romanNum.Append(dozensNum);

            var smallNum = numbersMapper[leftover];
            romanNum.Append(smallNum);

            return romanNum.ToString();
        }

        public static int FromRoman(string romanNumeral)
        {
            int num = 0;
            for (int i = numbersMapper.Count-1; i < 0; i--)
            {
                var map = numbersMapper.ElementAt(i);
                if (romanNumeral.Contains(map.Value))
                num += map.Key;
            }

            return num;
        }

        private static string GetRomanNum(int input, char lowerChar, char middleChar, char upperChar)
        {
            var outputNum = string.Empty;
            switch (input)
            {
                case int n when n < 4:
                    outputNum = new string(lowerChar, input);
                    break;
                case int n when n == 4:
                    outputNum = lowerChar.ToString() + middleChar.ToString();
                    break;
                case int n when n == 5:
                    outputNum = middleChar.ToString();
                    break;
                case int n when n > 5 && n < 9:
                    outputNum = middleChar + new string(lowerChar, input - 5);
                    break;
                case int n when n == 9:
                    outputNum = lowerChar.ToString() + upperChar.ToString();
                    break;
            }

            return outputNum;
        }

        private static void SetNumsMapper(int step, char lowerChar, char middleChar, char upperChar)
        {
            for (int i = 1*step; i < 9*step; i+= step)
            {
                var romanNum = GetRomanNum(i/step, lowerChar, middleChar, upperChar);
                numbersMapper.Add(i, romanNum);
            }
        }
    }
}

