using System.Linq;
namespace Modelo.HerramientasModelo
{
    public class HMControlDeCambios : MainRule<ACERCA_DE>
    {
        public bool Insertar(System.Collections.Generic.List<ACERCA_DE> _datos)
        {
            var Context = GetContext;
            using (var _transaccion = Context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in _datos)
                        if (Context.ACERCA_DE.FirstOrDefault(x => x.ID_ACERCA_DE == item.ID_ACERCA_DE) == null)
                            Context.ACERCA_DE.Add(item);

                    Context.SaveChanges();
                    _transaccion.Commit();
                    return true;
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    _transaccion.Rollback();
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                        foreach (var validationError in validationErrors.ValidationErrors)
                            System.Diagnostics.Trace.TraceInformation("Nombre del campo causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                    return false;
                }
                catch (System.Exception ex)
                {
                    _transaccion.Rollback();
                    throw ex;
                }
            }
        }

        public System.Collections.Generic.List<CustomRegistroCambios> ObtenerControlCambios()
        {
            try
            {
                return GetContext.ACERCA_DE.Select(x => new CustomRegistroCambios
                {
                    Autor = x.AUTOR,
                    Mensaje = x.CODIGO,
                    CorreoAutor = x.CORREO_AUTOR,
                    Fecha = x.FECHA,
                    Id = !string.IsNullOrEmpty(x.ID_ACERCA_DE) ? x.ID_ACERCA_DE.Substring(0, 10) : string.Empty
                }).ToList();
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }
    }

    public class CustomRegistroCambios
    {
        public string Autor { get; set; }
        public string CorreoAutor { get; set; }
        public string Id { get; set; }
        public string Mensaje { get; set; }
        public System.DateTime? Fecha { get; set; }
    }
}
