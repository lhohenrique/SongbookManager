using SongbookManager.Models;
using SongbookManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace SongbookManager.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddEditRepertoirePage : ContentPage
	{
		public AddEditRepertoirePage (Repertoire repertoire)
		{
			InitializeComponent ();

			BindingContext = new AddEditRepertoirePageViewModel(Navigation, repertoire);
		}

		protected async override void OnAppearing()
		{
			var viewModel = (AddEditRepertoirePageViewModel)BindingContext;
			await viewModel.PopulateRepertoireFieldsAsync();
		}

        private void MusicSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
			var viewModel = (AddEditRepertoirePageViewModel)BindingContext;
			viewModel.SearchCommand.Execute(null);
		}

        private void MusicListListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
			var viewModel = (AddEditRepertoirePageViewModel)BindingContext;

			MusicRep musicTapped = (MusicRep)e.Item;
			int musicTappedIndex = e.ItemIndex;

			viewModel.SelectionChangedAction(musicTapped, musicTappedIndex);
		}
    }
}