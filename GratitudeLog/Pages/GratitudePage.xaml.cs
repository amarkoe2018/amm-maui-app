using System.Collections.ObjectModel;

namespace GratitudeLog.Pages;

public partial class GratitudePage : ContentPage
{
    ObservableCollection<string> gratitudeItems = new();

    public GratitudePage()
    {
        InitializeComponent();
        GratitudeList.ItemsSource = gratitudeItems;
    }

    private void OnAddClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(GratitudeEntry.Text))
        {
            gratitudeItems.Insert(0, GratitudeEntry.Text.Trim());
            GratitudeEntry.Text = string.Empty;
        }
    }
}
