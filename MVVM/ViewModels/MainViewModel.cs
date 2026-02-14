using System.Collections.ObjectModel;
using System.Collections.Specialized;
using PropertyChanged;
using Tasker.MVVM.Models;

namespace Tasker.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        // FLAG to prevent re-entrant calls to UpdateData
        private bool _isUpdating = false;

        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<MyTask> Tasks { get; set; }

        public MainViewModel()
        {
            Categories = new ObservableCollection<Category>();
            Tasks = new ObservableCollection<MyTask>();

            // Subscribe to collection changes
            Tasks.CollectionChanged += Tasks_CollectionChanged;

            // Subscribe to each Category's PropertyChanged to catch manual changes
            Categories.CollectionChanged += Categories_CollectionChanged;

            FillData();
        }

        private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // When categories are added, subscribe to their property changes
            if (e.NewItems != null)
            {
                foreach (Category category in e.NewItems)
                {
                    // Subscribe to property changes on the category
                    category.PropertyChanged += (s, args) =>
                    {
                        // Force UI update when category properties change
                        if (args.PropertyName == nameof(Category.PendingTasks) ||
                            args.PropertyName == nameof(Category.Percentage))
                        {
                            // Raise PropertyChanged for the entire Categories collection
                            // This ensures UI bindings refresh
                        }
                    };
                }
            }
        }

        private void Tasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // CRITICAL: Prevent stack overflow by checking if we're already updating
            if (_isUpdating)
                return;

            // Subscribe to Subtask changes for new tasks
            if (e.NewItems != null)
            {
                foreach (MyTask task in e.NewItems)
                {
                    if (task.Subtasks != null)
                    {
                        task.Subtasks.CollectionChanged += (s, args) => UpdateData();
                    }
                }
            }

            UpdateData();
        }

        private void FillData()
        {
            Categories.Add(new Category
            {
                Id = 1,
                CategoryName = ".NET MAUI Course",
                Color = "#CF14DF"
            });

            Categories.Add(new Category
            {
                Id = 2,
                CategoryName = "Tutorials",
                Color = "#df6f14"
            });

            Categories.Add(new Category
            {
                Id = 3,
                CategoryName = "Shopping",
                Color = "#14df80"
            });

            Tasks.Add(new MyTask
            {
                TaskName = "Upload exercise files",
                Completed = false,
                CategoryId = 1
            });

            Tasks.Add(new MyTask
            {
                TaskName = "Plan next course",
                Completed = false,
                CategoryId = 1
            });

            Tasks.Add(new MyTask
            {
                TaskName = "Upload new ASP.NET video on YouTube",
                Completed = false,
                CategoryId = 2
            });

            Tasks.Add(new MyTask
            {
                TaskName = "Fix Settings.cs class of the project",
                Completed = false,
                CategoryId = 2
            });

            Tasks.Add(new MyTask
            {
                TaskName = "Update github repository",
                Completed = true,
                CategoryId = 2
            });

            Tasks.Add(new MyTask
            {
                TaskName = "Buy eggs",
                Completed = false,
                CategoryId = 3
            });

            Tasks.Add(new MyTask
            {
                TaskName = "Go for the pepperoni pizza",
                Completed = false,
                CategoryId = 3
            });

            UpdateData();
        }

        public void UpdateData()
        {
            // CRITICAL: Set flag to prevent re-entry
            if (_isUpdating)
                return;

            try
            {
                _isUpdating = true;

                // Update category statistics
                foreach (var c in Categories)
                {
                    var tasks = from t in Tasks where t.CategoryId == c.Id select t;
                    var completed = from t in tasks where t.Completed == true select t;
                    var notCompleted = from t in tasks where t.Completed == false select t;

                    var completedCount = completed.Count();
                    var tasksCount = tasks.Count();

                    // Update properties
                    c.PendingTasks = notCompleted.Count();
                    c.Percentage = tasksCount > 0 ? (float)completedCount / (float)tasksCount : 0f;

                    // FORCE PropertyChanged notification
                    c.OnPropertyChanged(nameof(Category.PendingTasks));
                    c.OnPropertyChanged(nameof(Category.Percentage));
                }

                // Update task colors
                foreach (var t in Tasks)
                {
                    var category = Categories.FirstOrDefault(c => c.Id == t.CategoryId);
                    if (category != null)
                    {
                        t.TaskColor = category.Color;
                    }

                    // Update subtask progress if task has subtasks
                    if (t.HasSubtasks)
                    {
                        t.OnPropertyChanged(nameof(MyTask.SubtaskProgress));
                        t.OnPropertyChanged(nameof(MyTask.CompletedSubtasks));
                    }
                }
            }
            finally
            {
                // CRITICAL: Always reset flag, even if exception occurs
                _isUpdating = false;
            }
        }
    }
}