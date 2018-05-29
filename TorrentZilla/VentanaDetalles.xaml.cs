using System.Linq;
namespace TorrentZilla
{
    public partial class VentanaDetalles : MahApps.Metro.Controls.MetroWindow
    {
        public VentanaDetalles(string _url)
        {
            InitializeComponent();
            CargaDetalles(_url);
        }

        private void LimpiaElementos()
        {
            try
            {
                lblNombreElemento.Content = string.Empty;

            }
            catch (System.Exception exc)
            {
                throw;
            }
        }
        private async void CargaDetalles(string _url)
        {
            try
            {
                LimpiaElementos();
                System.Net.Http.HttpClient _cliente = new System.Net.Http.HttpClient();
                var _respuesta = await _cliente.GetByteArrayAsync(_url);
                System.String source = System.Text.Encoding.GetEncoding("utf-8").GetString(_respuesta, 0, _respuesta.Length - 1);
                source = System.Net.WebUtility.HtmlDecode(source);
                HtmlAgilityPack.HtmlDocument _doc = new HtmlAgilityPack.HtmlDocument();
                _doc.LoadHtml(source);

                #region datos del elemento
                var _titulo = _doc.GetElementbyId("title");
                if (_titulo != null)
                    if (_titulo.ChildNodes.Count > 0)
                    {
                        var _texto = _titulo.ChildNodes.FirstOrDefault(x => x.Name == "#text");
                        lblNombreElemento.Content = _texto != null ? System.Text.RegularExpressions.Regex.Replace(_texto.InnerText, @"([^a-zA-Z0-9_]|^\s)", " ") : string.Empty;
                    }
                #endregion  
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }
    }
}