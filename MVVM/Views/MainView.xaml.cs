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

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            var deadlineDate = await DisplayPromptAsync(
                "Deadline",
                "Enter deadline (e.g., 'Today 5:30PM' or 'Tomorrow 9:00AM')",
                placeholder: "Today 5:30PM",
                keyboard: Keyboard.Text);

            DateTime deadline = DateTime.Today.AddHours(23).AddMinutes(59);

            if (!string.IsNullOrWhiteSpace(deadlineDate) &&
                deadlineDate.Contains("tomorrow", StringComparison.OrdinalIgnoreCase))
            {
                deadline = DateTime.Today.AddDays(1).AddHours(12);
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
    }
}