namespace TorrentZilla.Herramientas
{
    public class VariablesGlobales
    {
        public System.Collections.Generic.List<HtmlAgilityPack.HtmlDocument> _paginas { get; set; }
        public int CantidadPaginas { get; set; }
        public string Url { get; set; }
        public int CategoriaPorDefecto { get; set; }
        public System.Net.Http.HttpClient Cliente { get; set; }
        public HtmlAgilityPack.HtmlDocument DocumentoHTML { get; set; }
        public string NombreRepositorio { get { return "C:/Users/CASA/Desktop/torrentzilla/"; } }
        public string Usuario { get; set; }
        public string Clave { get; set; }
    }

    public class ListaTorrents
    {
        public string NombreAmigable { get; set; }
        public string Direccion { get; set; }
        public System.DateTime Fecha { get; set; }
        public int Categoria { get; set; }
        public string Origen { get; set; }
        public bool Seleccionado { get; set; }
    }

    public class Categorias
    {
        public string Descripcion { get; set; }
        public int Id { get; set; }
    }
}