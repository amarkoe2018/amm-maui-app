public partial class GratitudePage : ContentPage
{
    public GratitudePage()
    {
        InitializeComponent();
        BindingContext = new GratitudeListPageModel(new GratitudeRepository(/* logger */));

    }
}