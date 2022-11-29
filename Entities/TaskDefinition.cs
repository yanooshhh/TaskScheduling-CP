using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPplayground.Entities
{
    internal class TaskDefinition
    {
        public TaskDefinition(string name, int duration, int period, bool isET, int priority, int deadline, int separation, List<TaskDefinition>? tasks = null)
        {
            Name = name;
            Duration = duration;   
            Period = period;
            IsET = isET;
            Priority = priority;
            Deadline = deadline;
            Separation = separation;
            Tasks = tasks;
        }

        public string Name { get; set; }
        public int Duration { get; set; }
        public int Period { get; set; }
        public bool IsET { get; set; }
        public int Priority { get; set; }
        public int Deadline { get; set; }
        public int Separation { get; set; }
        public List<TaskDefinition>? Tasks { get; set; }


        
    }
}
