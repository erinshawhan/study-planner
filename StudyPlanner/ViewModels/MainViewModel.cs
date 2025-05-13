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
    public class MainViewModel : INotifyPropertyChanged
    {
        public ICommand AddTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ObservableCollection<StudyTask> UpcomingTasks { get; set; } = [];
        public ObservableCollection<StudyTask> AllTasks { get; set; } = [];
        private ObservableCollection<StudyTask> _tasks = [];
        public ObservableCollection<StudyTask> Tasks 
        { 
            get => _tasks;
            set
            {
                if (_tasks != value)
                {
                    _tasks = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<StudyTask>[] WeeklyTasks { get; private set; } = new ObservableCollection<StudyTask>[7];
        public ObservableCollection<string> Subjects { get; set; } = [];

        private string? _selectedSubject;
        public string? SelectedSubject { get => _selectedSubject; set { _selectedSubject = value!; FilterTasks(); } }
        
        private DateTime? _selectedDate;
        public DateTime? SelectedDate 
        { 
            get => _selectedDate; 
            set { _selectedDate = value; FilterTasks(); OnPropertyChanged(); } 
        }

        private StudyTask? _selectedTask;
        public StudyTask? SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (_selectedTask != value)
                {
                    _selectedTask = value;

                    OnPropertyChanged();
                    (EditTaskCommand as RelayCommand<StudyTask>)?.RaiseCanExecuteChanged();
                    (DeleteTaskCommand as RelayCommand<StudyTask>)?.RaiseCanExecuteChanged();
                }
            }
        }

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
        
        //public double OverallProgress
        //{
        //    get
        //    {
        //        if (AllTasks.Count == 0) return 0;
        //        int completed = AllTasks.Count(t => t.IsCompleted);
        //        return (double)completed / AllTasks.Count * 100;
        //    }
        //}

        public double OverallProgress => AllTasks.Count == 0 
            ? 0 
            : (double)AllTasks.Count(t => t.IsCompleted) / AllTasks.Count * 100;

        public ICommand ClearFilterCommand => new RelayCommand(() =>
        {
            SelectedSubject = null;
            SelectedDate = null;
            FilterTasks();
        });

        public MainViewModel()
        {
            AddTaskCommand = new RelayCommand<object>(_ => AddTask());
            EditTaskCommand = new RelayCommand<StudyTask>(_ => EditTask(), _ => SelectedTask != null);
            DeleteTaskCommand = new RelayCommand<StudyTask>(_ => DeleteTask(), _ => SelectedTask != null);

            var loaded = DataService.LoadTasks();
            AllTasks = [.. loaded];
            Tasks = [];
            UpcomingTasks = [];
            Subjects = [];

            foreach (var task in AllTasks)
            {
                Tasks.Add(task);
                task.PropertyChanged += Task_PropertyChanged!;
            }

            UpdateSubjectList();
            FilterTasks();
            UpdateReminders();
            NotifyProgressUpdate();

            AllTasks.CollectionChanged += (s, e) =>
            {
                SaveAll();
                UpdateSubjectList();
                FilterTasks();
                UpdateReminders();
                OnPropertyChanged(nameof(OverallProgress));
            };

            CurrentWeekStart = GetStartOfWeek(DateTime.Today);
        }

        private void AddTask()
        {
            var window = new AddTaskWindow();
            if (window.ShowDialog() == true)
            {
                AllTasks.Add(window.Task);
            }
            OnPropertyChanged(nameof(OverallProgress));
        }

        private void EditTask()
        {
            if (SelectedTask == null) return;

            // Create a copy to pass
            var editableCopy = new StudyTask
            {
                Title = SelectedTask.Title,
                Subject = SelectedTask.Subject,
                DueDate = SelectedTask.DueDate,
                IsCompleted = SelectedTask.IsCompleted
            };

            var window = new AddTaskWindow(editableCopy);
            if (window.ShowDialog() == true)
            {
                // Apply changes to the original selected task
                SelectedTask.Title = window.Task.Title;
                SelectedTask.Subject = window.Task.Subject;
                SelectedTask.DueDate = window.Task.DueDate;
                SelectedTask.IsCompleted = window.Task.IsCompleted;

              
            }
        }

        private void DeleteTask()
        {
            if (SelectedTask != null && AllTasks.Contains(SelectedTask))
            {
                AllTasks.Remove(SelectedTask);
                OnPropertyChanged(nameof(OverallProgress));
            }
        }

        private void UpdateWeeklyTasks()
        {
            for (int i = 0; i < 7; i++)
            {
                WeeklyTasks[i] = [];
            }

            foreach (var task in AllTasks)
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
            foreach (var task in AllTasks)
            {
                task.PropertyChanged -= Task_PropertyChanged!;
                task.PropertyChanged += Task_PropertyChanged!;
            }
            OnPropertyChanged(nameof(OverallProgress));
        }

        private void Task_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveAll();

            if (e.PropertyName == nameof(StudyTask.IsCompleted) || e.PropertyName == nameof(StudyTask.DueDate))
            {
                OnPropertyChanged(nameof(OverallProgress));
                UpdateReminders();
            }
        }

        private void SaveAll()
        {
            DataService.SaveTasks(AllTasks);
        }

        private void UpdateReminders()
        {
            UpcomingTasks.Clear();
            var upcoming = AllTasks.Where(t => !t.IsCompleted && (t.DueDate - DateTime.Today).TotalDays <= 2 && t.DueDate >= DateTime.Today);

            foreach (var task in upcoming)
            {
                UpcomingTasks.Add(task);
            }
            OnPropertyChanged(nameof(UpcomingTasks));
        }

        private static DateTime GetStartOfWeek(DateTime dt)
        {
            int diff = dt.DayOfWeek - DayOfWeek.Sunday;
            if (diff < 0) diff += 7;
            return dt.AddDays(-diff).Date;
        }

        public void NotifyUpcomingTasks()
        {
            foreach (var task in AllTasks)
            {
                if (!task.IsCompleted && (task.DueDate - DateTime.Now).TotalHours<= 24 &&
                    (task.DueDate - DateTime.Now).TotalHours > 0)
                {
                    ReminderService.ShowReminder(task);
                }
            }
        }

        private void FilterTasks()
        {
            _tasks.Clear();

            var filtered = AllTasks.AsEnumerable();

            if (!string.IsNullOrEmpty(SelectedSubject))
            {
                filtered = filtered.Where(t => t.Subject == SelectedSubject);
            }
            if (SelectedDate.HasValue)
            {
                filtered = filtered.Where(t => t.DueDate.Date == SelectedDate.Value.Date);
            }

            foreach (var task in filtered)
            {
                Tasks.Add(task);
            }
        }


        private void UpdateSubjectList()
        {
            var uniqueSubjects = AllTasks.Select(t => t.Subject).Distinct().OrderBy(s => s);
            Subjects.Clear();
            foreach (var subject in uniqueSubjects)
            {
                Subjects.Add(subject);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
