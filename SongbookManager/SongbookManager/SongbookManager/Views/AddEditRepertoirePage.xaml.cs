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

        private void MusicListCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			var viewModel = (AddEditRepertoirePageViewModel)BindingContext;
			if (viewModel != null)
			{
				CollectionView collection = (CollectionView)sender;
				List<Music> musicsSelected = new List<Music>();

				foreach (var music in collection.SelectedItems)
				{
					musicsSelected.Add((Music)music);
				}

				viewModel.SelectionChangedAction(musicsSelected);
			}
		}

        private void MusicSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
			var viewModel = (AddEditRepertoirePageViewModel)BindingContext;
			viewModel.SearchCommand.Execute(null);
		}
    }
}