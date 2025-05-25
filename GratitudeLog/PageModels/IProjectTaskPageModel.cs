using CommunityToolkit.Mvvm.Input;
using GratitudeLog.Models;

namespace GratitudeLog.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}