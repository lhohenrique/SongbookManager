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
    public partial class PlayRepertoirePage : TabbedPage
    {
        public PlayRepertoirePage(Repertoire repertoire)
        {
            InitializeComponent();

            BindingContext = new PlayRepertoirePageViewModel(Navigation, repertoire);
        }

        protected async override void OnAppearing()
        {
            var viewModel = (PlayRepertoirePageViewModel)BindingContext;
            await viewModel.LoadPageAsync();
        }
    }
}