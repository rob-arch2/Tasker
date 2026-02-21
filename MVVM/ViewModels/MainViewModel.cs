using System.Collections.ObjectModel;
using System.Collections.Specialized;
using PropertyChanged;
using Tasker.MVVM.Models;

namespace Tasker.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {
        private bool _isUpdating = false;

        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<MyTask> Tasks { get; set; }

        public Category SelectedCategory { get; set; }
        public string CurrentFilter { get; set; } = "All";

        public MainViewModel()
        {
            Categories = new ObservableCollection<Category>();
            Tasks = new ObservableCollection<MyTask>();

            Tasks.CollectionChanged += Tasks_CollectionChanged;
            Categories.CollectionChanged += Categories_CollectionChanged;

            FillData();
        }

        private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Category category in e.NewItems)
                {
                    category.PropertyChanged += (s, args) => { };
                }
            }
        }

        private void Tasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isUpdating) return;

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
            // Add categories with deadlines
            Categories.Add(new Category
            {
                Id = 1,
                CategoryName = ".NET MAUI Course",
                Color = "#CF14DF",
                Deadline = DateTime.Today.AddDays(7)
            });

            Categories.Add(new Category
            {
                Id = 2,
                CategoryName = "Tutorials",
                Color = "#df6f14",
                Deadline = DateTime.Today.AddDays(3)
            });

            Categories.Add(new Category
            {
                Id = 3,
                CategoryName = "Shopping",
                Color = "#14df80",
                Deadline = DateTime.Today.AddDays(1)
            });

            // Add tasks - TaskColor will be set by UpdateData()
            Tasks.Add(new MyTask { TaskName = "Upload exercise files", Completed = false, CategoryId = 1 });
            Tasks.Add(new MyTask { TaskName = "Plan next course", Completed = false, CategoryId = 1 });
            Tasks.Add(new MyTask { TaskName = "Upload new ASP.NET video on YouTube", Completed = false, CategoryId = 2 });
            Tasks.Add(new MyTask { TaskName = "Fix Settings.cs class of the project", Completed = false, CategoryId = 2 });
            Tasks.Add(new MyTask { TaskName = "Update github repository", Completed = true, CategoryId = 2 });
            Tasks.Add(new MyTask { TaskName = "Buy eggs", Completed = false, CategoryId = 3 });
            Tasks.Add(new MyTask { TaskName = "Go for the pepperoni pizza", Completed = false, CategoryId = 3 });

            UpdateData();
        }

        public void SelectCategory(Category category)
        {
            if (SelectedCategory == category)
            {
                SelectedCategory = null;
                foreach (var c in Categories)
                    c.IsSelected = false;
            }
            else
            {
                SelectedCategory = category;
                foreach (var c in Categories)
                    c.IsSelected = (c == category);
            }

            ApplyFilter();
        }

        public void SetFilter(string filter)
        {
            CurrentFilter = filter;
            ApplyFilter();
        }

        public void ApplyFilter()
        {
            foreach (var c in Categories)
            {
                // If a specific category is selected, hide all others
                if (SelectedCategory != null && SelectedCategory != c)
                {
                    c.IsVisible = false;
                    continue;
                }

                // Check the filter - show/hide the CATEGORY based on whether it HAS pending or done tasks
                if (CurrentFilter == "Pending")
                {
                    // Show category only if it has at least one pending task
                    bool hasPending = false;
                    foreach (var t in Tasks)
                    {
                        if (t.CategoryId == c.Id && !t.Completed)
                        {
                            hasPending = true;
                            break;
                        }
                    }
                    c.IsVisible = hasPending;
                }
                else if (CurrentFilter == "Done")
                {
                    // Show category only if ALL tasks are done AND there's at least one task
                    bool hasAnyTask = false;
                    bool allDone = true;

                    foreach (var t in Tasks)
                    {
                        if (t.CategoryId == c.Id)
                        {
                            hasAnyTask = true;
                            if (!t.Completed)
                            {
                                allDone = false;
                                break;
                            }
                        }
                    }

                    c.IsVisible = hasAnyTask && allDone;
                }
                else
                {
                    // "All" - show category
                    c.IsVisible = true;
                }
            }
        }

        public void UpdateData()
        {
            if (_isUpdating) return;

            try
            {
                _isUpdating = true;

                foreach (var c in Categories)
                {
                    var tasks = new ObservableCollection<MyTask>();
                    int completedCount = 0;
                    int totalCount = 0;
                    int pendingCount = 0;

                    foreach (var t in Tasks)
                    {
                        if (t.CategoryId == c.Id)
                        {
                            tasks.Add(t);
                            totalCount++;
                            if (t.Completed) completedCount++;
                            else pendingCount++;
                        }
                    }

                    c.CategoryTasks = tasks;
                    c.PendingTasks = pendingCount;
                    c.Percentage = totalCount > 0 ? (float)completedCount / totalCount : 0f;
                    c.OnPropertyChanged(nameof(Category.CategoryTasks));
                    c.OnPropertyChanged(nameof(Category.TotalTasks));
                    c.OnPropertyChanged(nameof(Category.PendingTasks));
                    c.OnPropertyChanged(nameof(Category.Percentage));
                }

                foreach (var t in Tasks)
                {
                    foreach (var c in Categories)
                    {
                        if (c.Id == t.CategoryId)
                        {
                            t.TaskColor = c.Color;
                            break;
                        }
                    }
                }
            }
            finally
            {
                _isUpdating = false;
            }

            ApplyFilter();
        }
    }
}