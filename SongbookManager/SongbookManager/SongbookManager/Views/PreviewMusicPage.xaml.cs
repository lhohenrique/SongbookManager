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
    public partial class PreviewMusicPage : TabbedPage
    {
        public PreviewMusicPage(Music music)
        {
            InitializeComponent();

            BindingContext = new PreviewMusicPageViewModel(Navigation, music);
        }

        protected override void OnAppearing()
        {
            var viewModel = (PreviewMusicPageViewModel)BindingContext;
            viewModel.ShowMusicDetailsCommand.Execute(null);
        }
    }
}