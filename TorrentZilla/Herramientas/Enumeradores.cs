namespace TorrentZilla.Herramientas
{
    public class Enumeradores
    {
        public enum eCategorias
        {
            AUDIO_MUSICA = 101,
            AUDIO_LIBROS = 102,
            AUDIO_CLIPS  = 103,
            AUDIO_FLAC   = 104,
            AUDIO_OTROS  = 105,
            VIDEO_PELICULAS = 201,
            VIDEO_PELICULAS_DVDR = 202,
            VIDEO_MUSICA = 203,
            VIDEO_CLIPS = 204,
            VIDEO_TV_SHOWS = 205,
            VIDEO_HANDHELD = 206,
            VIDEO_HD_PELICULAS = 207,
            VIDEO_HD_TV_SHOWS = 208,
            VIDEO_3D = 209,
            VIDEO_OTROS = 299,
            APLICACIONES_WINDOWS = 301,
            APLICACIONES_MAC = 302,
            APLICACIONES_UNIX = 303,
            APLICACIONES_HANDHELD = 304,
            APLICACIONES_IOS_IPAD_IPOD = 305,
            APLICACIONES_ANDROID = 306,
            APLICACIONES_OTROS = 399,
            JUEGOS_PC = 401,
            JUEGOS_MAC = 402,
            JUEGOS_PSX = 403,
            JUEGOS_XOBX360 = 404,
            JUEGOS_WII = 405,
            JUEGOS_HANDHELD = 406,
            JUEGOS_IOS_IPAD_IPOD = 407,
            JUEGOS_ANDROID = 408,
            JUEGOS_OTROS = 499,
            PORN_PELICULAS = 501,
            PORN_PELICULAS_DVDR = 502,
            PORN_IMAGENES = 503,
            PORN_JUEGOS = 504,
            PORN_PELICULAS_HD = 505,
            PORN_CLIPS = 506,
            PORN_OTROS = 599,
            OTROS_EBOOKS = 601,
            OTROS_COMICS = 602,
            OTROS_IMAGENES = 603,
            OTROS_COVERS = 604,
            OTROS_PHYSIBLES = 605,
            OTROS_OTROS = 699
        };

        public enum eDatosPorDefault
        {
            PAGINAS_POR_DEFECTO = 1,
            CATEGORIA_POR_DEFECTO = 7
        };
        public enum eModosBusqueda
        {
            SIN_DEFINIR = 0,
            POR_CATEGORIA = 1,
            POR_PALABRA_CLAVE = 2
        }
    }
}