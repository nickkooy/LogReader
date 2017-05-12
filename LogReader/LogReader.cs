using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NecroLogs;

namespace LogReader
{
	public class LReader: IDisposable, ILogEvents
	{
        const string LogFolderName = "logs";
        const int ReadingWaitTime = 200;
        const int ReadLineWaitTime = 300;
        const int ReadStartDelay = 100;

        public event EventHandler<LogReaderErrorArgs> OnFileError;

		public LReader(string dir, int qSize = 5)
		{
			NecrodancerDir = dir;
            logQ = new Queue<string>(qSize);
        }
        public string NecrodancerDir
        {
            get { return ndDir; }
            set
            {
                if (ndDir != value)
                {
                    ndDir = value;
                    try
                    {
                        dirCount = Directory.Exists(ndDir) && Directory.Exists(NecrodancerLogDirectory) ? Directory.EnumerateFiles(NecrodancerLogDirectory).Count() : 0;
                    }
                    catch (Exception e)
                    {
                        OnFileError?.Invoke(this, new LogReaderErrorArgs("There was a problem reading the Necrodancer Directory." + Environment.NewLine + e.Message));
                    }
                    LastModified = GetLastModifiedLog();
                }
            }
        }

        public string NecrodancerLogDirectory
        {
            get { return ndDir == null ? null : Path.Combine(ndDir, LogFolderName); }
        }

        public NDLogInfo LastModified
        {
            get { return lastModified; }
            private set
            {

                lastModified = value;
                bool gcCollect = logStream != null;
                DisposeStreams();
                DisposeTimer();
                if (lastModified != null)
                {
                    try
                    {
                        logStream = File.Open(lastModified.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        reader = new StreamReader(logStream);
                    }
                    catch (Exception e)
                    {
                        OnFileError?.Invoke(this, new LogReaderErrorArgs($"Error opening log file: {lastModified.FullPath}{Environment.NewLine}{e.Message}"));
                        lastModified = null;
                        DisposeStreams();
                        DisposeTimer();
                        gcCollect = true;
                    }
                }

                if (gcCollect)
                    GC.Collect();
            }
        }

        int dirCount;
        Queue<string> logQ;
        string ndDir;
        NDLogInfo lastModified;
        Timer readTimer;
        FileStream logStream;
        StreamReader reader;
        ReaderState readState = new ReaderState();

        public event EventHandler<OnLogEventArgs> OnLogEvent;

        class ReaderState
        {
            public bool Reading { get; set; }
        }

        public void StartRead()
        {
            if (logStream == null)
            {
                // Maybe stop reading instead?
                StopRead();
                return;
            }

            //jump to end of the file
            logStream.Seek(0, SeekOrigin.End);
            while (!reader.EndOfStream)
                EnqueueLine(readState);
            DisposeTimer();
            readTimer = new Timer(new TimerCallback(EnqueueLine), readState, ReadStartDelay, ReadLineWaitTime);
        }

        public void StopRead()
        {
            DisposeTimer();
        }

        void EnqueueLine(object state)
        {
            ReaderState rState = (ReaderState)state;
            if (!rState.Reading)
            {
                rState.Reading = true;

                string line = null;
                do
                {
                    line = ReadLine();
                    if (line != null)
                    {
                        //logQ.Enqueue(line);
                        OnLogEvent?.Invoke(this, new OnLogEventArgs(new LogLine(line)));
                    }
                }
                while (line != null);
                rState.Reading = false;
            }
        }

        public string ReadLine()
        {
            if (!Directory.Exists(NecrodancerDir))
                return null;
            int dCount = Directory.EnumerateFiles(NecrodancerLogDirectory).Count();
            if (dCount != dirCount)
            {
                dirCount = dCount;
                NDLogInfo lastLog = GetLastModifiedLog();
                if (lastModified == null || lastModified != lastLog)
                {
                    LastModified = lastLog;
                }
            }

            if (lastModified == null || reader == null || reader.EndOfStream)
                return null;

            string line = reader.ReadLine2();
            // End of file so just return null
            if (line == null)
                return null;

            // Check that we actually got the end of the line
            while (line.Substring(line.Length - Environment.NewLine.Length) != Environment.NewLine)
            {
                // Wait a bit
                System.Threading.Thread.Sleep(ReadingWaitTime);
                // Append any new characters
                line += reader.ReadLine2();
            }
            
            // Return the line without /r/n
            return line.Substring(0, line.Length - Environment.NewLine.Length);
        }
        

        NDLogInfo GetLastModifiedLog()
        {
            // Make sure it exists
            if (!Directory.Exists(NecrodancerDir))
            {
                // Check that you can read it
                if (!Utils.Wnd.CanReadDirectory(NecrodancerDir))
                {
                    OnFileError?.Invoke(this, new LogReaderErrorArgs($"Can't read directory: {NecrodancerDir}{Environment.NewLine}Try running this as admin or changing the file permissions."));
                    return null;
                }
                OnFileError?.Invoke(this, new LogReaderErrorArgs($"Directory doesn't exist: {NecrodancerDir}"));
                return null;
            }

            if (!Directory.Exists(NecrodancerLogDirectory))
            {
                if (!Utils.Wnd.CanReadDirectory(NecrodancerLogDirectory))
                {
                    OnFileError?.Invoke(this, new LogReaderErrorArgs($"Can't read directory: {NecrodancerLogDirectory}{Environment.NewLine}Try running this as an admin or changing the file permissions."));
                    return null;
                }
                OnFileError?.Invoke(this, new LogReaderErrorArgs($"Log directory not found: {NecrodancerLogDirectory}"));
                return null;
            }
            
            NDLogInfo logInfo = null;
            foreach (string file in Directory.EnumerateFiles(NecrodancerLogDirectory))
            {
                NDLogInfo newLog = new NDLogInfo(file);
                if (logInfo == null || NDLogInfo.Compare(logInfo, newLog) < 0)
                {
                    logInfo = newLog;
                }
            }

            if (logInfo == null)
            {
                OnFileError?.Invoke(this, new LogReaderErrorArgs($"No logs found in: {NecrodancerLogDirectory}"));
            }

            return logInfo;
        }

        public void Dispose()
        {
            DisposeStreams();
        }

        void DisposeStreams()
        {

            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
                reader = null;
            }
            if (logStream != null)
            {
                logStream.Close();
                logStream.Dispose();
                logStream = null;
            }
        }

        void DisposeTimer()
        {
            if (readTimer != null)
            {
                readTimer.Dispose();
                readTimer = null;
            }
        }
    }
}
