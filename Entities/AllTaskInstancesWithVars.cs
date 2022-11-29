using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPplayground.Entities
{
    internal class AllTaskInstancesWithVars
    {
        // ttTask: <start,end,interval>
        private Dictionary<TaskInstance, Tuple<IntVar, IntVar, IntervalVar>> ttDict;
        public AllTaskInstancesWithVars()
        {
            ttDict = new Dictionary<TaskInstance, Tuple<IntVar, IntVar, IntervalVar>>();
        }

        public void AddTT(TaskInstance task, IntVar start, IntVar end, IntervalVar interval)
        {
            ttDict.Add(task, Tuple.Create(start, end, interval));
        }

        public Dictionary<TaskInstance, Tuple<IntVar, IntVar, IntervalVar>> GetTTVars()
        {
            return ttDict;
        }

    }
}
