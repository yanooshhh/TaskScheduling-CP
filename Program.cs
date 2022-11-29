using System;
using System.Linq;
using CPplayground;
using CPplayground.Entities;
using CPplayground.Helpers;
using Google.OrTools.Sat;

string testCasePath = "test_cases\\inf_10_10_seperation\\seperationCase.csv";

CpModel model = new CpModel();
AllTaskInstancesWithVars allTasksVars = new();

var allTaskDefinitions = TaskReader.LoadTasks(testCasePath);
var allTtTaskDefinitions = allTaskDefinitions.Where(t => !t.IsET).ToList();
var allEtTaskDefinitions = allTaskDefinitions.Where(t => t.IsET).ToList();
var pollingServers = PollingServersHelper.GetPollingServersDefinitionsFromETs(allEtTaskDefinitions);
var allTtTasksAndPollingServers = allTtTaskDefinitions.Concat(pollingServers).ToList();

int fullPeriod = AuxiliaryHelper.GetLCM(allTtTasksAndPollingServers.Select(x => x.Period).ToArray());
var allTasksInstances = SchedulingHelper.GetAllRepeatingTasks(allTtTasksAndPollingServers, fullPeriod);

foreach (var taskInstance in allTasksInstances)
{
    String suffix = $"__{taskInstance.TaskDefinition.Name}_r{taskInstance.Release}";
    IntVar start = model.NewIntVar(taskInstance.Release, fullPeriod, "start" + suffix);
    IntVar end = model.NewIntVar(taskInstance.Release, fullPeriod, "end" + suffix);
    IntervalVar intervalVar = model.NewIntervalVar(start, taskInstance.TaskDefinition.Duration, end, "interval" + suffix);

    allTasksVars.AddTT(taskInstance, start, end, intervalVar);
}

// constraints
model.AddNoOverlap(allTasksVars.GetTTVars().Select(t => t.Value.Item3));

// objective
model.Minimize(OptimisationHelper.GetLossExpression(allTasksVars));


CpSolver solver = new();
CpSolverStatus status = solver.Solve(model);

Console.WriteLine($"Solve status: {status}, lossVal: {solver.BestObjectiveBound}");

var schedule = OptimisationHelper.ConvertToSchedule(allTasksVars, solver);

schedule.PrintFullSchedule();

