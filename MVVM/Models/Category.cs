using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.MVVM.Models
{
    public class Category : INotifyPropertyChanged
    {
        private int _id;
        private string _categoryName;
        private string _color;
        private int _pendingTasks;
        private float _percentage;
        private bool _isSelected;
        private bool _isExpanded;
        private DateTime _deadline;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public string CategoryName
        {
            get => _categoryName;
            set { _categoryName = value; OnPropertyChanged(); }
        }

        public string Color
        {
            get => _color;
            set { _color = value; OnPropertyChanged(); }
        }

        public int PendingTasks
        {
            get => _pendingTasks;
            set
            {
                if (_pendingTasks != value)
                {
                    _pendingTasks = value;
                    OnPropertyChanged();
                }
            }
        }

        public float Percentage
        {
            get => _percentage;
            set
            {
                if (Math.Abs(_percentage - value) > 0.001f)
                {
                    _percentage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        // NEW: IsExpanded for collapsible categories
        public bool IsExpanded
        {
            get => _isExpanded;
            set { _isExpanded = value; OnPropertyChanged(); }
        }

        // NEW: Deadline for categories
        public DateTime Deadline
        {
            get => _deadline;
            set { _deadline = value; OnPropertyChanged(); }
        }

        // NEW: Helper properties for bindings
        public int TotalTasks => CategoryTasks?.Count ?? 0;

        // NEW: List of tasks in this category
        public ObservableCollection<MyTask> CategoryTasks { get; set; }

        public Category()
        {
            CategoryTasks = new ObservableCollection<MyTask>();
            _isExpanded = false;
            _deadline = DateTime.Today.AddHours(23).AddMinutes(59);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}