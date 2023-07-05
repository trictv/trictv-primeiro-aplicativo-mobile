using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace trictv
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        


        public async void BtnEntrar(object sender, EventArgs e)
        {
            //if(txtSenha.Text.Length >= 6 && txtUsuario.Text != "")
            //{
            //   DisplayAlert($"{txtUsuario.Text}", "Parabéns vc é gay", "OK");
            //}
            //else
            //{
            //    DisplayAlert("Atenção", "É necessário que o Usuario esteja preenchido e tenha pelo menos 6 dígitos a senha", "OK");
            //}
            //Usuarios usas = new Usuarios();

            // await Navigation.PushModalAsync(new Usuarios());
            await Navigation.PushModalAsync(new NavigationPage(new Usuarios()));

        }
    }
}
