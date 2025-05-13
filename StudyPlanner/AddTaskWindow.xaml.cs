using StudyPlanner.Models;
using StudyPlanner.ViewModels;
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

            var mainVM = ((App)Application.Current).MainWindow.DataContext as MainViewModel;
            //SubjectComboBox.ItemsSource = mainVM?.Subjects;

            Task = task ?? new StudyTask { DueDate = DateTime.Today };
            TitleBox.Text = Task.Title;
            SubjectComboBox.Text = Task.Subject;
            DueDatePicker.SelectedDate = Task.DueDate;
            IsCompletedCheckBox.IsChecked = Task.IsCompleted;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Validate title
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                MessageBox.Show("Please enter a title for the task.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                TitleBox.BorderBrush = Brushes.Red;
                return;
            }

            // Validate subject
            if (string.IsNullOrWhiteSpace(SubjectComboBox.Text))
            {
                MessageBox.Show("Please enter a subject.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                SubjectComboBox.BorderBrush = Brushes.Red;
                return;
            }

            // Save the task
            Task.Title = TitleBox.Text.Trim();
            Task.Subject = SubjectComboBox.Text.Trim();
            Task.DueDate = DueDatePicker.SelectedDate ?? DateTime.Today;
            Task.IsCompleted = IsCompletedCheckBox.IsChecked ?? false;

            DialogResult = true;
            Close();
        }

    }
}
