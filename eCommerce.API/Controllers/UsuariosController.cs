using eCommerce.API.Models;
using eCommerce.API.Repositories;
using Microsoft.AspNetCore.Mvc;

/*
    Crud:
    - GET -> Obter a lista de usuários.
    - GET(ID) -> Obter o usuario passando o ID.
    - POST -> Cadastrar um usuário.
    - PUT -> Atualizar um usuário.
    - DELETE ->  Remover um usuário.

    METHOD HTTP: www.minhaapi.com.br/api/usuarios - GET
                 www.minhaapi.com.br/api/usuarios/2 - GET(ID)
*/

namespace eCommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private IUsuarioRepository _usuarioRepository;

        public UsuariosController()
        {
            _usuarioRepository = new UsuarioRepository();
        }

        [HttpGet]
        public IActionResult GetUsuarios()
        {
            return Ok(_usuarioRepository.GetUsuarios());
        }

        [HttpGet("{id}")]
        public IActionResult GetUsuariosById(int id)
        {
            var usuario = _usuarioRepository.GetUsuarioById(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }
        
        [HttpPost]
        public IActionResult Insert([FromBody]Usuario usuario)
        {
            _usuarioRepository.InsertUsuario(usuario);
            return Ok(usuario);
        }
        
        [HttpPut]
        public IActionResult UpdateUsuario([FromBody]Usuario usuario)
        {
            _usuarioRepository.UpdateUsuario(usuario);
            return Ok(usuario);
        }
        
        [HttpDelete("{id}")]
        public IActionResult DeleteUsuario(int id)
        {
            var usuario = _usuarioRepository.GetUsuarioById(id);

            if (usuario == null)
            {
                return NotFound();
            }
            else
            {
                _usuarioRepository.DeleteUsuario(id);
                return Ok("Usuário deletado");
            }            
        }
    }
}
