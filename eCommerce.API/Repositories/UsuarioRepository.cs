using Dapper;
using eCommerce.API.Models;
using eCommerce.API.Utils;
using System.Data;

namespace eCommerce.API.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        //Configuração para se conectar com o banco de dados usando configuração do ADO.NET/Dapper
        private IDbConnection _connection;
        public UsuarioRepository()
        {
            _connection = Utilidades.RetornaSqlConnection();
        }
        
        // ADO.NET > Dapper: Micro ORM (MER <-> POO)
        
        public List<Usuario> GetUsuarios()
        {
            // Para realizar essa consulta as propriedades das classes modelos têm que ter os mesmos nomes das colunas da tabela
            // Caso seja diferente têm que fazer algumas altareções. Consultar a documetação...
            // return _connection.Query<Usuario>("SELECT * FROM Usuarios").ToList();

            var usuarios = new List<Usuario>();
            var sql = RetornaQueryUsuarios();

            _connection.Query<Usuario, Contato, EnderecoEntrega, Departamento, Usuario>(sql, 
                (usuario, contato, enderecoEntrega, departamento) => {                    

                    if (usuarios.SingleOrDefault(s => s.Id == usuario.Id) == null)
                    {
                        usuario.Departamentos = new List<Departamento>();
                        usuario.EnderecosEntrega = new List<EnderecoEntrega>();
                        usuario.Contato = contato;
                        usuarios.Add(usuario);
                    }
                    else
                    {
                        usuario = usuarios.SingleOrDefault(s => s.Id == usuario.Id);
                    }

                    AdicionaEnderecosEntrega(usuario, enderecoEntrega);
                    AdicionaDepartamentos(usuario, departamento);

                    return usuario;
                });

            return usuarios;
        } 

        public Usuario GetUsuarioById(int id)
        {
            //O Id está sendo passado via "parâmetro, com essa forma se evita o SQL Injection."
            //E para atribuir o valor está sendo usando um objeto anônimo, em que a propriedade tem que ser o mesmo que o parâmetro, pois é case sensitive.            

            var usuarios = new List<Usuario>();
            var condicaoWhereUsuarioId = "WHERE U.Id = @Id";
            var sql = RetornaQueryUsuarios(condicaoWhereUsuarioId);

            _connection.Query<Usuario, Contato, EnderecoEntrega, Departamento, Usuario>(sql,
                (usuario, contato, enderecoEntrega, departamento) => {

                    if (usuarios.SingleOrDefault(s => s.Id == usuario.Id) == null)
                    {
                        usuario.Departamentos = new List<Departamento>();
                        usuario.EnderecosEntrega = new List<EnderecoEntrega>();
                        usuario.Contato = contato;
                        usuarios.Add(usuario);
                    }
                    else
                    {
                        usuario = usuarios.SingleOrDefault(s => s.Id == usuario.Id);
                    }

                    AdicionaEnderecosEntrega(usuario, enderecoEntrega);
                    AdicionaDepartamentos(usuario, departamento);

                    return usuario;
                }, new {Id = id});

            return usuarios.FirstOrDefault();
        }       

        public void InsertUsuario(Usuario usuario)
        {
            _connection.Open();
            var transaction = _connection.BeginTransaction();

            try
            {
                // O SELECT após o insert é para retornar o id inserido. O SCOPE_IDENTITY() retorna o id dentro daquele escopo de execução
                var sql = "INSERT INTO Usuarios (Nome, Email, Sexo, RG, CPF, NomeMae, SituacaoCadastro, DataCadastro) " +
                          "VALUES (@Nome, @Email, @Sexo, @RG, @CPF, @NomeMae, @SituacaoCadastro, @DataCadastro); " +
                          "SELECT CAST(SCOPE_IDENTITY() AS INT);";

                usuario.Id = _connection.Query<int>(sql, usuario, transaction).Single();

                if (usuario.Contato != null)
                {
                    usuario.Contato.UsuarioId = usuario.Id;
                    var sqlContato = "INSERT INTO Contatos (UsuarioId, Telefone, Celular) VALUES (@UsuarioId, @Telefone, @Celular); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    usuario.Contato.Id = _connection.Query<int>(sqlContato, usuario.Contato, transaction).Single();
                }

                InsereEnderecosEntrega(usuario, transaction);
                InsereDepartamentos(usuario, transaction);                

                transaction.Commit();
            }
            catch (Exception)
            {
                try
                {
                    transaction.Rollback();                    
                }
                catch (Exception)
                {

                    throw new Exception("Erro na tentativa de fazer um rollback"); ;
                }
            }
            finally
            {
                _connection.Close();
            }

        }

        public void UpdateUsuario(Usuario usuario)
        {

            _connection.Open();
            var transaction = _connection.BeginTransaction();

            try
            {
                var sql = "UPDATE Usuarios SET Nome = @Nome, Email = @Email, Sexo = @Sexo, RG = @RG, CPF = @CPF, NomeMae = @NomeMae, SituacaoCadastro = @SituacaoCadastro, DataCadastro = @DataCadastro WHERE Id = @Id";
                _connection.Execute(sql, usuario, transaction);

                if (usuario.Contato != null)
                {
                    var sqlContato = "UPDATE Contatos SET UsuarioId = @UsuarioId, Telefone = @Telefone, Celular = @Celular WHERE Id = @Id";
                    _connection.Execute(sqlContato, usuario.Contato, transaction);
                }

                var sqlDeletarEnderecosEntrega = "DELETE FROM EnderecosEntrega WHERE UsuarioId = @Id";
                _connection.Execute(sqlDeletarEnderecosEntrega, usuario, transaction);

                var sqlDeletarUsuariosDepartamentos = "DELETE FROM UsuariosDepartamentos WHERE UsuarioId = @Id";
                _connection.Execute(sqlDeletarUsuariosDepartamentos, usuario, transaction);

                InsereEnderecosEntrega(usuario, transaction);
                InsereDepartamentos(usuario, transaction);

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
            finally
            {
                _connection.Close();
            }
            
        }

        public void DeleteUsuario(int id)
        {
            _connection.Execute("DELETE FROM Usuarios WHERE ID = @Id", new {Id = id});
        }


        private string RetornaQueryUsuarios(string condicaoWhereUsuarioId = "")
        {
            return "SELECT U.*, C.*, EE.*, D.* FROM Usuarios U " +
                     "LEFT JOIN Contatos C ON C.UsuarioId = U.Id " +
                     "LEFT JOIN EnderecosEntrega EE ON EE.UsuarioId = U.Id " +
                     "LEFT JOIN UsuariosDepartamentos UD ON UD.UsuarioId = U.Id " +
                     "LEFT JOIN Departamentos D ON D.Id = UD.DepartamentoId" + condicaoWhereUsuarioId;
        }

        private void AdicionaEnderecosEntrega(Usuario? usuario, EnderecoEntrega enderecoEntrega)
        {
            // Verificação do Endereço de Entrega
            if (usuario.EnderecosEntrega.SingleOrDefault(s => s.Id == enderecoEntrega.Id) == null)
            {
                usuario.EnderecosEntrega.Add(enderecoEntrega);
            }
        }

        private void AdicionaDepartamentos(Usuario? usuario, Departamento departamento)
        {
            // Verificação do Departamento
            if (usuario.Departamentos.SingleOrDefault(s => s.Id == departamento.Id) == null)
            {
                usuario.Departamentos.Add(departamento);
            }
        }

        private void InsereEnderecosEntrega(Usuario usuario, IDbTransaction transaction)
        {
            if (usuario.EnderecosEntrega != null && usuario.EnderecosEntrega.Count > 0)
            {
                foreach (var enderecoEntrega in usuario.EnderecosEntrega)
                {
                    enderecoEntrega.UsuarioId = usuario.Id;
                    var sqlEnderecoEntrega = "INSERT INTO EnderecosEntrega (UsuarioId, NomeEndereco, CEP, Estado, Cidade, Bairro, Endereco, Numero, Complemento) Values (@UsuarioId, @NomeEndereco, @CEP, @Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    enderecoEntrega.Id = _connection.Query<int>(sqlEnderecoEntrega, enderecoEntrega, transaction).Single();
                }
            }
        }

        private void InsereDepartamentos(Usuario usuario, IDbTransaction transaction)
        {
            if (usuario.Departamentos != null && usuario.Departamentos.Count > 0)
            {
                foreach (var departamento in usuario.Departamentos)
                {
                    var sqlUsuariosDepartamentos = "INSERT INTO UsuariosDepartamentos (UsuarioId, DepartamentoId) Values (@UsuarioId, @DepartamentoId);";
                    departamento.Id = _connection.Execute(sqlUsuariosDepartamentos, new { UsuarioId = usuario.Id, DepartamentoId = departamento.Id }, transaction);
                }
            }
        }
    }
}