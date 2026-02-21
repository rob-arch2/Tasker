using Tasker.MVVM.Models;
using Tasker.MVVM.ViewModels;

namespace Tasker.MVVM.Views;

public partial class MainView : ContentPage
{
    private readonly MainViewModel mainViewModel = new MainViewModel();

    public MainView()
    {
        InitializeComponent();
        BindingContext = mainViewModel;
    }

    private void checkBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        mainViewModel.UpdateData();
    }

    private void OnCategoryTapped(object sender, EventArgs e)
    {
        var grid = sender as Grid;
        if (grid?.BindingContext is Category category)
        {
            category.IsExpanded = !category.IsExpanded;
            mainViewModel.SelectCategory(category);
        }
    }

    private async void CreateTaskForCategoryClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.CommandParameter is Category category)
        {
            string taskName = await DisplayPromptAsync(
                "New Task",
                $"Enter task name for {category.CategoryName}",
                placeholder: "Task name",
                maxLength: 100,
                keyboard: Keyboard.Text);

            if (!string.IsNullOrWhiteSpace(taskName))
            {
                var newTask = new MyTask
                {
                    TaskName = taskName,
                    Completed = false,
                    CategoryId = category.Id
                };
                mainViewModel.Tasks.Add(newTask);
            }
        }
    }

    private async void AddCategoryClicked(object sender, EventArgs e)
    {
        string categoryName = await DisplayPromptAsync(
            "New Category",
            "Enter category name",
            placeholder: "Category name",
            maxLength: 30,
            keyboard: Keyboard.Text);

        if (string.IsNullOrWhiteSpace(categoryName)) return;

        // Ask for deadline when creating category
        string deadlineInput = await DisplayPromptAsync(
            "Deadline",
            "Enter deadline date (e.g. 12/31/2025)",
            placeholder: "MM/dd/yyyy",
            keyboard: Keyboard.Text);

        DateTime deadline = DateTime.Today.AddHours(23).AddMinutes(59);

        if (!string.IsNullOrWhiteSpace(deadlineInput))
        {
            if (DateTime.TryParse(deadlineInput, out DateTime parsed))
                deadline = parsed;
        }

        var random = new Random();
        var newCategory = new Category
        {
            Id = mainViewModel.Categories.Count > 0
                ? mainViewModel.Categories.Max(c => c.Id) + 1
                : 1,
            CategoryName = categoryName,
            Color = Color.FromRgb(
                random.Next(0, 255),
                random.Next(0, 255),
                random.Next(0, 255)).ToHex(),
            Deadline = deadline,
            IsExpanded = false
        };

        mainViewModel.Categories.Add(newCategory);
        mainViewModel.UpdateData();
    }

    private async void AddTaskClicked(object sender, EventArgs e)
    {
        if (!mainViewModel.Categories.Any())
        {
            await DisplayAlert("No Categories", "Please add a category first.", "OK");
            return;
        }

        string taskName = await DisplayPromptAsync(
            "New Task",
            "Enter task name",
            placeholder: "Task name",
            maxLength: 100,
            keyboard: Keyboard.Text);

        if (string.IsNullOrWhiteSpace(taskName)) return;

        var categoryNames = mainViewModel.Categories.Select(c => c.CategoryName).ToArray();
        string chosen = await DisplayActionSheet("Select Category", "Cancel", null, categoryNames);

        if (string.IsNullOrEmpty(chosen) || chosen == "Cancel") return;

        var target = mainViewModel.Categories.FirstOrDefault(c => c.CategoryName == chosen);
        if (target == null) return;

        // No deadline prompt for tasks
        mainViewModel.Tasks.Add(new MyTask
        {
            TaskName = taskName,
            Completed = false,
            CategoryId = target.Id,
            TaskColor = target.Color
        });
    }

    private async void OnEditTaskTapped(object sender, EventArgs e)
    {
        if (sender is not Label label) return;
        if (label.BindingContext is not MyTask task) return;

        string newName = await DisplayPromptAsync(
            "Edit Task",
            "Enter new task name",
            initialValue: task.TaskName,
            placeholder: "Task name",
            maxLength: 100,
            keyboard: Keyboard.Text);

        if (string.IsNullOrWhiteSpace(newName)) return;
        if (newName == task.TaskName) return;

        task.TaskName = newName;
        mainViewModel.UpdateData();
    }

    private async void OnEditCategoryTapped(object sender, EventArgs e)
    {
        if (sender is not Label label) return;
        if (label.BindingContext is not Category category) return;

        string newName = await DisplayPromptAsync(
            "Edit Category",
            "Enter new category name",
            initialValue: category.CategoryName,
            placeholder: "Category name",
            maxLength: 30,
            keyboard: Keyboard.Text);

        if (!string.IsNullOrWhiteSpace(newName) && newName != category.CategoryName)
            category.CategoryName = newName;
    }

    private void OnFilterClicked(object sender, EventArgs e)
    {
        if (sender is not Button tapped) return;

        string filter = tapped.CommandParameter?.ToString() ?? "All";
        mainViewModel.SetFilter(filter);

        if (tapped.Parent is HorizontalStackLayout hsl)
        {
            foreach (var child in hsl.Children)
            {
                if (child is not Button btn) continue;

                bool isActive = btn.CommandParameter?.ToString() == filter;

                btn.BackgroundColor = isActive
                    ? filter switch
                    {
                        "Pending" => Color.FromArgb("#FF6B35"),
                        "Done" => Color.FromArgb("#2ECC71"),
                        _ => Colors.Black
                    }
                    : Colors.White;

                btn.TextColor = isActive ? Colors.White : Color.FromArgb("#555555");
                btn.BorderColor = isActive ? Colors.Transparent : Color.FromArgb("#E0E0E0");
            }
        }
    }
}