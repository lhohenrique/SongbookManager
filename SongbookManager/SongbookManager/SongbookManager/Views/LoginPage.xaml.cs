using SongbookManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SongbookManager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            BindingContext = new LoginPageViewModel();
        }

        protected override void OnAppearing()
        {
            var viewModel = (LoginPageViewModel)BindingContext;
            viewModel.OnAppearingAsync();
        }
    }
}