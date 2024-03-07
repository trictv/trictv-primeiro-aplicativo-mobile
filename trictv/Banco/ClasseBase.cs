using SQLite;
using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using trictv;

namespace trictv.Banco
{
    public class ClasseBase : INotifyPropertyChanged
    {
        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Declarações
        private bool inEdition;
        private eStatus status;
        private eStatusSincronizacacao sincronizacao;
        private string sincronizacaoDetalhes;
        public enum eOrdenacao { Ascendente = 0, Descendente = 1 }
        public enum eStatus { Nulo = -1, None = 0, Sincronizado = 1, Alterado = 2, Inserido = 3, Excluído = 4, AguardandoSincronização = 5, Sincronizando = 6, ExcluídoERP = 7 };
        public enum eStatusSincronizacacao { None = 0, Concluída = 1, Pendente = 2, Inconsistente = 3 };

        public struct sDadosRetorno
        {
            public int DeviceId;
            public object ChaveERP;
            public bool Sucesso;
            public string Mensagem;
            public eStatus TipoRetorno;
            public int DeviceIdNovo;
        }

        public struct sDadosConsulta
        {
            public int DeviceId { get; set; }
            public object ChaveERP { get; set; }
            public object StatusIntegracao { get; set; }
            public string Mensagem { get; set; }
            public ClasseBase.eStatus StatusObj { get; set; }
            public string MsgObj { get; set; }
        }

        public struct sChaveObjeto
        {
            public int DeviceId { get; set; }
            public string ChaveERP { get; set; }

            public sChaveObjeto(int deviceId, string chaveERP) : this()
            {
                this.DeviceId = deviceId;
                this.ChaveERP = chaveERP;
            }
        }
        #endregion

