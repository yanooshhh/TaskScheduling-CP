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

//foreach (var taskInstance in allTasksInstances)
//{
//    String suffix = $"__{taskInstance.TaskDefinition.Name}_r{taskInstance.Release}";
//    IntVar start = model.NewIntVar(taskInstance.Release, fullPeriod, "start" + suffix);
//    IntVar end = model.NewIntVar(taskInstance.Release, fullPeriod, "end" + suffix);
//    IntervalVar intervalVar = model.NewIntervalVar(start, taskInstance.TaskDefinition.Duration, end, "interval" + suffix);

//    allTasksVars.AddTT(taskInstance, start, end, intervalVar);
//}

//// constraints
//model.AddNoOverlap(allTasksVars.GetTTVars().Select(t => t.Value.Item3));

//// objective
//model.Minimize(OptimisationHelper.GetLossExpression(allTasksVars));

var allTTTasksInstances = SchedulingHelper.GetAllRepeatingTasks(allTtTaskDefinitions, fullPeriod);

foreach (var taskInstance in allTTTasksInstances)
{
    String suffix = $"__{taskInstance.TaskDefinition.Name}_r{taskInstance.Release}";
    IntVar start = model.NewIntVar(taskInstance.Release, fullPeriod, "start" + suffix);
    IntVar end = model.NewIntVar(taskInstance.Release, fullPeriod, "end" + suffix);
    IntervalVar intervalVar = model.NewIntervalVar(start, taskInstance.TaskDefinition.Duration, end, "interval" + suffix);

    allTasksVars.AddTT(taskInstance, start, end, intervalVar);
}
// constraints for TT
model.AddNoOverlap(allTasksVars.GetTTVars().Select(t => t.Value.Item3));

for (int i = 0; i <= 3; i++)
{
    String suffix = $"__poolingServer{i}_";
    IntVar serverPeriod = model.NewIntVar(1, fullPeriod, "period" + suffix);
    IntVar serverDuration = model.NewIntVar(1, fullPeriod, "duration" + suffix);

    allTasksVars.AddPollingServer(i, serverDuration, serverPeriod);
}


// objective
model.Minimize(OptimisationHelper.GetNewLossExpression(allTasksVars, allEtTaskDefinitions, fullPeriod));

CpSolver solver = new();
CpSolverStatus status = solver.Solve(model);

Console.WriteLine($"Solve status: {status}, lossVal: {solver.BestObjectiveBound}");

var schedule = OptimisationHelper.ConvertToSchedule(allTasksVars, solver);

schedule.PrintFullSchedule();
Console.WriteLine("_______");

Console.WriteLine(solver.Value(allTasksVars.GetETVars()[0].Item2));


//var pollingServerSlots = PollingServersHelper.GetETPollingServersSlots(schedule);
//pollingServerSlots.ForEach(slot => Console.WriteLine(slot));
//Console.WriteLine("_______");

//(var isSchedulable, var responseTime) = PollingServersHelper.getSchedulabilityofET(pollingServerSlots, schedule.GetETs(), fullPeriod);

//Console.WriteLine($"Schedulability test: isSchedulable={isSchedulable}, responesTime={responseTime}");



// Check ET-schedulability now 

// instead of a start/stop for polling servers, maybe I should use a constant period or a budget... what that budget would be - I already use the sum of all tasks as the worst case budget



