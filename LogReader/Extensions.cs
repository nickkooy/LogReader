using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LogReader
{
    public static class Extensions
    {
        /// <summary>
        /// Reads the line and includes the eol characters "\r\n"
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static string ReadLine2(this StreamReader reader)
        {
            if (!reader.BaseStream.CanRead || reader.EndOfStream)
                return null;

            string line = "";
            while (!reader.EndOfStream)
            {
                char c = (char)reader.Read();
                line += c;
                if (c == '\r')
                {
                    char nextChar = (char)reader.Peek();
                    if (nextChar == '\n')
                    {
                        reader.Read();
                        line += nextChar;
                        break;
                    }
                }
            }

            return line;
        }
    }
}
