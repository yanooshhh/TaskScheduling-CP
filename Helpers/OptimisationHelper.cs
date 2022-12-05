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
        static public LinearExpr GetLossExpression(AllJobsOptVariables allTasksVars)
        {
            List<LinearExpr> allExpressions = new List<LinearExpr>();
            
            foreach (var taskVars in allTasksVars.GetTTVars())
            {
                var task = taskVars.Key;
                (var start, var interval) = taskVars.Value;
                if (interval.EndExpr() > taskVars.Key.AbsoluteDeadline)
                {
                    allExpressions.Add(LinearExpr.Constant(DEADLINE_PENALTY));              
                } 
                else
                {
                    allExpressions.Add(interval.EndExpr() - task.Release);
                }
            }

            return LinearExpr.Sum(allExpressions);
        }

        static public Schedule ConvertToSchedule(AllJobsOptVariables allTasksVars, CpSolver solver)
        {
            var schedule = new Schedule();

            foreach (var taskWithVars in allTasksVars.GetTTVars())
            {
                (var task, (var start, _)) = taskWithVars;

                int optimalStart = (int) solver.Value(start);
                schedule.Add(optimalStart, task);
            }

            schedule.Sort();
            return schedule;
        }
    }
}
