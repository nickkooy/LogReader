using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReader
{
    public class NDLogInfo
    {
        const string LogTitlePreface = "necrodancer_log_";
        const string LogExtension = ".txt";

        public DateTime CreateTime { get; set; }
        public string FileName { get; set; }
        public string FullPath { get; set; }

        public NDLogInfo(string filepath)
        {
            FullPath = filepath;
            FileName = Path.GetFileName(filepath);
            CreateTime = GetLogDateFromName(FileName);
        }


        string GetLogDateStr(string filename)
        {
            if (filename.Length <= LogTitlePreface.Length + LogExtension.Length)
            {
                return filename;
            }
            string rval = filename.Substring(LogTitlePreface.Length, filename.Length - LogExtension.Length - LogTitlePreface.Length);
            return rval;
        }

        DateTime GetLogDateFromName(string filename)
        {
            string dStr = GetLogDateStr(filename);
            DateTime monthDT;
            if (dStr.Length < 21)
                return DateTime.MinValue;
            int year, day, hour, min, sec;
            bool bY, bM, bD, bH, bMin, bS;

            if (int.TryParse(dStr.Substring(0, 4), out year) &&
                DateTime.TryParseExact(dStr.Substring(5, 1) + dStr.Substring(6, 2).ToLower(), "MMM", new CultureInfo("en-us"), DateTimeStyles.None, out monthDT) &&
                int.TryParse(dStr.Substring(9, 2), out day) &&
                int.TryParse(dStr.Substring(12, 2), out hour) &&
                int.TryParse(dStr.Substring(15, 2), out min) &&
                int.TryParse(dStr.Substring(18, 2), out sec))
            {
                return new DateTime(year, monthDT.Month, day, hour, min, sec);
            }

            return DateTime.MinValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is NDLogInfo)
            {
                return this == (NDLogInfo)obj;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static int Compare(NDLogInfo x, NDLogInfo y)
        {
            if (x == null && y != null)
                return 1;
            else if (x != null && y == null)
                return -1;
            else if (x == null && y == null)
                return 0;
            else
                return DateTime.Compare(x.CreateTime, y.CreateTime);
        }
    }
}
