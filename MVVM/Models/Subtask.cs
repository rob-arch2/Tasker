using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasker.MVVM.Models
{
    /// <summary>
    /// Subtask model - represents a sub-item within a task
    /// Place this file in: MVVM/Models/Subtask.cs
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class Subtask
    {
        public string TaskName { get; set; }
        public bool Completed { get; set; }
    }
}