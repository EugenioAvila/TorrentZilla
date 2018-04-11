using System.Linq;

namespace Modelo
{
    public abstract class MainRule<TEntity> where TEntity : class
    {
        TorrentZillaLiteEntities ctx;
        string id_usuario;
        string pathproceso;
        bool needValidate;
        bool EnablingLazyLoading;

        public TorrentZillaLiteEntities GetContext
        {
            get
            {
                return ctx = ctx ?? new TorrentZillaLiteEntities();
            }
        }
        public MainRule()
        {
            ctx = new TorrentZillaLiteEntities();
        }

        ~MainRule()
        {
            ctx = null;
        }

        public MainRule<TEntity> ValidarUsuario(string ID_Usuario, string PathProceso)
        {
            id_usuario = ID_Usuario;
            pathproceso = PathProceso;
            needValidate = true;
            return this;
        }

        public MainRule<TEntity> UseLazyLoading(bool isEnabled = true)
        {
            EnablingLazyLoading = isEnabled;
            return this;
        }

        #region [Operaciones Basicas]
        public IQueryable<TEntity> SelectFrom
        {
            get
            {
                try
                {
                    ctx = GetContext;
                    ctx.Configuration.LazyLoadingEnabled = EnablingLazyLoading;
                    return ctx.Set<TEntity>();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
        }

        public virtual TEntity Insertar(TEntity entity)
        {
            try
            {
                ctx = GetContext;
                ctx.Configuration.LazyLoadingEnabled = EnablingLazyLoading;
                ctx.Set<TEntity>().Add(entity);
                ctx.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    System.Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return entity;
        }

        public virtual void Insertar(TEntity[] entity)
        {
            try
            {
                ctx = GetContext;

                //if (!ObtenerPermiso(EnumSeguridad.PermisoAccion.INSERTAR))
                //    throw new ApplicationException("No Tiene Permisos Para Realizar Esta Operación. [permiso necesario: \"INSERTAR\"]");

                ctx.Configuration.LazyLoadingEnabled = EnablingLazyLoading;
                ctx.Set<TEntity>().AddRange(entity);
                ctx.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public virtual TEntity Actualizar(TEntity entity)
        {
            try
            {
                ctx = GetContext;

                //if (!ObtenerPermiso(EnumSeguridad.PermisoAccion.ACTUALIZAR))
                //    throw new ApplicationException("No Tiene Permisos Para Realizar Esta Operación. [permiso necesario: \"ACTUALIZAR\"]");

                ctx.Configuration.LazyLoadingEnabled = EnablingLazyLoading;
                ctx.Set<TEntity>().Attach(entity);
                ctx.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                ctx.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return entity;
        }

        public virtual void Actualizar(TEntity[] entity)
        {
            try
            {
                ctx = GetContext;

                //if (!ObtenerPermiso(EnumSeguridad.PermisoAccion.ACTUALIZAR))
                //    throw new ApplicationException("No Tiene Permisos Para Realizar Esta Operación. [permiso necesario: \"ACTUALIZAR\"]");

                ctx.Configuration.LazyLoadingEnabled = EnablingLazyLoading;
                foreach (var item in entity)
                {
                    ctx.Set<TEntity>().Attach(item);
                    ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                ctx.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public virtual TEntity Eliminar(TEntity entity)
        {
            try
            {
                ctx = GetContext;

                //if (!ObtenerPermiso(EnumSeguridad.PermisoAccion.ELIMINAR))
                //    throw new ApplicationException("No Tiene Permisos Para Realizar Esta Operación. [permiso necesario: \"ELIMINAR\"]");

                ctx.Configuration.LazyLoadingEnabled = EnablingLazyLoading;
                ctx.Set<TEntity>().Attach(entity);
                ctx.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return entity;
        }

        public virtual void Eliminar(System.Collections.Generic.ICollection<TEntity> entity)
        {
            try
            {
                ctx = GetContext;
                ctx.Configuration.LazyLoadingEnabled = EnablingLazyLoading;
                foreach (var item in entity)
                {
                    ctx.Set<TEntity>().Attach(item);
                    ctx.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                }
                ctx.SaveChanges();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region [Utileria]
        public System.Data.Entity.Infrastructure.DbRawSqlQuery<TElement> EjecutarQuery<TElement>(string query)
        {
            try
            {
                ctx = GetContext;
                return ctx.Database.SqlQuery<TElement>(query);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public T ObtenerSecuencia_SQL<T>(string NombreSecuencia)
        {
            try
            {
                ctx = GetContext;
                return ctx.Database.SqlQuery<T>(string.Format("SELECT NEXT VALUE FOR {0};", NombreSecuencia)).FirstOrDefault();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public System.DateTime GetFechaServer
        {
            get
            {
                try
                {
                    return EjecutarQuery<System.DateTime>("SELECT GETDATE();").SingleOrDefault();
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Metodo para obtener el proximo ID utilizado en llavez compuestas.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tabla"></param>
        /// <param name="columna"></param>
        /// <param name="filtros"></param>
        /// <returns></returns>
        public virtual T GetIDProceso<T>(string tabla, string columna, string filtros) where T : struct
        {
            string query = string.Format("SELECT ISNULL(MAX({0}),0) + 1 AS ID FROM {1} WHERE {2} ", columna, tabla, filtros);
            return ctx.Database.SqlQuery<T>(query).Single<T>();
        }

        #endregion
    }

    public class EnumSeguridad
    {
        public enum PermisoAccion
        {
            INSERTAR, ACTUALIZAR, ELIMINAR, CONSULTAR, SIN_PERMISO
        }

        public enum TipoSistema
        {
            ESCRITORIO = 2, WEB = 1, MOVIL = 3
        }
    }
}