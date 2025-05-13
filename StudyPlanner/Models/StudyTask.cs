using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlanner.Models
{
    public class StudyTask : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _title;
        public string Title { get => _title; set { _title = value; OnPropertyChanged(); } }
        private string _subject;
        public string Subject { get => _subject; set { _subject = value; OnPropertyChanged(); } }
        private DateTime _dueDate;
        public DateTime DueDate { get => _dueDate; set { _dueDate = value; OnPropertyChanged(); } }
        private bool _isCompleted;
        public bool IsCompleted { get => _isCompleted; set { _isCompleted = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "") => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
