using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlanner.Models
{
    public class StudyTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
