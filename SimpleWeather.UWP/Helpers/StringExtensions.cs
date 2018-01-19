//--------------------------------------------------------------------------
//
//     Copyright (CPOL) 1.02 Design IT Right
//
//     THE WORK (AS DEFINED BELOW) IS PROVIDED UNDER THE TERMS OF THIS CODE 
//     PROJECT OPEN LICENSE ("LICENSE"). THE WORK IS PROTECTED BY COPYRIGHT 
//     AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS 
//     AUTHORIZED UNDER THIS LICENSE OR COPYRIGHT LAW IS PROHIBITED.
//
//     BY EXERCISING ANY RIGHTS TO THE WORK PROVIDED HEREIN, YOU ACCEPT 
//     AND AGREE TO BE BOUND BY THE TERMS OF THIS LICENSE. THE AUTHOR GRANTS 
//     YOU THE RIGHTS CONTAINED HEREIN IN CONSIDERATION OF YOUR ACCEPTANCE OF 
//     SUCH TERMS AND CONDITIONS. IF YOU DO NOT AGREE TO ACCEPT AND BE BOUND 
//     BY THE TERMS OF THIS LICENSE, YOU CANNOT MAKE ANY USE OF THE WORK.
//
//     Author: juwikuang
//     https://www.codeproject.com/Tips/1004964/Title-Case-in-VB-net-or-Csharp
//
//--------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.Helpers
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string s)
        {
            var upperCase = s.ToUpper();
            var words = upperCase.Split(' ');

            var minorWords = new String[] {"ON", "IN", "AT", "OFF", "WITH", "TO", "AS", "BY",//prepositions
                                   "THE", "A", "OTHER", "ANOTHER",//articles
                                   "AND", "BUT", "ALSO", "ELSE", "FOR", "IF"};//conjunctions

            var acronyms = new String[] {"UK", "USA", "US",//countries
                                   "BBC",//TV stations
                                   "TV"};//others

            //The first word.
            //The first letter of the first word is always capital.
            if (acronyms.Contains(words[0]))
            {
                words[0] = words[0].ToUpper();
            }
            else
            {
                words[0] = words[0].ToPascalCase();
            }

            //The rest words.
            for (int i = 0; i < words.Length; i++)
            {
                if (minorWords.Contains(words[i]))
                {
                    words[i] = words[i].ToLower();
                }
                else if (acronyms.Contains(words[i]))
                {
                    words[i] = words[i].ToUpper();
                }
                else
                {
                    words[i] = words[i].ToPascalCase();
                }
            }

            return string.Join(" ", words);

        }

        public static string ToPascalCase(this string s)
        {
            return s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower();
        }
    }
}
