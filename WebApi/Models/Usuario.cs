using System;
using System.Collections.Generic;

#nullable disable

namespace WebApi.Models
{
    public partial class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }
        public string Token { get; set; }
    }
}
