using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LogReader.Expressions;
using LogReader.Models;

namespace LogReader
{

    public class ResetMachine
    {
        public string ResetKey { get; set; }
        public string PauseKey { get; set; }
        public string WindowName { get; set; }
        public List<ResetExpr> ItemExprs { get; set; } = new List<ResetExpr>();
        public bool Complete { get; private set; }
        public bool Running { get; private set; }
        public int ResetCount { get; private set; }

        public event EventHandler<EventArgs> OnComplete;
        public event EventHandler<EventArgs> OnReset;

        List<string> items = new List<string>();
        bool levelCreating = false;
        LReader reader;

        ExprReader exprReader = new ExprReader();

        public void OnLevelCreating()
        {
            if (Complete)
                return;
            levelCreating = true;
            items.Clear();
        }

        public void OnLevelCreated()
        {
            if (Complete || !levelCreating)
                return;

            levelCreating = false;

            bool setFound = false;
            foreach (var item in ItemExprs)
            {
                exprReader.ParseExpr(item.Expression);
                if (exprReader.EvaluateItems(items))
                {
                    setFound = true;
                    break;
                }
            }

            if (setFound)
            {
                Complete = true;
                Running = false;
                if (this.reader != null)
                    reader.OnLogEvent -= OnLogEvent;
                WindowHook.SendKeystroke(PauseKey, WindowName);
                OnComplete?.Invoke(this, new EventArgs());
            }
            else
            {
                ++ResetCount;
                OnReset?.Invoke(this, new EventArgs());
                WindowHook.SendKeystroke(ResetKey, WindowName);
            }
        }

        public void Start(LReader reader)
        {
            ResetCount = 0;
            Complete = false;
            Running = true;
            // Make sure we don't have hanging events
            if (this.reader != null)
                this.reader.OnLogEvent -= OnLogEvent;

            this.reader = reader;
            this.reader.OnLogEvent += OnLogEvent;
        }

        public void Stop()
        {
            Running = false;
        }

        void OnLogEvent(object sender, NecroLogs.OnLogEventArgs e)
        {
            if (Complete || !Running)
                return;

            if (e.Line.Text.StartsWith("NEWLEVEL:"))
            {
                OnLevelCreating();
            }

            if (Regex.IsMatch(e.Line.Text, "CREATEMAP ZONE\\d+: Finished!") || e.Line.Text.StartsWith("Level generation completed"))
            {
                OnLevelCreated();
            }

            if (levelCreating && e.Line.Text.StartsWith("ITEM NEW:"))
            {
                var match = Regex.Match(e.Line.Text, "itemType: (\\w+)");
                if (match.Success)
                {
                    items.Add(match.Groups[1].Value);
                }
            }
        }
    }
}
