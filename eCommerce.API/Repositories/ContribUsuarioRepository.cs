using Dapper.Contrib.Extensions;
using eCommerce.API.Models;
using eCommerce.API.Utils;
using System.Data;

namespace eCommerce.API.Repositories
{
    public class ContribUsuarioRepository : IUsuarioRepository
    {
        private IDbConnection _connection;
        public ContribUsuarioRepository()
        {
            _connection = Utilidades.RetornaSqlConnection();
        }

        public List<Usuario> GetUsuarios()
        {   
            return _connection.GetAll<Usuario>().ToList();
        }

        public Usuario GetUsuarioById(int id)
        {
            return _connection.Get<Usuario>(id);
        }        

        public void InsertUsuario(Usuario usuario)
        {
            usuario.Id = Convert.ToInt32(_connection.Insert(usuario));
        }

        public void UpdateUsuario(Usuario usuario)
        {
            _connection.Update(usuario);
        }

        public void DeleteUsuario(int id)
        {
            _connection.Delete(GetUsuarioById(id));
        }
    }
}