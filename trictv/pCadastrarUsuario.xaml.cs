using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace trictv
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class pCadastrarUsuario : ContentPage
    {
        public pCadastrarUsuario()
        {
            InitializeComponent();
        }

        async void BtnSalvar(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNome.Text) || string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                await DisplayAlert("Atenção", "Todos os campos precisam estar preenchidos, os campos Usuario e Senha não podem ter espaços", "OK");
            }
            else
            {
                AdicionarUsuario();
            }
        }

        private async void AdicionarUsuario()
        {
            await App.MyTabelaLogim.CreateLogim(new Classes.Logim
            {
                Nome = txtNome.Text,
                Usuario = txtUsuario.Text,
                Senha = txtSenha.Text
            });
            await Navigation.PopAsync();
        }
    }
}