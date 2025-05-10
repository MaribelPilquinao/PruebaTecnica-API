using Microsoft.AspNetCore.Mvc;
using Prueba_crud.Models;
using Prueba_crud.Services;

namespace Prueba_crud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ClienteService _service;

        public ClientesController(ClienteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAll());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var cliente = await _service.GetById(id);
            return cliente == null ? NotFound(new { mensaje = "Cliente no encontrado" }) : Ok(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _service.Create(cliente);
            return resultado > 0
                ? Ok(new { mensaje = "Cliente creado con éxito" })
                : BadRequest(new { mensaje = "No se pudo crear el cliente" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Cliente cliente)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            cliente.Id = id;
            var resultado = await _service.Update(cliente);
            return resultado > 0
                ? Ok(new { mensaje = "Cliente actualizado" })
                : NotFound(new { mensaje = "No se pudo actualizar el cliente" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _service.Delete(id);
            return resultado > 0
                ? Ok(new { mensaje = "Cliente eliminado" })
                : NotFound(new { mensaje = "El cliente ya fue eliminado o no existe" });
        }
    }
}
