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
        // pollingServerId: <period, duration>
        private Dictionary<int, Tuple<IntVar, IntVar>> pollingServers;
        public AllTaskInstancesWithVars()
        {
            ttDict = new Dictionary<TaskInstance, Tuple<IntVar, IntVar, IntervalVar>>();
            pollingServers = new Dictionary<int, Tuple<IntVar, IntVar>>();
        }

        public void AddTT(TaskInstance task, IntVar start, IntVar end, IntervalVar interval)
        {
            ttDict.Add(task, Tuple.Create(start, end, interval));
        }

        public void AddPollingServer(int pollingServerId, IntVar duration, IntVar period)
        {
            pollingServers.Add(pollingServerId, Tuple.Create(duration, period));
        }

        public Dictionary<TaskInstance, Tuple<IntVar, IntVar, IntervalVar>> GetTTVars()
        {
            return ttDict;
        }

        public Dictionary<int, Tuple<IntVar, IntVar>> GetETVars()
        {
            return pollingServers;
        }

    }
}
