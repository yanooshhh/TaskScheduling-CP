//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CPplayground.Entities
//{
//    internal class PollingServerSlot
//    {
//        public PollingServerSlot(int sereverId, int startTime, int endTime, List<Job> tasks)
//        {
//            ServerId = sereverId;
//            StartTime = startTime;
//            EndTime = endTime;
//            TaskInstances = tasks;
//        }
//        public int ServerId { get; set; }
//        public int StartTime { get; set; }
//        public int EndTime { get; set; }
//        public List<Job> TaskInstances { get; set; }
//        public override string ToString()
//        {
//            return $"Polling server {ServerId} running [{StartTime};{EndTime}) serving {TaskInstances.Count} task(s)";
//        }
//    }
//}
