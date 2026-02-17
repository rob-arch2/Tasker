using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using Tasker.MVVM.Models;

namespace Tasker.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class NewTaskViewModel
    {
        public string Task { get; set; }
        public Category SelectedCategory { get; set; }
        public DateTime Deadline { get; set; } = DateTime.Now.AddDays(7);
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

            var newTask = new MyTask
            {
                TaskName = Task,
                CategoryId = SelectedCategory.Id,
                Completed = false,
                Deadline = HasDeadline ? Deadline : (DateTime?)null,
                Subtasks = new ObservableCollection<Subtask>(Subtasks)
            };

            Tasks.Add(newTask);
        }
    }
}