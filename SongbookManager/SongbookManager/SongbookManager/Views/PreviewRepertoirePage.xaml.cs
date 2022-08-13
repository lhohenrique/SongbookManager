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
    public partial class PreviewRepertoirePage : ContentPage
    {
        public PreviewRepertoirePage(Repertoire repertoire)
        {
            InitializeComponent();

            BindingContext = new PreviewRepertoirePageViewModel(Navigation, repertoire);
        }

        protected override void OnAppearing()
        {
            var viewModel = (PreviewRepertoirePageViewModel)BindingContext;
            viewModel.LoadPage();
        }
    }
}