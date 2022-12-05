using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPplayground.Entities
{
    internal class Job
    {
        public Job(TaskDefinition taksDef, int release, int absoluteDeadline)
        {
            TaskDefinition = taksDef;
            Release = release;
            AbsoluteDeadline = absoluteDeadline;
        }
        public TaskDefinition TaskDefinition { get; set; }
        public int Release { get; set; }
        public int AbsoluteDeadline { get; set; }

        public override string ToString()
        {
            return $" task: {TaskDefinition.Name}, released at: {Release}, deadline: {AbsoluteDeadline}, duration {TaskDefinition.Duration}, separation: {TaskDefinition.Separation}";
        }
    }
}
