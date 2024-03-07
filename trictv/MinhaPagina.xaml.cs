using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace trictv
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MinhaPagina : ContentPage
    {
        public MinhaPagina()
        {
            InitializeComponent();
        }

        private async void BotaoOpcao1_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new pCadastrarUsuario()));
        }

        private async void BotaoOpcao2_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new Usuarios()));
        }

        private async void BotaoOpcao3_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new pNotificacao()));
        }

        private async void BotaoOpcao4_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Opção 4", "Você clicou na Opção 4!", "OK");
        }
    }
}
