using CPplayground.Entities;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPplayground.Helpers
{
    internal class SchedulingHelper
    {
        public static List<TaskInstance> GetAllRepeatingTasks(List<TaskDefinition> taskDefinitions, int fullPeriod)
        {
            List<TaskInstance> taskInstances = new();
            
            foreach (TaskDefinition taskDefinition in taskDefinitions)
            {
                for (int tick = 0; tick < fullPeriod; tick += taskDefinition.Period)
                {
                    taskInstances.Add(new TaskInstance(taskDefinition, 
                                                       tick, 
                                                       tick + taskDefinition.Deadline));
                }
            }

            return taskInstances;
        }

        static public Schedule ConvertToSchedule(AllTaskInstancesWithVars allTasksVars, CpSolver solver)
        {
            var schedule = new Schedule();

            foreach (var taskWithVars in allTasksVars.GetTTVars())
            {
                (var task, (var start, _, _)) = taskWithVars;

                int optimalStart = (int)solver.Value(start);
                schedule.Add(optimalStart, task);
            }

            schedule.Sort();
            return schedule;
        }
    }
}