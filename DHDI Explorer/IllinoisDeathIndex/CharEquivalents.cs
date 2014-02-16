using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genealogy
{
    class CharEquivalents
    {

        private class TransChar : Tuple<String, string, string>
        {
            public TransChar(String Native, string Web, string PlainText)
                : base(Native, Web, PlainText)
            {
            }

            public String Native { get { return this.Item1; } }
            public string Web { get { return this.Item2; } }
            public string PlainText { get { return this.Item3; } }
        }

        private static List<TransChar> lChars = new List<TransChar>(new TransChar[] {
                new TransChar( "´", "&#180;", "'" ),
                new TransChar( "Á", "&Aacute;", "A" ),
                new TransChar( "á", "&aacute;", "a" ),
                new TransChar( "Ä", "&Auml;", "A" ),
                new TransChar( "ä", "&auml;", "a" ),
                new TransChar( "Ą", "&#260;", "A" ),
                new TransChar( "ą", "&#261;", "a" ),
                new TransChar( "Â", "&Acirc;", "A" ),
                new TransChar( "â", "&acirc;", "a" ),
                new TransChar( "Ă", "&#258;", "A" ),
                new TransChar( "ă", "&#259;", "a" ),
                new TransChar( "Ć", "&#262;", "C" ),
                new TransChar( "ć", "&#263;", "c" ),
                new TransChar( "Č", "&#268;", "C" ),
                new TransChar( "č", "&#269;", "c" ),
                new TransChar( "Ç", "&Ccedil;", "C" ),
                new TransChar( "ç", "&ccedil;", "c" ),
                new TransChar( "Ď", "&#270;", "D" ),
                new TransChar( "ď", "&#271;", "d" ),
                new TransChar( "Đ", "&#272;", "D" ),
                new TransChar( "đ", "&#273;", "d" ),
                new TransChar( "É", "&Eacute;", "E" ),
                new TransChar( "é", "&eacute;", "e" ),
                new TransChar( "Ě", "&#282;", "E" ),
                new TransChar( "ě", "&#283;", "e" ),
                new TransChar( "Ę", "&#280;", "E" ),
                new TransChar( "ę", "&#281;", "e" ),
                new TransChar( "Ë", "&Euml;", "E" ),
                new TransChar( "ë", "&euml;", "e" ),
                new TransChar( "Í", "&Iacute;", "I" ),
                new TransChar( "í", "&iacute;", "i" ),
                new TransChar( "Î", "&Icirc;", "I" ),
                new TransChar( "î", "&icirc;", "i" ),
                new TransChar( "Ĺ", "&#313;", "L" ),
                new TransChar( "ĺ", "&#314;", "l" ),
                new TransChar( "Ľ", "&#317;", "L" ),
                new TransChar( "ľ", "&#318;", "l" ),
                new TransChar( "Ł", "&#321;", "L" ),
                new TransChar( "ł", "&#322;", "l" ),
                new TransChar( "Ń", "&#323;", "N" ),
                new TransChar( "ń", "&#324;", "n" ),
                new TransChar( "Ň", "&#327;", "N" ),
                new TransChar( "ň", "&#328;", "n" ),
                new TransChar( "Ó", "&Oacute;", "O" ),
                new TransChar( "ó", "&oacute;", "o" ),
                new TransChar( "Ô", "&Ocirc;", "O" ),
                new TransChar( "ô", "&ocirc;", "o" ),
                new TransChar( "Ö", "&Ouml;", "O" ),
                new TransChar( "ö", "&ouml;", "o" ),
                new TransChar( "Õ", "&Otilde;", "O" ),
                new TransChar( "õ", "&otilde;", "o" ),
                new TransChar( "Ő", "&#336;", "O" ),
                new TransChar( "ő", "&#337;", "o" ),
                new TransChar( "Ř", "&#344;", "R" ),
                new TransChar( "ř", "&#345;", "r" ),
                new TransChar( "Ŕ", "&#340;", "R" ),
                new TransChar( "ŕ", "&#341;", "r" ),
                new TransChar( "Ś", "&#346;", "S" ),
                new TransChar( "ś", "&#347;", "s" ),
                new TransChar( "Š", "&#352;", "S" ),
                new TransChar( "š", "&#353;", "s" ),
                new TransChar( "Ş", "&#350;", "S" ),
                new TransChar( "ş", "&#351;", "s" ),
                new TransChar( "Ť", "&#356;", "T" ),
                new TransChar( "ť", "&#357;", "t" ),
                new TransChar( "Ţ", "&#354;", "T" ),
                new TransChar( "ţ", "&#355;", "t" ),
                new TransChar( "Ú", "&Uacute;", "U" ),
                new TransChar( "ú", "&uacute;", "u" ),
                new TransChar( "Ü", "&Uuml;", "U" ),
                new TransChar( "ü", "&uuml;", "u" ),
                new TransChar( "Ů", "&#366;", "U" ),
                new TransChar( "ů", "&#367;", "u" ),
                new TransChar( "Ű", "&#360;", "U" ),
                new TransChar( "ű", "&#361;", "u" ),
                new TransChar( "Ý", "&Yacute;", "Y" ),
                new TransChar( "ý", "&yacute;", "y" ),
                new TransChar( "Ź", "&#377;", "Z" ),
                new TransChar( "ź", "&#378;", "z" ),
                new TransChar( "Ž", "&#381;", "Z" ),
                new TransChar( "ž", "&#382;", "z" ),
                new TransChar( "Ż", "&#379;", "Z" ),
                new TransChar( "ż", "&#380;", "z" )
        });

        private static Dictionary<String, TransChar> mDictionary = null;

        public static bool Transliterate( String sNative, out string WebReturn, out string PlainTextReturn )
        {
            if (mDictionary == null)
            {
                mDictionary = new Dictionary<String, TransChar>();
                foreach (TransChar tNext in lChars)
                {
                    mDictionary.Add(tNext.Native, tNext);
                }
            }

            bool bModified = false;
            string sWeb = string.Empty, sPlainText = string.Empty;
            for (int i = 0; i < sNative.Length; ++i)
            {
                String cNext = sNative.Substring(i, 1);
                TransChar tVal;
                if (mDictionary.TryGetValue(cNext, out tVal))
                {
                    bModified = true;
                    sWeb += tVal.Web;
                    sPlainText += tVal.PlainText;
                }
                else
                {
                    sWeb += cNext;
                    sPlainText += cNext;
                }
            }

            WebReturn = sWeb;
            PlainTextReturn = sPlainText;
            return bModified;
        }
    }
}
