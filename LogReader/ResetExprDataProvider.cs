using LogReader.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReader
{
    public class ResetExprDataProvider
    {
        ObservableCollection<ResetExpr> resetExprsObs;
        public ResetExprDataProvider()
        {
            resetExprsObs = new ObservableCollection<ResetExpr>(Properties.Settings.Default.ResetExpressions);
        }

        public ObservableCollection<ResetExpr> GetExpressions()
        {
            return resetExprsObs;
        }
    }
}
