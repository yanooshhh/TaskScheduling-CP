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
        static readonly long DEADLINE_PENALTY = 1000000;

        static public Schedule ConvertToSchedule(AllJobsOptVariables allJobsVars, CpSolver solver)
        {
            var schedule = new Schedule();

            foreach ((var job, var interval) in allJobsVars.GetOptVars())
                schedule.Add((int) solver.Value(interval.StartExpr()), job);

            schedule.Sort();
            return schedule;
        }

        static public (CpModel, AllJobsOptVariables) PrepareModel(List<Job> jobs, int fullPeriod)
        {
            CpModel model = new();
            AllJobsOptVariables allTasksVars = new();

            foreach (var job in jobs)
            {
                string suffix = $"__{job.TaskDefinition.Name}_r{job.Release}";
                IntVar start = model.NewIntVar(job.Release, fullPeriod, "start" + suffix);
                IntervalVar intervalVar = model.NewFixedSizeIntervalVar(start, job.TaskDefinition.Duration, "interval" + suffix);

                allTasksVars.Add(job, intervalVar);
            }

            model.AddNoOverlap(allTasksVars.GetOptVars().Select(t => t.Value));
            model.Minimize(GetLossExpression(allTasksVars));

            return (model, allTasksVars);
        }

        static private LinearExpr GetLossExpression(AllJobsOptVariables allJobsVars)
        {
            List<LinearExpr> allExpressions = new();

            foreach ((var job, var interval) in allJobsVars.GetOptVars())
            {
                if (interval.EndExpr() > job.AbsoluteDeadline)
                    allExpressions.Add(LinearExpr.Constant(DEADLINE_PENALTY));
                else
                    allExpressions.Add(interval.EndExpr() - job.Release);
            }

            return LinearExpr.Sum(allExpressions);
        }
    }
}
