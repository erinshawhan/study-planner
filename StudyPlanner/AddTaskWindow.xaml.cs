using StudyPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StudyPlanner.Views
{
    public partial class AddTaskWindow : Window
    {
        public StudyTask Task { get; private set; }

        public AddTaskWindow(StudyTask task = null)
        {
            InitializeComponent();

            Task = task ?? new StudyTask { DueDate = DateTime.Today };
            TitleBox.Text = Task.Title;
            SubjectBox.Text = Task.Subject;
            DueDatePicker.SelectedDate = Task.DueDate;
            IsCompletedCheckBox.IsChecked = Task.IsCompleted;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Task.Title = TitleBox.Text;
            Task.Subject = SubjectBox.Text;
            Task.DueDate = DueDatePicker.SelectedDate ?? DateTime.Today;
            Task.IsCompleted = IsCompletedCheckBox.IsChecked ?? false;
            DialogResult = true;
            Close();
        }
    }
}
