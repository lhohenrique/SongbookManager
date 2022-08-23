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
    public partial class DataPage : ContentPage
    {
        public DataPage()
        {
            InitializeComponent();

            BindingContext = new DataPageViewModel();
        }

        protected async override void OnAppearing()
        {
            var viewModel = (DataPageViewModel)BindingContext;
            await viewModel.LoadPageAsync();
        }
    }
}