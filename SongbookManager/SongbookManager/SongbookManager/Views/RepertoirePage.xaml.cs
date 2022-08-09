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
    public partial class RepertoirePage : ContentPage
    {
        public RepertoirePage()
        {
            InitializeComponent();

            BindingContext = new RepertoirePageViewModel(Navigation);
        }

        protected async override void OnAppearing()
        {
            var viewModel = (RepertoirePageViewModel)BindingContext;
            await viewModel.LoadingPage();
        }
    }
}