using Dapper;
using eCommerce.API.Models;
using eCommerce.API.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace eCommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipsController : ControllerBase
    {
        private IDbConnection _connection;
        public TipsController()
        {
            _connection = Utilidades.RetornaSqlConnection();        
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var sql = "SELECT * FROM Usuarios WHERE Id = @Id; " +
                      "SELECT* FROM Contatos WHERE UsuarioId = @Id; " +
                      "SELECT* FROM EnderecosEntrega WHERE UsuarioId = @Id; " +
                      "SELECT D.*FROM UsuariosDepartamentos UD INNER JOIN Departamentos D ON UD.DepartamentoId = D.Id WHERE UD.UsuarioId = @Id;";

            using (var multipleResultSet = _connection.QueryMultiple(sql, new { Id = id }))
            {
                var usuario = multipleResultSet.Read<Usuario>().SingleOrDefault();
                var contato = multipleResultSet.Read<Contato>().SingleOrDefault();
                var enderecosEntrega = multipleResultSet.Read<EnderecoEntrega>().ToList();
                var departamento = multipleResultSet.Read<Departamento>().ToList();

                if (usuario != null)
                {
                    usuario.Contato = contato;
                    usuario.EnderecosEntrega = enderecosEntrega;
                    usuario.Departamentos = departamento;

                    return Ok(usuario);
                }
            }

            return NotFound("Usuário não encontrado!");
        }

        // Dapper Stored Procedures

        [HttpGet("stored/usuarios")]
        public IActionResult StoredGet()
        {
            var usuarios = _connection.Query<Usuario>("SelecionarUsuarios", commandType: CommandType.StoredProcedure);

            return Ok(usuarios);
        }

        [HttpGet("stored/usuarios/{id}")]
        public IActionResult StoredGet(int id)
        {
            var usuario = _connection.Query<Usuario>("SelecionarUsuario", new { Id = id }, commandType: CommandType.StoredProcedure);

            return Ok(usuario);
        }

        // Dapper Fluent Mapper

        [HttpGet("mapper/sql/usuarios")]
        public IActionResult MapperSql()
        {
            /*
             *  Problema: Mapear colunas com nomes diferentes das propriedades do objeto
             *  Solução 01: SQL(MER) => Renomear os nomes das colunas 
             */
            
            var usuarios = _connection.Query<UsuarioDiferente>("SELECT Id Cod, Nome NomeCompleto, Email, Sexo, RG, CPF, NomeMae NomeCompletoMae, SituacaoCadastro Situacao, DataCadastro FROM Usuarios;");
            return Ok(usuarios);
        }

        [HttpGet("mapper/map/usuarios")]
        public IActionResult MapperMap()
        {
            /*
             *  Problema: Mapear colunas com nomes diferentes das propriedades do objeto
             *  Solução 02: C#(POO) => Mapeamento por meio da biblioteca Dapper.FluentMap
             *  Configuração feita na classe Program.cs
             */

            var usuarios = _connection.Query<UsuarioDiferente>("SELECT * FROM Usuarios;");
            return Ok(usuarios);
        }
    }
}