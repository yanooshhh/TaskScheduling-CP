using System;
using System.Linq;
using CPplayground;
using CPplayground.Entities;
using CPplayground.Helpers;
using Google.OrTools.Sat;

string testCasePath = "test_cases\\inf_10_10_seperation\\seperationCase.csv";

var allTaskDefinitions = TaskReader.LoadTasks(testCasePath);
var allTtTaskDefinitions = allTaskDefinitions.Where(t => !t.IsET).ToList();
var allEtTaskDefinitions = allTaskDefinitions.Where(t => t.IsET).ToList();
var pollingServers = PollingServersHelper.GetPollingServersDefinitionsFromETs(allEtTaskDefinitions);
var allTtTasksAndPollingServers = allTtTaskDefinitions.Concat(pollingServers).ToList();

int fullPeriod = MathHelper.GetLCM(allTtTasksAndPollingServers.Select(x => x.Period).ToArray());
var allJobs = SchedulingHelper.GetJobsFromTaskDefinitions(allTtTasksAndPollingServers, fullPeriod);

(CpModel model, AllJobsOptVariables allJobsVars) = OptimisationHelper.PrepareModel(allJobs, fullPeriod);

CpSolver solver = new();
CpSolverStatus status = solver.Solve(model);

Console.WriteLine($"Solve status: {status}, lossVal: {solver.BestObjectiveBound}");

var schedule = OptimisationHelper.ConvertToSchedule(allJobsVars, solver);
schedule.PrintFullSchedule();
Console.WriteLine($"All tasks are {(schedule.IsSchedulable() ? "" : "NOT ")}scheduled before their deadlines.");




