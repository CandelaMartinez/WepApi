using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("Productos")]
    [EnableCors("MyPolicy")]
    [Authorize]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly EmpresaContext _context;

        //inyeccion de dependencias, inyecta en el constructor el contexto.
        public ProductoController(EmpresaContext context)
        {
            _context = context;
        }

        // GET: api/Producto
        [HttpGet]
        [Route("ListarProductos")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.ToListAsync();
        }

        // GET: api/Producto/5
        [HttpGet]
        [Route("ObtenerProducto/{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        // PUT: api/Producto/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Route("ActualizarProducto")]
        public async Task<IActionResult> PutProducto(Producto producto)
        {
            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(producto.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Producto
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("CrearProducto")]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            _context.Productos.Add(producto);
           var resultado= await _context.SaveChangesAsync();
            if (resultado == 1)
            {
                return Ok(new {respuesta="0000",mensaje="Exito"});
            }
            else
            {
                return BadRequest(new { respuesta = "1111", mensaje = "Error" });
            }

            
        }

        // DELETE: api/Producto/5
        [HttpDelete]
        [Route("EliminarProducto/{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            

            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                return Ok(new { respuesta = "0000", mensaje = "Exito" });
            }
            else
            {
                return BadRequest(new { respuesta = "1111", mensaje = "Error" });
            }
               

            
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
