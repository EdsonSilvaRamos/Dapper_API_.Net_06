using eCommerce.API.Models;
using eCommerce.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers
{
    [Route("api/Contrib/Usuarios")]
    [ApiController]
    public class ContribUsuariosController : ControllerBase
    {
        private IUsuarioRepository _usuarioRepository;

        public ContribUsuariosController()
        {
            _usuarioRepository = new ContribUsuarioRepository();
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
        public IActionResult Insert([FromBody] Usuario usuario)
        {
            _usuarioRepository.InsertUsuario(usuario);
            return Ok(usuario);
        }

        [HttpPut]
        public IActionResult UpdateUsuario([FromBody] Usuario usuario)
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
