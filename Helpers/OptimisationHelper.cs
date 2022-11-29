using CPplayground.Entities;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPplayground.Helpers
{
    internal class OptimisationHelper
    {
        static readonly long DEADLINE_PENALTY = 100000000;
        static public LinearExpr GetLossExpression(AllTaskInstancesWithVars allTasksVars)
        {
            List<LinearExpr> allExpressions = new List<LinearExpr>();
            
            foreach (var taskVars in allTasksVars.GetTTVars())
            {
                var task = taskVars.Key;
                (var start, var end, _) = taskVars.Value;
                if (end > taskVars.Key.AbsoluteDeadline)
                {
                    allExpressions.Add(LinearExpr.Constant(DEADLINE_PENALTY));              
                } 
                else
                {
                    allExpressions.Add(end - task.Release);
                }
            }

            return LinearExpr.Sum(allExpressions);
        }

        static public LinearExpr GetNewLossExpression(AllTaskInstancesWithVars allTasksVars, List<TaskDefinition> etTaskDefinitions, int fullPeriod)
        {
            List<LinearExpr> allExpressions = new List<LinearExpr>();

            foreach (var taskVars in allTasksVars.GetTTVars())
            {
                var task = taskVars.Key;
                (var start, var end, _) = taskVars.Value;
                if (end > taskVars.Key.AbsoluteDeadline)
                {
                    allExpressions.Add(LinearExpr.Constant(DEADLINE_PENALTY));
                }
                else
                {
                    allExpressions.Add(end - task.Release);
                }
            }

            LinearExpr ttScore = LinearExpr.Sum(allExpressions);


            LinearExpr etScore = LinearExpr.Constant(0);
            for (int i = 0; i <= 3; i++)
            {
                var tasksPerServer = etTaskDefinitions.Where(t => t.Separation == i);
                (bool isSchedulable, int responseTime) = PollingServersHelper.GetResponseTimesForPollingServes(allTasksVars.GetETVars()[i], tasksPerServer, fullPeriod);
                if (!isSchedulable)
                    etScore += DEADLINE_PENALTY;
                else
                    etScore += responseTime;
            }

            return ttScore + etScore;
        }

        static public Schedule ConvertToSchedule(AllTaskInstancesWithVars allTasksVars, CpSolver solver)
        {
            var schedule = new Schedule();

            foreach (var taskWithVars in allTasksVars.GetTTVars())
            {
                (var task, (var start, _, _)) = taskWithVars;

                int optimalStart = (int) solver.Value(start);
                schedule.Add(optimalStart, task);
            }

            schedule.Sort();
            return schedule;
        }
    }
}
