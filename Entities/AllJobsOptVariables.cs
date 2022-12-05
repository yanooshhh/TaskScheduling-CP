using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPplayground.Entities
{
    internal class AllJobsOptVariables
    {
        // ttTask: <start,interval>
        private Dictionary<Job, IntervalVar> optVarsDict;
        public AllJobsOptVariables()
        {
            optVarsDict = new Dictionary<Job, IntervalVar>();
        }

        public void Add(Job task, IntervalVar interval)
        {
            optVarsDict.Add(task, interval);
        }

        public Dictionary<Job, IntervalVar> GetOptVars()
        {
            return optVarsDict;
        }

    }
}
