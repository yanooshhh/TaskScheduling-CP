using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPplayground.Entities
{
    internal class Schedule
    {
        private readonly List<Tuple<int, Job>> schedule;
        public Schedule() { schedule = new List<Tuple<int, Job>>(); }
        
        public void Add(int start, Job task)
        {
            schedule.Add(Tuple.Create(start, task));
        }

        public List<Tuple<int, Job>> Get()
        {
            return schedule;
        }

        public List<Tuple<int, Job>> GetServer()
        {
            return schedule.Where(item => item.Item2.TaskDefinition.IsET).ToList();
        }

        public void Sort()
        {
            schedule.Sort((x, y) => x.Item1.CompareTo(y.Item1));
        }

        public void PrintFullSchedule()
        {
            Console.WriteLine($"No. of jobs: {schedule.Count}");
            schedule.ForEach(item =>
            {
                Console.WriteLine($"Interval: [{item.Item1}; {item.Item1 + item.Item2.TaskDefinition.Duration}) {item.Item2}");
            });
        }


    }
}
