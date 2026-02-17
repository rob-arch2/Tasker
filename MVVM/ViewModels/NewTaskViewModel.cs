using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasker.MVVM.Models;

namespace Tasker.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class NewTaskViewModel
    {
        public string Task { get; set; }
        public Category SelectedCategory { get; set; }

        // Separate Date and Time for better UX
        public DateTime DeadlineDate { get; set; } = DateTime.Today.AddDays(7);
        public TimeSpan DeadlineTime { get; set; } = new TimeSpan(17, 0, 0); // Default 5:00 PM

        public bool HasDeadline { get; set; } = false;
        public string NewSubtaskText { get; set; }

        public ObservableCollection<MyTask> Tasks { get; set; }
        public ObservableCollection<Category> Categories { get; set; }

        public ObservableCollection<Subtask> Subtasks { get; set; }

        public NewTaskViewModel()
        {
            Subtasks = new ObservableCollection<Subtask>();
        }

        public void AddSubtask()
        {
            if (!string.IsNullOrWhiteSpace(NewSubtaskText))
            {
                Subtasks.Add(new Subtask
                {
                    TaskName = NewSubtaskText,
                    Completed = false
                });
                NewSubtaskText = string.Empty;
            }
        }

        public void RemoveSubtask(Subtask subtask)
        {
            Subtasks.Remove(subtask);
        }

        public void CreateTask()
        {
            if (string.IsNullOrWhiteSpace(Task) || SelectedCategory == null)
                return;

            // Combine date and time into a single DateTime
            DateTime? finalDeadline = null;
            if (HasDeadline)
            {
                finalDeadline = DeadlineDate.Date + DeadlineTime;
            }

            // Create main task
            var newTask = new MyTask
            {
                TaskName = Task,
                CategoryId = SelectedCategory.Id,
                Completed = false,
                Deadline = finalDeadline,
                Subtasks = new ObservableCollection<Subtask>(Subtasks),
                TaskColor = SelectedCategory.Color
            };

            Tasks.Add(newTask);
        }
    }
}