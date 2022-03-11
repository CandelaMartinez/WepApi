using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("Seguridad")]
    [EnableCors("MyPolicy")]
    [ApiController]
    [Authorize]
    public class SeguridadController : ControllerBase
    {
        //instancio el contexto, que es mi DB para poder usarla
        private readonly EmpresaContext _context;
        //instancio configuration para poder acceder al appsettings
        private IConfiguration _configuration;

        //inyeccion de dependencias para agregar el uso de los servicios que necesito usar+
        public SeguridadController(EmpresaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        //endpoint para construir token
        //Authorize, como el controller esta puesto para que se pida authorizacion
        //AllowAnonimous me permite usar este endpoint sin pedir autorizacion
        [HttpPost]
        [AllowAnonymous]
        [Route("ValidarUsuario")]
        public async Task<IActionResult> ValidarUsuario([FromBody] Usuario usuario)
        {
            //si el nombre de usuario y clave es correcto, validamos
            //accedo al contexto, DB para validar nombre y clave con el objeto usuario que paso x parametro
            if (usuario == null) return BadRequest();
            var result = await _context.Usuarios.Where(o => o.NombreUsuario == usuario.NombreUsuario &&
             o.Clave == usuario.Clave).FirstOrDefaultAsync();

            //construimos el token
            if (result != null)
            {
                var Token = BuildToken(usuario);
                usuario.Token = Token;
                return Ok(usuario);
            }
            else
            {
                return NotFound();
            }
           

        }

        //metodo que construye el token
        private string BuildToken(Usuario usuario)
        {
            //declaro y doy valor a los parametros que usare en la construccion del token con JwtSecurityToken
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth:Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario)
            };

            //construir
            var token = new JwtSecurityToken(_configuration["Auth:Jwt:Issuer"],
                _configuration["Auth:Jwt:Audience"],
                claims,expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Auth:Jwt:TokenExpirationInMinutes"])),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
