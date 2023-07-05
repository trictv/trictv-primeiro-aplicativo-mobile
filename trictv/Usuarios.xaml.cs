using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trictv.Classes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace trictv
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Usuarios : ContentPage
    {
        public Usuarios()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                List<Logim> list = await App.MyTabelaLogim.Logims();
                MyUsuarios.ItemsSource = await App.MyTabelaLogim.Logims();
                
            }
            catch { }
        }

        async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new pCadastrarUsuario());

        }
    }
}