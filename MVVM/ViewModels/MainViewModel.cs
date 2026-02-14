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
     public class MainViewModel
     {
          public ObservableCollection<Category> Categories { get; set; }
          public ObservableCollection<MyTask> Tasks { get; set; }

          public MainViewModel()
          {
               FillData();
               Tasks.CollectionChanged += Tasks_CollectionChanged;
          }

          private void Tasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
          {
               UpdateData();
          }

        private void FillData()
        {
            Categories = new ObservableCollection<Category>
               {
                    new Category
                    {
                         Id = 1,
                         CategoryName = ".NET MAUI Course",
                         Color = "#CF14DF",
                         Deadline = DateTime.Today.AddHours(14).AddMinutes(30), // Today 2:30PM
                         IsExpanded = false
                    },
                    new Category
                    {
                         Id = 2,
                         CategoryName = "Tutorials",
                         Color = "#df6f14",
                         Deadline = DateTime.Today.AddHours(16), // Today 4:00PM
                         IsExpanded = false
                    },
                    new Category
                    {
                         Id = 3,
                         CategoryName = "Shopping",
                         Color = "#14df80",
                         Deadline = DateTime.Today.AddHours(16).AddMinutes(45), // Today 4:45PM
                         IsExpanded = false
                    },
                    new Category
                    {
                         Id = 4,
                         CategoryName = "Skincare",
                         Color = "#FF6B9D",
                         Deadline = DateTime.Today.AddHours(19).AddMinutes(30), // Today 7:30PM
                         IsExpanded = false
                    },
                    new Category
                    {
                         Id = 5,
                         CategoryName = "Pray",
                         Color = "#4A90E2",
                         Deadline = DateTime.Today.AddHours(22).AddMinutes(30), // Today 10:30PM
                         IsExpanded = false
                    },
                    new Category
                    {
                         Id = 6,
                         CategoryName = "Cook Breakfast",
                         Color = "#F39C12",
                         Deadline = DateTime.Today.AddDays(1).AddHours(9).AddMinutes(30), // Tomorrow 9:30AM
                         IsExpanded = false
                    },
                    new Category
                    {
                         Id = 7,
                         CategoryName = "Study",
                         Color = "#9B59B6",
                         Deadline = DateTime.Today.AddDays(1).AddHours(17).AddMinutes(30), // Tomorrow 5:30AM
                         IsExpanded = false
                    }
               };

            Tasks = new ObservableCollection<MyTask>
               {
                    new MyTask
                    {
                         TaskName = "Upload exercise files",
                         Completed = false,
                         CategoryId = 1
                    },
                    new MyTask
                    {
                         TaskName = "Upload exercise files",
                         Completed = false,
                         CategoryId = 1
                    },
                    new MyTask
                    {
                         TaskName = "Update github repository",
                         Completed = false,
                         CategoryId = 2
                    },
                    new MyTask
                    {
                         TaskName = "Fix Settings.cs class of the project",
                         Completed = true,
                         CategoryId = 2
                    },
                    new MyTask
                    {
                         TaskName = "Buy eggs",
                         Completed = false,
                         CategoryId = 3
                    },
                    new MyTask
                    {
                         TaskName = "Go for the pepperoni pizza",
                         Completed = false,
                         CategoryId = 3
                    },
                    new MyTask
                    {
                         TaskName = "Morning skincare routine",
                         Completed = false,
                         CategoryId = 4
                    },
                    new MyTask
                    {
                         TaskName = "Apply sunscreen",
                         Completed = false,
                         CategoryId = 4
                    },
                    new MyTask
                    {
                         TaskName = "Fajr prayer",
                         Completed = false,
                         CategoryId = 5
                    },
                    new MyTask
                    {
                         TaskName = "Maghrib prayer",
                         Completed = false,
                         CategoryId = 5
                    },
                    new MyTask
                    {
                         TaskName = "Prepare ingredients",
                         Completed = false,
                         CategoryId = 6
                    },
                    new MyTask
                    {
                         TaskName = "Cook eggs and bacon",
                         Completed = false,
                         CategoryId = 6
                    },
                    new MyTask
                    {
                         TaskName = "Review Chapter 5",
                         Completed = false,
                         CategoryId = 7
                    },
                    new MyTask
                    {
                         TaskName = "Complete practice problems",
                         Completed = false,
                         CategoryId = 7
                    },
               };

            UpdateData();
        }

        public void UpdateData()
        {
            foreach (var c in Categories)
            {
                var tasks = from t in Tasks
                            where t.CategoryId == c.Id
                            select t;

                var completed = from t in tasks
                                where t.Completed == true
                                select t;

                var notCompleted = from t in tasks
                                   where t.Completed == false
                                   select t;

                c.PendingTasks = notCompleted.Count();
                c.TotalTasks = tasks.Count();
                c.Percentage = tasks.Count() > 0 ? (float)completed.Count() / (float)tasks.Count() : 0;

                // Update CategoryTasks collection
                c.CategoryTasks.Clear();
                foreach (var task in tasks)
                {
                    c.CategoryTasks.Add(task);
                }
            }
            foreach (var t in Tasks)
            {
                var catColor =
                     (from c in Categories
                      where c.Id == t.CategoryId
                      select c.Color).FirstOrDefault();
                t.TaskColor = catColor;
            }
        }
    }
}
