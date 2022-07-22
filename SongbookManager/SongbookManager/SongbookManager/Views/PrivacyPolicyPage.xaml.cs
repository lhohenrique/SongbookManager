using SongbookManager.Helpers;
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
    public partial class PrivacyPolicyPage : ContentPage
    {
        public PrivacyPolicyPage()
        {
            InitializeComponent();

            var htmlSource = new HtmlWebViewSource();
            htmlSource.Html = GlobalVariables.PrivacyPolicy;
            PrivacyPolicyWebView.Source = htmlSource;
        }
    }
}