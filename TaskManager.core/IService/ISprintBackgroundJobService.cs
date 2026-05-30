using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Core.IService
{
    public interface ISprintBackgroundJobService
    {
        Task CloseEndedSprintsAsync();

    }
}
