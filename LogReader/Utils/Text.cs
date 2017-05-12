using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReader.Utils
{
    public static class Text
    {
        public static string UnEscapeStr(string str)
        {
            if (str == null)
                return "";

            string rval = string.Copy(str);
            for (int i = 0; i < rval.Length - 1; ++i)
            {
                if (rval[i] == '\\')
                {
                    string rplc = "";
                    char c = rval[i + 1];
                    int rCount = 2;

                    switch (c)
                    {
                        case 'r': rplc = "\r"; break;
                        case 'n': rplc = "\n"; break;
                        case 't': rplc = "\t"; break;
                        case '\\':
                            rplc = "\\";
                            ++i; // skip that one
                            break;
                        default:
                            rCount = 0;
                            break;
                    }

                    rval = rval.Remove(i, rCount);
                    rval = rval.Insert(i, rplc);
                }
            }
            return rval;
        }
    }
}
