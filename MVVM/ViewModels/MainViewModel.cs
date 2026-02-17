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
            Categories.Add(new Category { Id = 1, CategoryName = ".NET MAUI Course", Color = "#CF14DF" });
            Categories.Add(new Category { Id = 2, CategoryName = "Tutorials", Color = "#df6f14" });
            Categories.Add(new Category { Id = 3, CategoryName = "Shopping", Color = "#14df80" });

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
                // Hide category if another one is selected
                if (SelectedCategory != null && SelectedCategory != c)
                {
                    c.IsVisible = false;
                    continue;
                }

                c.IsVisible = true;

                // Rebuild CategoryTasks based on filter
                var filtered = new ObservableCollection<MyTask>();
                foreach (var t in Tasks)
                {
                    if (t.CategoryId != c.Id) continue;
                    if (CurrentFilter == "Pending" && t.Completed) continue;
                    if (CurrentFilter == "Done" && !t.Completed) continue;
                    filtered.Add(t);
                }

                c.CategoryTasks = filtered;
                c.OnPropertyChanged(nameof(Category.CategoryTasks));
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
                        if (c.Id == t.CategoryId) { t.TaskColor = c.Color; break; }
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