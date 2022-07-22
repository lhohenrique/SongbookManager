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
    public partial class SharePage : ContentPage
    {
        public SharePage()
        {
            InitializeComponent();

            BindingContext = new SharePageViewModel(Navigation);
        }

        protected override async void OnAppearing()
        {
            var viewModel = (SharePageViewModel)BindingContext;
            await viewModel.HandlePageState();
        }
    }
}