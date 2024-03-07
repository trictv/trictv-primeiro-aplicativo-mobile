using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using trictv.Classes;
using trictv.Banco;
using SQLite;

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
        private static SQLiteConnection _conn;
        public static SQLiteConnection conn
        {
            get
            {
                if(_conn == null)
                {
                    _conn = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Trictv.db3"));
                }
                return _conn;
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
