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
    public class MyTask
    {
        public string TaskName { get; set; }
        public bool Completed { get; set; }
        public int CategoryId { get; set; }
        public string TaskColor { get; set; }

        // NEW: Deadline support
        public DateTime? Deadline { get; set; }
        public bool HasDeadline => Deadline.HasValue;
        public string DeadlineText => Deadline.HasValue
            ? $"Due: {Deadline.Value:MMM dd, yyyy}"
            : "No deadline";

        // NEW: Subtasks support
        public ObservableCollection<Subtask> Subtasks { get; set; }

        // NEW: Progress calculation
        public int TotalSubtasks => Subtasks?.Count ?? 0;
        public int CompletedSubtasks => Subtasks?.Count(s => s.Completed) ?? 0;
        public float SubtaskProgress => TotalSubtasks > 0
            ? (float)CompletedSubtasks / TotalSubtasks
            : 0f;
        public bool HasSubtasks => Subtasks != null && Subtasks.Count > 0;

        public MyTask()
        {
            Subtasks = new ObservableCollection<Subtask>();
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class Subtask
    {
        public string TaskName { get; set; }
        public bool Completed { get; set; }
    }
}