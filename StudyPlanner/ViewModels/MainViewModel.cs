using GalaSoft.MvvmLight.Command;
using StudyPlanner.Models;
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
        public ObservableCollection<StudyTask> Tasks { get; set; } = new();

        private StudyTask _selectedTask;
        public StudyTask SelectedTask
        {
            get => _selectedTask;
            set { _selectedTask = value; OnPropertyChanged(); }
        }

        public ICommand AddTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }

        public MainViewModel()
        {
            AddTaskCommand = new RelayCommand<ICommand>(_ => AddTask());
            DeleteTaskCommand = new RelayCommand<ICommand>(_ => DeleteTask(), _ => SelectedTask != null);
        }

        private void AddTask()
        {
            Tasks.Add(new StudyTask
            {
                Title = "New Task",
                Subject = "General",
                DueDate = DateTime.Today.AddDays(1)
            });
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
