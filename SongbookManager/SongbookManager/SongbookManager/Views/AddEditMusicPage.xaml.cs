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
    public partial class AddEditMusicPage : TabbedPage
    {
        public AddEditMusicPage(Music music)
        {
            InitializeComponent();

            BindingContext = new AddEditMusicPageViewModel(Navigation, music);
        }

        protected async override void OnAppearing()
        {
            var viewModel = (AddEditMusicPageViewModel)BindingContext;
            await viewModel.PopulateMusicFieldsAsync();
        }
    }
}