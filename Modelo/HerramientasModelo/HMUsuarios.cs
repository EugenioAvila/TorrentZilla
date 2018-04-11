using System.Linq;
namespace Modelo.HerramientasModelo
{
    public class HMUsuarios : MainRule<USUARIOS>
    {
        public USUARIOS ObtenerUsuario(string _Id, string _Clave)
        {
            try
            {
                return GetContext.USUARIOS.FirstOrDefault(x => x.ID_USUARIO.Trim() == _Id && x.CLAVE == _Clave);
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }
    }
}