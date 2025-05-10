using Microsoft.AspNetCore.Mvc;
using Prueba_crud.Models;
using Prueba_crud.Services;

namespace Prueba_crud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController: ControllerBase
    {
        private readonly PedidoService _service;

        public PedidosController(PedidoService service)
        {
            _service = service;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var pedidos = await _service.GetAll();
        //    return Ok(pedidos);
        //}

        [HttpGet("detalles")]
        public async Task<IActionResult> GetAllConDetalles()
        {
            var pedidos = await _service.GetAllConDetalles();
            return Ok(pedidos);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetPedido(int id)
        {
            var pedido = await _service.GetPedidoById(id);
            if (pedido == null)
                return NotFound(new { mensaje = "Pedido no encontrado" });

            return Ok(pedido);
        }


        [HttpPost]
        public async Task<IActionResult> CrearPedido([FromBody] PedidoCrearDTO pedido)
        {
            try
            {
                var pedidoId = await _service.CrearPedido(pedido);
                return Ok(new { mensaje = "Pedido creado correctamente", pedidoId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al crear el pedido", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Pedido pedido)
        {
            pedido.Id = id;
            var result = await _service.UpdatePedido(pedido);
            if (result > 0)
                return Ok(new { mensaje = "Pedido actualizado correctamente" });
            else
                return NotFound(new { mensaje = "No se encontró el pedido para modificar" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeletePedido(id);
            if (result > 0)
                return Ok(new { mensaje = "Pedido eliminado correctamente" });
            else
                return NotFound(new { mensaje = "El pedido no existe o ya fue eliminado" });
        }


    }
}
