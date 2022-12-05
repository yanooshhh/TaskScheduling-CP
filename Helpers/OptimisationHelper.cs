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

        static public (CpModel, AllJobsOptVariables) PrepareModel(List<Job> jobs, int fullPeriod)
        {
            CpModel model = new CpModel();
            AllJobsOptVariables allTasksVars = new();

            foreach (var job in jobs)
            {
                string suffix = $"__{job.TaskDefinition.Name}_r{job.Release}";
                IntVar start = model.NewIntVar(job.Release, fullPeriod, "start" + suffix);
                IntervalVar intervalVar = model.NewFixedSizeIntervalVar(start, job.TaskDefinition.Duration, "interval" + suffix);

                allTasksVars.AddTT(job, start, intervalVar);
            }

            // constraints
            model.AddNoOverlap(allTasksVars.GetTTVars().Select(t => t.Value.Item2));

            // objective
            model.Minimize(GetLossExpression(allTasksVars));

            return (model, allTasksVars);
        }

        static private LinearExpr GetLossExpression(AllJobsOptVariables allTasksVars)
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
    }
}
