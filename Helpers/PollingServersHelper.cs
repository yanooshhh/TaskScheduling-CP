using CPplayground.Entities;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPplayground.Helpers
{
    internal class PollingServersHelper
    {
        public static List<TaskDefinition> GetPollingServersDefinitionsFromETs(List<TaskDefinition> etTasks)
        {
            List<TaskDefinition> pollingServers = new();

            for (int i = 0; i <= 3; i++)
            {
                var tasksFilteredBySeparation = etTasks.Where(t => t.Separation == i);

                // The period of a server is the GCD value of its ET tasks minimal separation periods
                int period = AuxiliaryHelper.GetGCD(tasksFilteredBySeparation.Select(t => t.Period).ToArray());

                // Full utilisation of the server
                int deadline = period;

                // Computing duration
                double totalUntilization = 0;
                foreach (var task in tasksFilteredBySeparation)
                    totalUntilization += (double) task.Duration / (double) task.Period;

                int duration = (int) (totalUntilization * period);

                pollingServers.Add(new TaskDefinition($"PollingServer{i}", duration, period, false, 0, deadline, i, tasksFilteredBySeparation.ToList()));
            }

            return pollingServers;
        }
    }  
}
