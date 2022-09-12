using SongbookManager.Models;
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
    public partial class AdminPage : TabbedPage
    {
        public AdminPage()
        {
            InitializeComponent();

            BindingContext = new AdminPageViewModel(Navigation);
        }

        protected async override void OnAppearing()
        {
            var viewModel = (AdminPageViewModel)BindingContext;
            await viewModel.LoadingPageAsync();
        }

        private async void UserMenuItem_Clicked(object sender, EventArgs e)
        {
            var user = ((MenuItem)sender).BindingContext as User;
            if(user == null)
            {
                return;
            }
            
            var viewModel = (AdminPageViewModel)BindingContext;
            await viewModel.RemoveUserAction(user);
        }

        private async void MusicMenuItem_Clicked(object sender, EventArgs e)
        {
            var music = ((MenuItem)sender).BindingContext as Music;
            if (music == null)
            {
                return;
            }

            var viewModel = (AdminPageViewModel)BindingContext;
            await viewModel.RemoveMusicAction(music);
        }
    }
}