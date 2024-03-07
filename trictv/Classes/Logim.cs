using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using trictv.Banco;

namespace trictv.Classes
{
    public class Logim : ClasseBase
    {

        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Nome { get; set; }

        [MaxLength(100)]
        public string Usuario { get; set; }

        public string Senha { get; set; }
    }
}
