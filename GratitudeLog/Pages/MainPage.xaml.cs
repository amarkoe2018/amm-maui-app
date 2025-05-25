using GratitudeLog.Models;
using GratitudeLog.PageModels;

namespace GratitudeLog.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}