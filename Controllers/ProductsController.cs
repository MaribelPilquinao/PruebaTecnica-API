using Microsoft.AspNetCore.Mvc;
using Prueba_crud.Models;
using Prueba_crud.Services;

namespace Prueba_crud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductsController(ProductService service) 
        
        { 
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAll();
            return Ok(products);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> Get(int id)
        {
            var producto = await _service.GetById(id);

            if (producto == null)
            {
                return NotFound(new { mensaje = "El producto no existe o ya fue eliminado." });
            }

            return Ok(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product producto)
        {
            try
            {
                var resultado = await _service.Create(producto);
                if (resultado > 0)
                    return Ok(new { mensaje = "Producto creado con éxito" });
                else
                    return BadRequest(new { mensaje = "No se pudo crear el producto" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error en el servidor", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product producto)
        {
            producto.Id = id;
            await _service.Update(producto);
            return Ok(new { mensaje = "Producto actualizado" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var resultado = await _service.Delete(id);

                if (resultado > 0)
                {
                    return Ok(new { mensaje = "Producto eliminado correctamente" });
                }
                else
                {
                    return NotFound(new { mensaje = "El producto ya fue eliminado o no existe" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error en el servidor", error = ex.Message });
            }
        }
    }
}
