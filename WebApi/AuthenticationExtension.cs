using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi
{
    public static class AuthenticationExtension
    {
        //IConfiguration me permite interactuar con el appsettings.json
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            //obtengo el campo key dentro del objeto Jwt, dentro de Auth del appsetting a traves de configuration
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Auth:Jwt:Key"]));

            //instalo paquete Microsoft.AspNetCore.Authentication.JwtBearer, misma version que los demas paquetes
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //autorizo el uso de estos atributos
                    //ClockSkew hace que el tiempo de validez comience desde cero, en appsetting puse que dure 20 min
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew=TimeSpan.Zero,
                    ValidateIssuerSigningKey=true,
                    RequireExpirationTime= true,
                    ValidIssuer=configuration["Auth:Jwt:Issuer"],
                    ValidAudience= configuration["Auth:Jwt:Audience"],
                    IssuerSigningKey= signingKey
                
                };
                options.Events = new JwtBearerEvents
                {
                    //si falla la authentication del token
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("OnAutheticationFailed" + context.Exception.Message);
                        return Task.CompletedTask;

                    },
                    //comprobamos si el token es valido
                    OnTokenValidated= context =>
                    {
                        Console.WriteLine("OnTokenValidated" + context.SecurityToken);
                        return Task.CompletedTask;
                    }

                };

            });

            return services;

        }
    }
}
