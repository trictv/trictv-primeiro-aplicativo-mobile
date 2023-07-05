using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using trictv.Classes;
using trictv.Banco;

namespace trictv
{
    public partial class App : Application
    {
        private static BdLogim db;
        public static BdLogim MyTabelaLogim
        {
            get
            {
                if (db == null)
                {
                    db = new BdLogim(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Trictv.db3"));
                }
                return db;
            }
        }
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

        }
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
