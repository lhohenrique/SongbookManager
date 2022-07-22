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
    public partial class MusicPage : ContentPage
    {
        public MusicPage()
        {
            InitializeComponent();

            BindingContext = new MusicPageViewModel(Navigation);
        }

        protected async override void OnAppearing()
        {
            var viewModel = (MusicPageViewModel)BindingContext;
            await viewModel.LoadingPage();
        }

        private void MusicSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var viewModel = (MusicPageViewModel)BindingContext;
            viewModel.SearchCommand.Execute(null);
        }
    }
}