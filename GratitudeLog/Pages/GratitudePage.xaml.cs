namespace GratitudeLog.Pages;
public partial class GratitudePage : ContentPage
{
    public GratitudePage()
    {
        InitializeComponent();
        //BindingContext = App.Current.Services.GetRequiredService<GratitudeListPageModel>();
        BindingContext = MauiProgram.Services.GetRequiredService<GratitudeListPageModel>();

    }
}