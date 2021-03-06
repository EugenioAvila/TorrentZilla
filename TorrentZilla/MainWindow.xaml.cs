﻿using System.Linq;
namespace TorrentZilla
{
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        short _modo = (short)Herramientas.Enumeradores.eModosBusqueda.SIN_DEFINIR;
        System.Collections.Generic.List<Herramientas.ListaTorrents> _lista = new System.Collections.Generic.List<Herramientas.ListaTorrents>();
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                InicializaControles();
                animBuscando.Visibility = System.Windows.Visibility.Collapsed;
                txtBuscando.Text = "";
            }
            catch (System.Exception exc)
            {
                System.Windows.MessageBox.Show(exc.Message);
            }
        }
        
        public void InicializaControles()
        {
            try
            {
                System.Collections.Generic.List<Herramientas.Categorias> _categorias = new System.Collections.Generic.List<Herramientas.Categorias>();
                GetEnumList<Herramientas.Enumeradores.eCategorias>().ForEach(z =>
                {
                    _categorias.Add(new Herramientas.Categorias()
                    {
                        Id = z.Value,
                        Descripcion = z.Key.Replace('_', ' ')
                    });
                });

                ComboCategoria.DisplayMemberPath = "Descripcion";
                ComboCategoria.ItemsSource = _categorias;
                ComboCategoria.SelectedIndex = 0;

                ComboCategoria.IsEnabled = false;
                txtPalabrasClave.IsEnabled = false;
            }
            catch (System.Exception exc)
            {
                System.Windows.MessageBox.Show(exc.Message);
            }
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public static System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, int>> GetEnumList<T>()
        {
            var list = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, int>>();
            foreach (var e in System.Enum.GetValues(typeof(T)))
            {
                list.Add(new System.Collections.Generic.KeyValuePair<string, int>(e.ToString(), (int)e));
            }
            return list;
        }

        public async void Buscar()
        {
            try
            {
                if (_modo == (short)Herramientas.Enumeradores.eModosBusqueda.SIN_DEFINIR)
                {
                    animBuscando.Visibility = System.Windows.Visibility.Collapsed;
                    txtBuscando.Text = "SELECCIONE EL MODO DE BUSQUEDA";
                    return;
                }

                GridResultados.ItemsSource = new System.Collections.Generic.List<Herramientas.ListaTorrents>();
                animBuscando.Visibility = System.Windows.Visibility.Visible;
                txtBuscando.Text = "BUSCANDO";
                var _categoriaElegida = (Herramientas.Categorias)ComboCategoria.SelectedItem;
                Herramientas.VariablesGlobales variablesGlobales = new Herramientas.VariablesGlobales()
                {
                    CantidadPaginas = string.IsNullOrEmpty(txtCantidadPaginas.Text) ? System.Convert.ToInt32(Herramientas.Enumeradores.eDatosPorDefault.PAGINAS_POR_DEFECTO) : System.Convert.ToInt32(txtCantidadPaginas.Text),
                    Url = "https://thepiratebay.org/browse/",
                    CategoriaPorDefecto = _categoriaElegida != null ? System.Convert.ToInt32(_categoriaElegida.Id) : System.Convert.ToInt32(Herramientas.Enumeradores.eDatosPorDefault.CATEGORIA_POR_DEFECTO)
                };

                _lista = new System.Collections.Generic.List<Herramientas.ListaTorrents>();
                string _claves = !string.IsNullOrEmpty(txtPalabrasClave.Text) ? txtPalabrasClave.Text.Replace(" ", "%20") : string.Empty;
                System.DateTime _fecha = System.DateTime.Now;
                string _url = string.Empty;
                if (variablesGlobales.CantidadPaginas > 0)
                    for (int i = 0; i < variablesGlobales.CantidadPaginas; i++)
                    {
                        if (_modo == (short)Herramientas.Enumeradores.eModosBusqueda.POR_CATEGORIA)
                            _url = variablesGlobales.Url + System.Convert.ToInt32(_categoriaElegida.Id) + "/" + i + "/3";
                        if (_modo == (short)Herramientas.Enumeradores.eModosBusqueda.POR_PALABRA_CLAVE)
                            _url = "https://thepiratebay.org/search/" + _claves + "/0/99/0";

                        if (string.IsNullOrEmpty(_url))
                            _url = variablesGlobales.Url;

                        variablesGlobales.Cliente = new System.Net.Http.HttpClient();
                        var _respuesta = await variablesGlobales.Cliente.GetByteArrayAsync(_url);
                        System.String source = System.Text.Encoding.GetEncoding("utf-8").GetString(_respuesta, 0, _respuesta.Length - 1);
                        source = System.Net.WebUtility.HtmlDecode(source);
                        variablesGlobales.DocumentoHTML = new HtmlAgilityPack.HtmlDocument();
                        variablesGlobales.DocumentoHTML.LoadHtml(source);
                        txtBuscando.Text = "DECODIFICANDO";
                        string _urlBase = string.Empty;
                        foreach (HtmlAgilityPack.HtmlNode link in variablesGlobales.DocumentoHTML.DocumentNode.SelectNodes("//a[@href]"))
                        {
                            HtmlAgilityPack.HtmlAttribute att = link.Attributes["href"];
                            if (!string.IsNullOrEmpty(att.Value))
                            {
                                if (att.Value.StartsWith("/torrent/"))
                                    _urlBase = "https://thepiratebay.org" + att.Value.Trim();

                                if (att.Value.StartsWith("magnet:?"))
                                {
                                    var _elementos = att.Value.Split('&');
                                    string encodedString = System.Web.HttpUtility.HtmlEncode(_elementos.FirstOrDefault(x => x.StartsWith("dn="))).Replace('+', ' ').Remove(0, 3);
                                    string _nombreL = System.Text.RegularExpressions.Regex.Replace(encodedString, @"([^a-zA-Z0-9_]|^\s)", " ");
                                    _lista.Add(new Herramientas.ListaTorrents()
                                    {
                                        Categoria = System.Convert.ToInt32(Herramientas.Enumeradores.eDatosPorDefault.CATEGORIA_POR_DEFECTO),
                                        Direccion = att.Value,
                                        Fecha = _fecha,
                                        NombreAmigable = _nombreL,
                                        Seleccionado = false,
                                        Origen = _urlBase
                                    });
                                }
                            }
                        }
                    }

                animBuscando.Visibility = System.Windows.Visibility.Collapsed;
                txtBuscando.Text = "TERMINADO, SE HAN HALLADO " + _lista.Count + " ELEMENTOS";
                GridResultados.ItemsSource = _lista != null ? _lista.Any() ? _lista.Where(x => x.NombreAmigable != "").OrderBy(y => y.NombreAmigable).ToList() : new System.Collections.Generic.List<Herramientas.ListaTorrents>() : new System.Collections.Generic.List<Herramientas.ListaTorrents>();
            }
            catch (System.Exception exc)
            {
                animBuscando.Visibility = System.Windows.Visibility.Collapsed;
                txtBuscando.Text = "SURGIO UNA EXCEPCION: " + exc.Message;
            }
        }

        private void ComboBuscarPor_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                string text = ((sender as System.Windows.Controls.ComboBox).SelectedItem as System.Windows.Controls.ComboBoxItem).Content as string;
                if (!string.IsNullOrEmpty(text))
                    switch (text)
                    {
                        case "Por Categoria":
                            ComboCategoria.IsEnabled = true;
                            txtPalabrasClave.IsEnabled = false;
                            txtCantidadPaginas.IsEnabled = true;
                            _modo = (short)Herramientas.Enumeradores.eModosBusqueda.POR_CATEGORIA;
                            break;
                        case "Por Palabra Clave":
                            txtPalabrasClave.IsEnabled = true;
                            ComboCategoria.IsEnabled = false;
                            txtCantidadPaginas.IsEnabled = false;
                            _modo = (short)Herramientas.Enumeradores.eModosBusqueda.POR_PALABRA_CLAVE;
                            break;
                    }
            }
            catch (System.Exception exc)
            {
                System.Windows.MessageBox.Show(exc.Message);
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Buscar();
        }

        private void Copy(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            try
            {
                var _origen = (System.Windows.Controls.DataGrid)e.Source;
                if (_origen.SelectedItem != null)
                {
                    var _selected = (Herramientas.ListaTorrents)_origen.SelectedItem;
                    if (_selected != null)
                        System.Windows.Clipboard.SetData(System.Windows.DataFormats.Text, _selected.Direccion);
                }
            }
            catch (System.Exception exc)
            {
                System.Windows.MessageBox.Show(exc.Message);
            }
        }

        private void MenuItem_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var aa = (System.Windows.Controls.MenuItem)sender;
                string _direccion = string.Empty;
                var _selected = (Herramientas.ListaTorrents)GridResultados.SelectedItem;
                if (_selected != null)
                    _direccion = _selected.Direccion;

                switch (aa.Name)
                {
                    case "Iniciar":
                        System.Diagnostics.Process.Start(_direccion);
                        break;
                    case "ExportarSeleccionado":
                        if (GridResultados.Items.Count == 0)
                        {
                            System.Windows.MessageBox.Show("Seleccione al menos un enlace para exportar");
                            return;
                        }

                        using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + @"\EnlacesTorrentZilla.txt"))
                        {
                            foreach (var item in GridResultados.Items)
                                if (item is Herramientas.ListaTorrents _elemento)
                                    if (_elemento.Seleccionado)
                                        outputFile.WriteLine(string.Format("{0} \t - \t {1}", !string.IsNullOrEmpty(_elemento.NombreAmigable) ? _elemento.NombreAmigable.Trim() : string.Empty,
                                                                                              _elemento.Direccion));
                        }

                        System.Windows.MessageBox.Show("Se han exportado los enlaces en el escritorio, bajo el nombre EnlacesTorrentZilla.txt");
                        System.Collections.Generic.List<Herramientas.ListaTorrents> _filtros = new System.Collections.Generic.List<Herramientas.ListaTorrents>();
                        _lista.ForEach(x => _filtros.Add(new Herramientas.ListaTorrents()
                        {
                            Categoria = x.Categoria,
                            NombreAmigable = x.NombreAmigable,
                            Direccion = x.Direccion,
                            Fecha = x.Fecha,
                            Origen = x.Origen,
                            Seleccionado = false
                        }));

                        GridResultados.ItemsSource = _filtros.OrderBy(x => x.NombreAmigable).ToList();
                        break;
                    case "DetallesSeleccionado":
                        VentanaDetalles _detalles = new VentanaDetalles(_selected.Origen);
                        _detalles.Show();
                        break;
                    default:
                        break;
                }
            }
            catch (System.Exception exc)
            {
                System.Windows.MessageBox.Show(exc.Message);
            }
        }
        
        private void btnLimpiar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            txtBuscando.Text = string.Empty;
            txtCantidadPaginas.Text = string.Empty;
            txtPalabrasClave.Text = string.Empty;
            GridResultados.ItemsSource = new System.Collections.Generic.List<Herramientas.ListaTorrents>();
        }
    }
}