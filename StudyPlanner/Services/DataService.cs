using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyPlanner.Models;
using System.IO;
using System.Text.Json;

namespace StudyPlanner.Services
{
    public static class DataService
    {
        private static readonly string FilePath = "tasks.json";

        public static List<StudyTask> LoadTasks()
        {
            if (!File.Exists(FilePath)) return new List<StudyTask>();

            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<StudyTask>>(json, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true}) ?? new List<StudyTask>();
        }

        public static void SaveTasks(IEnumerable<StudyTask> tasks)
        {
            string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }
}