        #region Propriedades Persistidas
        public eStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                OnPropertyChanged("Status");
                OnPropertyChanged("StatusImagem");
            }
        }

        public eStatusSincronizacacao Sincronizacao
        {
            get
            {
                return sincronizacao;
            }
            set
            {
                sincronizacao = value;
                OnPropertyChanged("Sincronizacao");
                OnPropertyChanged("SincronizacaoImagem");
            }
        }

        public string SincronizacaoDetalhes
        {
            get
            {
                return sincronizacaoDetalhes;
            }
            set
            {
                sincronizacaoDetalhes = value;
                OnPropertyChanged("SincronizacaoDetalhes");
            }
        }

        public DateTime UltimaAlteracao { get; set; }
        #endregion

        #region Propriedades de Apoio
        /// <summary>
        /// Indica se o objeto está em edição.
        /// </summary>
        [Ignore]
        public bool InEdition
        {
            get
            {
                return inEdition;
            }
            set
            {
                inEdition = value;
                OnPropertyChanged("InEdition");
            }
        }

        /// <summary>
        /// Retorna o "source" para binding de imagem do status.
        /// </summary>
        [Ignore]
        public string StatusImagem
        {
            get
            {
                switch (this.Status)
                {
                    case eStatus.Sincronizado:
                        return "status_sincronizado.png";
                    case eStatus.Alterado:
                        return "status_alterado.png";
                    case eStatus.Inserido:
                        return "status_inserido.png";
                    case eStatus.Excluído:
                        return "status_excluido.png";
                    case eStatus.AguardandoSincronização:
                        return "hourglass.png";
                    case eStatus.Sincronizando:
                        return "syncProcess.png";
                    case eStatus.ExcluídoERP:
                        return "status_excluido.png";
                    default:
                        return "";
                }
            }
        }

        /// <summary>
        /// Retorna o "source" para binding de imagem da sincronização.
        /// </summary>
        [Ignore]
        public string SincronizacaoImagem
        {
            get
            {
                switch (this.Sincronizacao)
                {
                    case eStatusSincronizacacao.Inconsistente:
                        return "sync_inconsistente.png";
                    case eStatusSincronizacacao.Pendente:
                        return "sync_pendente.png";
                    //case eStatusSincronizacacao.Concluída:
                    //    return "sync.png";
                    default:
                        return "";
                }
            }
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Retorna o objeto do dispositivo de acordo com a classe e o ID do dispositivo.
        /// </summary>
        /// <typeparam name="T">Classe a ser considerada.</typeparam>
        /// <param name="deviceId">ID do dispositivo.</param>
        /// <returns></returns>
        public static T GetObjeto<T>(int deviceId) where T : new()
        {
            T obj = default(T);
            var tipo = typeof(T);

            SQLiteConnection conn = App.conn;
            conn.CreateTable<T>();
            ObservableCollection<T> objs = new ObservableCollection<T>(conn.Query<T>("SELECT * FROM " + tipo.Name + " WHERE DeviceId = " + deviceId.ToString()));
            if (objs != null)
            {
                if (objs.Count > 0)
                {
                    obj = objs[0];
                }
            }
            return obj;
        }

        /// <summary>
        /// Retorna o ID do dispositivo de acordo com o código/número do objeto.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="chaveERP">Código ou número correspondente à chave do objeto no DataPlus ERP.</param>
        /// <returns></returns>
        public static int GetDeviceId<T>(object chaveERP) where T : new()
        {
            int deviceId = 0;
            var tipo = typeof(T);
            var pkProp = tipo.GetProperty("Codigo");
            if (pkProp == null)
            {
                pkProp = tipo.GetProperty("Numero");
            }

            SQLiteConnection conn = App.conn;
            conn.CreateTable<T>();
            string valor = "";
            if (chaveERP is string)
            {
                valor = "'" + chaveERP + "'";
            }
            else
            {
                valor = chaveERP.ToString();
            }
            ObservableCollection<T> objs = new ObservableCollection<T>(conn.Query<T>("SELECT * FROM " + tipo.Name + " WHERE " + pkProp.Name + " = " + valor));

            if (objs != null && objs.Count > 0)
            {
                if (objs[0] != null)
                {
                    deviceId = Convert.ToInt32(objs[0].GetType().GetProperty("DeviceId").GetValue(objs[0]));
                }
            }
            return deviceId;
        }

        /// <summary>
        /// Retorna uma coleção de determinada classe de objeto.
        /// </summary>
        /// <typeparam name="T">Classe a ser considerada.</typeparam>
        /// <param name="desconsiderarEx">Indica se serão desconsiderados objetos com status excluído.</param>
        /// <returns></returns>
        public static ObservableCollection<T> GetLista<T>(bool desconsiderarEx = false) where T : new()
        {
            return GetLista<T>(eStatus.Nulo, "", desconsiderarEx);
        }

        public static ObservableCollection<T> GetLista<T>(string filtro, bool desconsiderarEx = false) where T : new()
        {
            return GetLista<T>(eStatus.Nulo, filtro, desconsiderarEx);
        }

        public static ObservableCollection<T> GetLista<T>(eStatus status) where T : new()
        {
            return GetLista<T>(status, "");
        }

        public static ObservableCollection<T> GetLista<T>(string filtro) where T : new()
        {
            return GetLista<T>(eStatus.Nulo, filtro);
        }

        private static ObservableCollection<T> GetLista<T>(eStatus status, string filtro, bool desconsiderarEx = false) where T : new()
        {
            var tipo = typeof(T); string filtroAux = "";

            SQLiteConnection conn = App.conn;
            conn.CreateTable<T>();

            ObservableCollection<T> objs;

            if (status != eStatus.Nulo)
            {
                filtroAux += " Status = " + Convert.ToInt32(status).ToString();
            }
            if (filtro != "")
            {
                if (filtroAux != "")
                {
                    filtroAux += " AND ";
                }
                filtroAux += filtro;
            }
            if (desconsiderarEx == true)
            {
                if (filtroAux != "")
                {
                    filtroAux += " AND ";
                }
                filtroAux += " Status <> " + Convert.ToInt32(eStatus.Excluído).ToString();
            }

            objs = new ObservableCollection<T>(conn.Query<T>($"SELECT * FROM {tipo.Name} {(filtroAux != "" ? "WHERE " + filtroAux : "")}"));
            return objs;
        }

        public async static Task<ObservableCollection<T>> FiltrarLista<T>(string paramPesq, string nomeProp, List<T> listaObj) where T : new()
        {
            return await Task.Run(() =>
            {
                var tipo = typeof(T);
                var prop = tipo.GetProperty(nomeProp);

                if (prop != null)
                {
                    ObservableCollection<T> listaNaoNulos = new ObservableCollection<T>(listaObj.Where(x => x.GetType().GetProperty(nomeProp).GetValue(x) != null));

                    return new ObservableCollection<T>(listaNaoNulos.Where(x => x.GetType().GetProperty(nomeProp).GetValue(x).ToString().ToLower().Contains(paramPesq.ToLower())));
                }
                else
                {
                    return new ObservableCollection<T>(listaObj);
                }
            });
        }
        public async static Task<ObservableCollection<T>> GetLista_Async<T>(bool desconsiderarEx = false, string propFiltro = "", eOrdenacao ordem = eOrdenacao.Ascendente) where T : new()
        {
            return await Task.Run(() =>
            {
                var tipo = typeof(T); string filtro = "";

                if (String.IsNullOrEmpty(propFiltro) == false)
                {
                    var prop = tipo.GetProperty(propFiltro);

                    if (prop != null)
                    {
                        filtro = " ORDER BY " + propFiltro + (ordem == eOrdenacao.Ascendente ? " ASC" : " DESC");
                    }
                }

                SQLiteConnection conn = App.conn;
                conn.CreateTable<T>();

                ObservableCollection<T> objs;

                if (desconsiderarEx == false)
                {
                    objs = new ObservableCollection<T>(conn.Query<T>("SELECT * FROM " + tipo.Name + filtro));
                }
                else
                {
                    objs = new ObservableCollection<T>(conn.Query<T>("SELECT * FROM " + tipo.Name + " WHERE Status <> " + Convert.ToInt32(eStatus.Excluído).ToString() + filtro));
                }
                return objs;
            });
        }

        /// <summary>
        /// Retorna o próximo ID do dispositivo de determinada classe.
        /// </summary>
        /// <typeparam name="T">Classe a ser considerada.</typeparam>
        /// <returns></returns>
        public static int NextDeviceId<T>() where T : new()
        {
            int id = 1;
            var tipo = typeof(T);

            SQLiteConnection conn = App.conn;
            conn.CreateTable<T>();
            var obj = conn.Query<T>("SELECT * FROM " + tipo.Name + " ORDER BY DeviceId DESC LIMIT 1");

            if (obj != null)
            {
                if (obj.Count > 0)
                {
                    var objAtual = obj[0];

                    id = Convert.ToInt32(objAtual.GetType().GetProperty("DeviceId").GetValue(objAtual)) + 1;

                    if (id <= 0)
                    {
                        id = 1;
                    }
                }
            }
            else
            {
                
            }

            return id;
        }

        /// <summary>
        /// Verifica se determinado objeto está gravado no dispositivo.
        /// </summary>
        /// <typeparam name="T">Classe do objeto.</typeparam>
        /// <param name="deviceId">ID do dispositivo.</param>
        /// <returns></returns>
        public static bool IsSaved<T>(int deviceId) where T : new()
        {
            SQLiteConnection conn = App.conn;
            conn.CreateTable<T>();
            var obj = conn.Get<T>(deviceId);
            if (obj != null)
            {
                if (Convert.ToInt32(obj.GetType().GetProperty("DeviceId").GetValue(obj)) == deviceId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Grava o objeto atual no dispositivo.
        /// </summary>
        /// <param name="msg">Mensagem de retorno em caso de erro na gravação.</param>
        /// <returns></returns>
        public virtual bool Gravar(ref string msg )
        {
            SQLiteConnection conn = App.conn;
            if (this.Status != eStatus.Inserido)
            {
                if (this.Status == eStatus.None)
                {
                    this.Status = eStatus.Inserido;
                }
                else
                {
                    this.Status = eStatus.Alterado;
                    this.UltimaAlteracao = DateTime.Today;
                }
            }
            this.Sincronizacao = eStatusSincronizacacao.Pendente;
            conn.InsertOrReplace(this);
            conn.Commit();
            return true;
        }

        /// <summary>
        /// Exclui o objeto atual do dispositivo ou altera seu status para excluído.
        /// </summary>
        /// <param name="msg">Mensagem de retorno em caso de erro na exclusão.</param>
        /// <returns></returns>
        public virtual bool Excluir(ref string msg)
        {
            SQLiteConnection conn = App.conn;
            if (this.Status == eStatus.Inserido)
            {
                conn.Delete(this);
            }
            else if (this.Status == eStatus.Excluído)
            {
                msg = "O objeto atual já tem seu status definido como excluído.";
                return false;
            }
            else
            {
                this.Status = eStatus.Excluído;
                this.Sincronizacao = eStatusSincronizacacao.Pendente;
                conn.Update(this);
            }
            conn.Commit();
            return true;
        }

        /// <summary>
        /// Retorna o XML para envio de consulta de status.
        /// </summary>
        /// <param name="chavesObj">Lista de chaves para envio.</param>
        /// <returns></returns>
        public static string MontarStringStatus(List<sChaveObjeto> chavesObj)
        {
            if (chavesObj?.Count > 0)
            {
                string xmlString = "<ObjetosStatus>";
                foreach (sChaveObjeto chave in chavesObj)
                {
                    xmlString += "<ObjetoStatus>";
                    xmlString += "<DeviceId>" + chave.DeviceId.ToString() + "</DeviceId>";
                    xmlString += "<ChaveERP>" + chave.ChaveERP.ToString() + "</ChaveERP>";
                    xmlString += "</ObjetoStatus>";
                }

                xmlString += "</ObjetosStatus>";
                return xmlString;
            }
            else
            {
                return "";
            }
        }
        #endregion
    }
}