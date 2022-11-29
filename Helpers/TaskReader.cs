using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPplayground.Entities;

namespace CPplayground.Helpers
{
    internal class TaskReader
    {
        static public List<TaskDefinition> LoadTasks(string path)
        {
            //Tuple containing two lists, TT task list and ET task list
            List<TaskDefinition> tasks = new List<TaskDefinition>();
            //Configures CSV parser and sets delimiter
            string currentDir = Environment.CurrentDirectory;

            using var reader = new StreamReader(path);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";"
            };
            using var csv = new CsvReader(reader, config);

            csv.Read();
            csv.ReadHeader();
            // Reads CSV file and creates TT and ET task objects bases on type field
            while (csv.Read())
            {
                tasks.Add(new TaskDefinition(
                    csv.GetField<string>("name"),
                    csv.GetField<int>("duration"),
                    csv.GetField<int>("period"),
                    "ET".Equals(csv.GetField("type")),
                    csv.GetField<int>("priority"),
                    csv.GetField<int>("deadline"),
                    csv.GetField<int>("separation")));
            }
            return tasks;
        }
    }
}
