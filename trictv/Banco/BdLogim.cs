using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using trictv.Classes;

namespace trictv.Banco
{
    public class BdLogim
    {
        private readonly SQLiteAsyncConnection db;

        public BdLogim(string nome)
        {
            db = new SQLiteAsyncConnection(nome);
            db.CreateTableAsync<Logim>();
        }
        public Task<int> CreateLogim (Logim logim)
        {
            return db.InsertAsync(logim);
        }
        public Task<List<Logim>> Logims()
        {
            return db.Table<Logim>().ToListAsync();
        }
        public Task<int> UpdaetLogim (Logim logim)
        {
            return db.UpdateAsync(logim);
        }
        public Task<int> DeleteLogim (Logim logim)
        {
            return db.DeleteAsync(logim);
        }
    }
}
