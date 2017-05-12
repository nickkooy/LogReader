using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReader
{
    public class LogReaderErrorArgs : EventArgs
    {
        public string ErrorMessage { get; private set; }
        public LogReaderErrorArgs(string errormsg) { ErrorMessage = errormsg; }
    }
}
