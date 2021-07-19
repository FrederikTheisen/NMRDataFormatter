using System;
using System.Collections.Generic;

namespace MDTools
{
    public static class OneToThreeLetterConverter
    {
        static readonly Dictionary<char, string> Lib = new Dictionary<char, string>()
        {
            {'q',"gln" },
            {'w',"trp" },
            {'e',"glu" },
            {'r',"arg" },
            {'t',"thr" },
            {'y',"tyr" },
            {'i',"ile" },
            {'p',"pro" },
            {'a',"ala" },
            {'s',"ser" },
            {'d',"asp" },
            {'f',"phe" },
            {'g',"gly" },
            {'h',"his" },
            {'k',"lys" },
            {'l',"leu" },
            {'c',"cys" },
            {'v',"val" },
            {'n',"asn" },
            {'m',"met" }
        };

        public static string Convert(string letter)
        {
            if (!string.IsNullOrEmpty(letter))
            {
                if (letter.Length != 1)
                {
                    return letter;
                }
                else return Convert(letter.ToUpper()[0]);
            }

            return "***";
        }

        public static string Convert(char letter)
        {
            char aa = letter.ToString().ToLower()[0];

            if (!Lib.ContainsKey(aa)) return "***";
            else return Lib[aa].ToUpper();
        }
    }
}
