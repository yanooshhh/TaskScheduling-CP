using System;
using System.Linq;
using CPplayground;
using CPplayground.Entities;
using CPplayground.Helpers;
using Google.OrTools.Sat;

string testCasePath = "test_cases\\inf_10_10_seperation\\seperationCase.csv";

CpModel model = new CpModel();
AllJobsOptVariables allTasksVars = new();

var allTaskDefinitions = TaskReader.LoadTasks(testCasePath);
var allTtTaskDefinitions = allTaskDefinitions.Where(t => !t.IsET).ToList();
var allEtTaskDefinitions = allTaskDefinitions.Where(t => t.IsET).ToList();
var pollingServers = PollingServersHelper.GetPollingServersDefinitionsFromETs(allEtTaskDefinitions);
var allTtTasksAndPollingServers = allTtTaskDefinitions.Concat(pollingServers).ToList();

int fullPeriod = AuxiliaryHelper.GetLCM(allTtTasksAndPollingServers.Select(x => x.Period).ToArray());
var allJobs = SchedulingHelper.GetJobsFromTaskDefinitions(allTtTasksAndPollingServers, fullPeriod);

foreach (var job in allJobs)
{
    string suffix = $"__{job.TaskDefinition.Name}_r{job.Release}";
    IntVar start = model.NewIntVar(job.Release, fullPeriod, "start" + suffix);
    IntervalVar intervalVar = model.NewFixedSizeIntervalVar(start, job.TaskDefinition.Duration, "interval" + suffix);

    allTasksVars.AddTT(job, start, intervalVar);
}

// constraints
model.AddNoOverlap(allTasksVars.GetTTVars().Select(t => t.Value.Item2));

// objective
model.Minimize(OptimisationHelper.GetLossExpression(allTasksVars));


CpSolver solver = new();
CpSolverStatus status = solver.Solve(model);

Console.WriteLine($"Solve status: {status}, lossVal: {solver.BestObjectiveBound}");

var schedule = OptimisationHelper.ConvertToSchedule(allTasksVars, solver);

schedule.PrintFullSchedule();


