using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.MVVM.Models
{
     [AddINotifyPropertyChangedInterface]
     public class Category
     {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string Color { get; set; }
        public int PendingTasks { get; set; }
        public float Percentage { get; set; }
        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }
        public DateTime Deadline { get; set; }
        public int TotalTasks { get; set; }
        public ObservableCollection<MyTask> CategoryTasks { get; set; } = new ObservableCollection<MyTask>();
    }
}
