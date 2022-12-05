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
        // ttTask: <start,end,interval>
        private Dictionary<Job, Tuple<IntVar, IntVar, IntervalVar>> ttDict;
        public AllJobsOptVariables()
        {
            ttDict = new Dictionary<Job, Tuple<IntVar, IntVar, IntervalVar>>();
        }

        public void AddTT(Job task, IntVar start, IntVar end, IntervalVar interval)
        {
            ttDict.Add(task, Tuple.Create(start, end, interval));
        }

        public Dictionary<Job, Tuple<IntVar, IntVar, IntervalVar>> GetTTVars()
        {
            return ttDict;
        }

    }
}
