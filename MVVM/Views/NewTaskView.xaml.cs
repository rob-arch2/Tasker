using Tasker.MVVM.Models;
using Tasker.MVVM.ViewModels;

namespace Tasker.MVVM.Views;

public partial class NewTaskView : ContentPage
{
    private NewTaskViewModel viewModel;

    public NewTaskView()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel = BindingContext as NewTaskViewModel;
    }

    private void OnAddSubtaskClicked(object sender, EventArgs e)
    {
        viewModel?.AddSubtask();
    }

    private void OnDeleteSubtaskClicked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is Subtask subtask)
        {
            viewModel?.RemoveSubtask(subtask);
        }
    }

    private async void OnCreateTaskClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(viewModel?.Task))
        {
            await DisplayAlert("Error", "Please enter a task name", "OK");
            return;
        }

        if (viewModel?.SelectedCategory == null)
        {
            await DisplayAlert("Error", "Please select a category", "OK");
            return;
        }

        viewModel.CreateTask();
        await DisplayAlert("Success", "Task created successfully!", "OK");
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        bool hasData = !string.IsNullOrWhiteSpace(viewModel?.Task) ||
                       viewModel?.Subtasks?.Count > 0;

        if (hasData)
        {
            bool shouldCancel = await DisplayAlert(
                "Confirm",
                "Discard this task?",
                "Yes",
                "No");

            if (!shouldCancel)
                return;
        }

        await Navigation.PopAsync();
    }
}