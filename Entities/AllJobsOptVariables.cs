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
        private Dictionary<Job, Tuple<IntVar, IntervalVar>> ttDict;
        public AllJobsOptVariables()
        {
            ttDict = new Dictionary<Job, Tuple<IntVar, IntervalVar>>();
        }

        public void AddTT(Job task, IntVar start, IntervalVar interval)
        {
            ttDict.Add(task, Tuple.Create(start, interval));
        }

        public Dictionary<Job, Tuple<IntVar, IntervalVar>> GetTTVars()
        {
            return ttDict;
        }

    }
}
