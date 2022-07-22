using SongbookManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SongbookManager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();

            BindingContext = new ProfilePageViewModel(Navigation);
        }

        protected override void OnAppearing()
        {
            var viewModel = (ProfilePageViewModel)BindingContext;
            viewModel.OnAppearing();
        }
    }
}