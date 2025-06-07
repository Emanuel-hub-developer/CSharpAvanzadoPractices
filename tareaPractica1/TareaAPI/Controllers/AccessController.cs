using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareaAPI.Core.DomainLayer.DTOs;
using TareaAPI.Core.DomainLayer.Models;
using TareaAPI.Infrastructure.Data;
using TareaAPI.InfrastructureLayer.Persistance.Utilities;

namespace TareaAPI.Controllers
{
    [Route("api/")]
    [AllowAnonymous]
    [ApiController]

    public class AccessController : ControllerBase
    {
        private readonly TareaAPIContext _context;
        private readonly JwtGeneralConfig _jwtconfig;
       

        public AccessController(TareaAPIContext context, JwtGeneralConfig jwtConfig)
        {
            _context = context;

            _jwtconfig = jwtConfig;

        }

        [HttpPost]
        [Route("auth/register")]
        public async Task<IActionResult> Registrarse(UserDTO objeto)
        {
            var modeloUsuario = new UserModel
            {
                Name = objeto.Name,
                BirthDate = objeto.BirthDate,
                Email = objeto.Email,
                Password = _jwtconfig.encriptarSHA256(objeto.Password)
            };

            await _context.Users.AddAsync(modeloUsuario);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {

                var usuarioDTO = new UserDTO
                {
                    Name = objeto.Name,
                    BirthDate = objeto.BirthDate,
                    Email = objeto.Email,
                    Password = objeto.Password
                };

                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, message = "Usuario registrado correctamente." });
            }
            else
            {

                return StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = false, message = "No se pudo registrar el usuario en la base de datos." });
            }
        }


        [HttpPost]
        [Route("auth/login")]
        public async Task<IActionResult> Login(LoginDTO objeto)
        {
            try
            {


                var usuarioEncontrado = await _context.Users
                                       .Where(u =>
                                       u.Email == objeto.Email &&
                                       u.Password == _jwtconfig.encriptarSHA256(objeto.Password)
                                       ).FirstOrDefaultAsync();


                if (usuarioEncontrado == null)
                    return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });



                var jwtToken = _jwtconfig.generateJWT(usuarioEncontrado);

                var refreshToken = _jwtconfig.GenerateRefreshToken();


                // Guardar en la base de datos
                var historialToken = new RecordRefreshTokens
                {
                    UserId = usuarioEncontrado.Id,
                    Token = jwtToken,
                    RefreshToken = refreshToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5)// Refresh token valido por 5 minutos
                };




                _context.RecordRefreshTokens.Add(historialToken);
                await _context.SaveChangesAsync();


                return Ok(new { Token = jwtToken, RefreshToken = refreshToken });
            }

            catch (DbUpdateException dbEx)
            {
                // Manejo de excepciones específicas de la base de datos
                var errorMessage = dbEx.InnerException != null ? dbEx.InnerException.Message : "Error desconocido al actualizar la base de datos.";
                Console.WriteLine($"Error al guardar en la base de datos: {errorMessage}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = errorMessage });
            }
            catch (Exception ex)
            {
                // Manejo de excepciones generales
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Error inesperado al crear RefreshToken" });
            }


        }



        [HttpPost]
        [Route("auth/refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDTO)
        {
            var storedToken = await _context.RecordRefreshTokens
            .FirstOrDefaultAsync(rt => rt.RefreshToken == refreshTokenDTO.RefreshToken);

            // Verificar si el token es nulo o ha expirado
            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Refresh Token inválido o expirado." });
            }

            // Obtener el usuario asociado al refresh token
            var usuario = await _context.Users.FirstOrDefaultAsync(u => u.Id == storedToken.UserId);

            if (usuario == null)
            {
                return Unauthorized(new { message = "Usuario no encontrado." });
            }

            // Generar un nuevo JWT
            var newJwtToken = _jwtconfig.generateJWT(usuario);

            //Refrescar el Token
            var newRefreshToken = _jwtconfig.GenerateRefreshToken();


            await _context.SaveChangesAsync();

            return Ok(new { Token = newJwtToken, RefreshToken = newRefreshToken });
        }


    }
}
