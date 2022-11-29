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
                
                // the period of a server is the GCD value of its ET tasks minimal separation periods
                int period = AuxiliaryHelper.GetGCD(tasksFilteredBySeparation.Select(t => t.Period).ToArray());

                // the worst-case duration of a server is all its tasks being released all together i.e. the sum of all their durations
                int duration = tasksFilteredBySeparation.Select(t => t.Duration).Sum()/10;

                // ?? the worst-case deadline of a server is the minimal deadline from its tasks
                int deadline = tasksFilteredBySeparation.Select(t => t.Deadline).Min();

                pollingServers.Add(new TaskDefinition($"PollingServer{i}", duration, period, false, 0, deadline, i, tasksFilteredBySeparation.ToList()));
            }
            
            return pollingServers;
        }
        
        //public static List<PollingServerSlot> GetETPollingServersSlots(Schedule schedule)
        //{
        //    List<PollingServerSlot> slots = new();

        //    // temps
        //    int? currentServerId = null;
        //    int? slotStartTime = null;
        //    int? slotEndTime = null;

        //    List<TaskInstance> executedTasks = null;


        //    //TODO: extend the slots from the finish of the preceeding TT to the start of the following TT
        //    foreach ((var taskStartTime, var task) in schedule.Get())
        //    {
        //        if (currentServerId.HasValue)
        //        {
        //            // ET followed by TT
        //            if (!task.TaskDefinition.IsET)
        //            {
        //                //Console.WriteLine($"[{slotStartTime}, {slotEndTime}) - a slot for the server {currentServerId} serving {count} ET tasks");
        //                slots.Add(new PollingServerSlot(currentServerId.Value, slotStartTime.Value, slotEndTime.Value, executedTasks));
        //                currentServerId = null;
        //                slotStartTime = null;
        //                executedTasks = new List<TaskInstance>();
        //            }

        //            // ET followed by ET with matching separation
        //            else if (task.TaskDefinition.Separation == currentServerId)
        //            {
        //                executedTasks.Add(task);
        //                slotEndTime = taskStartTime + task.TaskDefinition.Duration;
        //            }

        //            // ET followed by ET with non-matching separation
        //            else
        //            {
        //                //Console.WriteLine($"[{slotStartTime}, {slotEndTime}) - a slot for the server {currentServerId} serving {count} ET tasks");
        //                slots.Add(new PollingServerSlot(currentServerId.Value, slotStartTime.Value, slotEndTime.Value, executedTasks));

        //                currentServerId = task.TaskDefinition.Separation;
        //                slotStartTime = taskStartTime;
        //                slotEndTime = taskStartTime + task.TaskDefinition.Duration;
        //                executedTasks = new List<TaskInstance> { task };
        //            }
        //        }
        //        else
        //        {
        //            // TT followed by ET
        //            if (task.TaskDefinition.IsET)
        //            {
        //                currentServerId = task.TaskDefinition.Separation;
        //                slotStartTime = taskStartTime;
        //                slotEndTime = taskStartTime + task.TaskDefinition.Duration;
        //                executedTasks = new List<TaskInstance> { task };
        //            }

        //            // TT followed by TT - do nothing
        //        }
        //    }

        //    // last item 
        //    if (currentServerId.HasValue)
        //    {
        //        //Console.WriteLine($"[{slotStartTime}, {slotEndTime}) - a slot for the server {currentServerId} serving {count} ET tasks");
        //        slots.Add(new PollingServerSlot(currentServerId.Value, slotStartTime.Value, slotEndTime.Value, executedTasks));
        //    }

        //    return slots;
        //}

        public static (bool, int) getSchedulabilityofET(List<PollingServerSlot> pollingServerSlots, List<Tuple<int,TaskInstance>> ETtaskSchedule, int fullPeriod)
        {
            int responseTime = 0;

            foreach ((_,var task) in ETtaskSchedule)
            {
                int t = 0;
                responseTime = task.AbsoluteDeadline + 1;

                while (t < fullPeriod)
                {
                    int supply = getSupplyAtTick(pollingServerSlots, t);
                    int demand = 0;

                    foreach ((_, var higherPriorityTask) in ETtaskSchedule.Where(t => t.Item2.TaskDefinition.Priority > task.TaskDefinition.Priority)) 
                    {
                        demand += (int) Math.Ceiling((double) t / (double) higherPriorityTask.TaskDefinition.Period) * higherPriorityTask.TaskDefinition.Duration;
                    }

                    if (supply > demand)
                    {
                        responseTime = t;
                        break;
                    }

                    t++;
                }

                if (responseTime > task.AbsoluteDeadline)
                {
                    return (false, responseTime);
                }
            }

            return (true, responseTime);
        }

        private static int getSupplyAtTick(List<PollingServerSlot> pollingServerSlots, int tick)
        {
            int supply = 0;
            
            foreach (PollingServerSlot pollingServerSlot in pollingServerSlots)
            {
                if (pollingServerSlot.EndTime < tick)
                {
                    supply += pollingServerSlot.EndTime - pollingServerSlot.StartTime;
                }
                else if (pollingServerSlot.StartTime < tick)
                {
                    supply += tick - pollingServerSlot.StartTime;
                }
                else
                {
                    break;
                }
            }

            return supply;
        }
        public static (bool, int) GetResponseTimesForPollingServes(Tuple<IntVar, IntVar> optimisationVars, IEnumerable<TaskDefinition> tasks, int fullPeriod)
        {
            (IntVar periodVar, IntVar durationVar) = optimisationVars;

            LinearExpr delta = 2 * (periodVar - durationVar);

            int responseTime = 0;
            foreach (var task in tasks)
            {
                int t = 0;
                responseTime = task.Deadline + 1;

                while (t <= fullPeriod)
                {
                    LinearExpr supply = 1 * (t - delta);
                    int demand = 0;
                    foreach (var task2 in tasks.Where(t => t.Priority >= task.Priority))
                    {
                        demand += (int) Math.Ceiling((t / (double)task2.Period)) * task2.Duration;
                    }
                    if (supply >= demand * periodVar)
                    {
                        responseTime = t;
                        break;
                    }
                    t++;
                }
                if (responseTime > task.Deadline)
                {
                    return (false, responseTime);
                }
            }

            return (true, responseTime);
        }
    }
}

// Next step is to use the Alg 2 for getting the score for ET
// In the report - provide worst response time for  TT(and ET) and seperatly the result of Alg2 for ET
// Here, instead of using alpha/delta, implement my own supply function supply(t) that is obtained from the ET table.
// Then next step is to add supply(t, serverId) 
