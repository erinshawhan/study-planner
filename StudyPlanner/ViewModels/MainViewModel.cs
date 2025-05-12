using GalaSoft.MvvmLight.Command;
using StudyPlanner.Models;
using StudyPlanner.Services;
using StudyPlanner.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StudyPlanner.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<StudyTask> Tasks { get; set; } = [];

        private StudyTask? _selectedTask;
        public StudyTask SelectedTask
        {
            get => _selectedTask!;
            set { _selectedTask = value; OnPropertyChanged(); }
        }
        public ObservableCollection<StudyTask>[] WeeklyTasks { get; private set; } = new ObservableCollection<StudyTask>[7];
        private DateTime _currentWeekStart;
        public DateTime CurrentWeekStart
        {
            get => _currentWeekStart;
            set
            {
                _currentWeekStart = value;
                OnPropertyChanged();
                UpdateWeeklyTasks();
            }
        }
        public double OverallProgress
        {
            get
            {
                if (Tasks.Count == 0) return 0;
                int completed = Tasks.Count(t => t.IsCompleted);
                return (double)completed / Tasks.Count * 100;
            }
        }

        public ICommand AddTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand EditTaskCommand { get; }

        public MainViewModel()
        {
            AddTaskCommand = new RelayCommand<ICommand>(_ => AddTask());
            EditTaskCommand = new RelayCommand<ICommand>(_ => EditTask(), _ => SelectedTask != null);
            DeleteTaskCommand = new RelayCommand<ICommand>(_ => DeleteTask(), _ => SelectedTask != null);
            CurrentWeekStart = GetStartOfWeek(DateTime.Today);

            static DateTime GetStartOfWeek(DateTime dt)
            {
                int diff = dt.DayOfWeek - DayOfWeek.Sunday;
                return dt.AddDays(-diff).Date;
            }

            Tasks.CollectionChanged += (s, e) => NotifyProgressUpdate();

            var loaded = DataService.LoadTasks();
            foreach (var task in loaded)
            {
                Tasks.Add(task);
            }

            Tasks.CollectionChanged += (s, e) => SaveAll();
            foreach (var task in Tasks)
            {
                task.PropertyChanged += Task_PropertyChanged!;
            }

            NotifyProgressUpdate();
            CurrentWeekStart = GetStartOfWeek(DateTime.Today);
        }

        private void AddTask()
        {
            var window = new AddTaskWindow();
            if (window.ShowDialog() == true)
            {
                Tasks.Add(window.Task);
            }
        }

        private void EditTask()
        {
            var window = new AddTaskWindow(new StudyTask
            {
                Title = SelectedTask.Title,
                Subject = SelectedTask.Subject,
                DueDate = SelectedTask.DueDate,
                IsCompleted = SelectedTask.IsCompleted
            });

            if (window.ShowDialog() == true)
            {
                SelectedTask.Title = window.Task.Title;
                SelectedTask.Subject = window.Task.Subject;
                SelectedTask.DueDate = window.Task.DueDate;
                SelectedTask.IsCompleted = window.Task.IsCompleted;
                OnPropertyChanged(nameof(Tasks));
            }
        }

        private void DeleteTask()
        {
            if (SelectedTask != null)
            {
                if (SelectedTask != null)
                {
                    Tasks.Remove(SelectedTask);
                }
            }
        }

        private void UpdateWeeklyTasks()
        {
            for (int i = 0; i < 7; i++)
            {
                WeeklyTasks[i] = [];
            }

            foreach (var task in Tasks)
            {
                var diff = (task.DueDate.Date - CurrentWeekStart).Days;
                if (diff >= 0 && diff < 7)
                {
                    WeeklyTasks[diff].Add(task);
                }
            }

            OnPropertyChanged(nameof(WeeklyTasks));
        }

        private void NotifyProgressUpdate()
        {
            foreach (var task in Tasks)
            {
                task.PropertyChanged -= Task_PropertyChanged!;
                task.PropertyChanged += Task_PropertyChanged!;
            }
            OnPropertyChanged(nameof(OverallProgress));
        }

        private void Task_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveAll();
            if (e.PropertyName == nameof(StudyTask.IsCompleted))
            {
                OnPropertyChanged(nameof(OverallProgress));
            }
        }

        private void SaveAll()
        {
            DataService.SaveTasks(Tasks);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
