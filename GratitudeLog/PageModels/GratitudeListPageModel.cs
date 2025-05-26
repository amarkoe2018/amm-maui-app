#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GratitudeLog.Data;
using GratitudeLog.Models;
// using GratitudeLog.Services;
using System.Collections.ObjectModel;


namespace GratitudeLog.PageModels
{
    public partial class GratitudeListPageModel : ObservableObject
    {
        private readonly GratitudeRepository _gratitudeRepository;

        [ObservableProperty]
        private ObservableCollection<GratitudeEntry> gratitudeEntries = new();

        [ObservableProperty]
        private string newEntryText;

        public GratitudeListPageModel(GratitudeRepository gratitudeRepository)
        {
            _gratitudeRepository = gratitudeRepository;
        }

        [RelayCommand]
        private async Task Appearing()
        {
            var items = await _gratitudeRepository.ListAsync();
            GratitudeEntries = new ObservableCollection<GratitudeEntry>(items);
        }

        [RelayCommand]
        private async Task AddGratitude()
        {
            if (string.IsNullOrWhiteSpace(NewEntryText))
                return;
            
            var entry = new GratitudeEntry
            {
                Entry = NewEntryText.Trim()

            };
            
            await _gratitudeRepository.SaveItemAsync(entry);

            GratitudeEntries.Insert(0, entry);
            NewEntryText = string.Empty;
        }
    }
}