using SongbookManager.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SongbookManager
{
    public partial class MainPage : Shell
    {
        public MainPage()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RegisterPage),
                typeof(RegisterPage));
            Routing.RegisterRoute(nameof(ForgotPasswordPage),
                typeof(ForgotPasswordPage));
        }
    }
}
