using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using StudyPlanner.Models;

namespace StudyPlanner.Services
{
    class ReminderService
    {
        public static void ShowReminder(StudyTask task)
        {
            new ToastContentBuilder()
                  .AddText("Upcoming Task Due!")
                  .AddText($"{task.Title} is due on {task.DueDate:d}")
                  .Show();
        }
    }
}
