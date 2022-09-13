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

        private async void RepertoireMusicsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var viewModel = (PreviewRepertoirePageViewModel)BindingContext;

            MusicRep musicTapped = (MusicRep)e.Item;
            int musicTappedIndex = e.ItemIndex;

            await viewModel.SelectionChangedAction(musicTapped, musicTappedIndex);
        }
    }
}