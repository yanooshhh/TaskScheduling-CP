using System;
using System.Diagnostics;
using System.Linq;
using CPplayground;
using CPplayground.Entities;
using CPplayground.Helpers;
using Google.OrTools.Sat;

string testCasePath = "FinalTestCases\\test";

for (int i = 1; i <= 5; i++)
{
    Console.WriteLine($"\n----- TEST {i} -----\n");
    
    Stopwatch stopwatch = new();

    var allTaskDefinitions = TaskReader.LoadTasks(testCasePath + i + ".csv");
    var allTtTaskDefinitions = allTaskDefinitions.Where(t => !t.IsET).ToList();
    var allEtTaskDefinitions = allTaskDefinitions.Where(t => t.IsET).ToList();
    var pollingServers = PollingServersHelper.GetPollingServersDefinitionsFromETs(allEtTaskDefinitions);
    var allTtTasksAndPollingServers = allTtTaskDefinitions.Concat(pollingServers).ToList();

    int fullPeriod = MathHelper.GetLCM(allTtTasksAndPollingServers.Select(x => x.Period).ToArray());
    var allJobs = SchedulingHelper.GetJobsFromTaskDefinitions(allTtTasksAndPollingServers, fullPeriod);

    (CpModel model, AllJobsOptVariables allJobsVars) = OptimisationHelper.PrepareModel(allJobs, fullPeriod);

    CpSolver solver = new();

    stopwatch.Start();
    CpSolverStatus status = solver.Solve(model);
    stopwatch.Stop();

    Console.WriteLine($"Solve status: {status}, lossVal: {solver.BestObjectiveBound}");

    var schedule = OptimisationHelper.ConvertToSchedule(allJobsVars, solver);
    schedule.PrintFullSchedule();
    Console.WriteLine($"All TT tasks are {(schedule.IsSchedulable() ? "" : "NOT ")}scheduled before their deadlines.");
    Console.WriteLine($"The avg WCRT for TT tasks: {schedule.GetAvgWRCTForTT()}");
    Console.WriteLine($"The solution was found in {stopwatch.ElapsedMilliseconds}ms");
}




