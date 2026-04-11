using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelourEssence.Infraestructure.Data;
using VelourEssence.Infraestructure.Models;
using VelourEssence.Infraestructure.Repository.Interfaces;

namespace VelourEssence.Infraestructure.Repository.Implementations
{
    // Repositorio para manejar usuarios del sistema
    public class RepositoryUsuarios : IRepositoryUsuarios
    {
        private readonly VelourEssenceContext _context;

        // Constructor con inyección del contexto de BD
        public RepositoryUsuarios(VelourEssenceContext context)
        {
            _context = context;
        }

        // Obtiene todos los usuarios con información del rol
        public async Task<ICollection<Usuario>> ListAsync()
        {
            var collection = await _context.Set<Usuario>()
                .Include(u => u.IdRolNavigation)
                .ToListAsync();

            return collection;
        }

        // Busca un usuario por ID con información del rol
        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Set<Usuario>()
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);
        }

        // Crea un nuevo usuario
        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            _context.Set<Usuario>().Add(usuario);
            await _context.SaveChangesAsync();
            
            // Recargar el usuario con la información del rol
            return await GetByIdAsync(usuario.IdUsuario) ?? usuario;
        }

        // Actualiza un usuario existente
        public async Task<Usuario?> UpdateAsync(Usuario usuario)
        {
            var usuarioExistente = await _context.Set<Usuario>()
                .FirstOrDefaultAsync(u => u.IdUsuario == usuario.IdUsuario);

            if (usuarioExistente == null)
                return null;

            // Actualizar propiedades
            usuarioExistente.NombreUsuario = usuario.NombreUsuario;
            usuarioExistente.Correo = usuario.Correo;
            usuarioExistente.IdRol = usuario.IdRol;

            // Solo actualizar contraseña si se proporciona una nueva
            if (!string.IsNullOrWhiteSpace(usuario.Contraseña))
            {
                usuarioExistente.Contraseña = usuario.Contraseña;
            }

            _context.Set<Usuario>().Update(usuarioExistente);
            await _context.SaveChangesAsync();

            // Recargar el usuario con la información del rol
            return await GetByIdAsync(usuario.IdUsuario);
        }

        // Elimina un usuario por ID
        public async Task<bool> DeleteAsync(int id)
        {
            var usuario = await _context.Set<Usuario>()
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
                return false;

            _context.Set<Usuario>().Remove(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        // Obtiene todos los roles disponibles
        public async Task<ICollection<Rol>> GetRolesAsync()
        {
            return await _context.Set<Rol>()
                .OrderBy(r => r.NombreRol)
                .ToListAsync();
        }

        // Verifica si un nombre de usuario ya existe
        public async Task<bool> ExisteNombreUsuarioAsync(string nombreUsuario, int? idUsuarioExcluir = null)
        {
            var query = _context.Set<Usuario>()
                .Where(u => u.NombreUsuario.ToLower() == nombreUsuario.ToLower());

            if (idUsuarioExcluir.HasValue)
            {
                query = query.Where(u => u.IdUsuario != idUsuarioExcluir.Value);
            }

            return await query.AnyAsync();
        }

        // Verifica si un correo ya existe
        public async Task<bool> ExisteCorreoAsync(string correo, int? idUsuarioExcluir = null)
        {
            var query = _context.Set<Usuario>()
                .Where(u => u.Correo.ToLower() == correo.ToLower());

            if (idUsuarioExcluir.HasValue)
            {
                query = query.Where(u => u.IdUsuario != idUsuarioExcluir.Value);
            }

            return await query.AnyAsync();
        }

        // Busca un usuario por correo electrónico o nombre de usuario
        public async Task<Usuario?> GetByEmailOrUsernameAsync(string emailOUsuario)
        {
            return await _context.Set<Usuario>()
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => 
                    u.Correo.ToLower() == emailOUsuario.ToLower() || 
                    u.NombreUsuario.ToLower() == emailOUsuario.ToLower());
        }

        // Busca usuario por correo (para recuperación de contraseña)
        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Set<Usuario>()
                .FirstOrDefaultAsync(u => u.Correo.ToLower() == email.ToLower());
        }

        // Guarda el token y expiración para recuperación de contraseña
        public async Task<bool> SetPasswordResetTokenAsync(int idUsuario, string token, DateTime expiration)
        {
            var usuario = await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
            if (usuario == null) return false;
            usuario.PasswordResetToken = token;
            usuario.PasswordResetTokenExpiration = expiration;
            _context.Set<Usuario>().Update(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        // Limpia el token de recuperación (después de usar o expirar)
        public async Task<bool> ClearPasswordResetTokenAsync(int idUsuario)
        {
            var usuario = await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
            if (usuario == null) return false;
            usuario.PasswordResetToken = null;
            usuario.PasswordResetTokenExpiration = null;
            _context.Set<Usuario>().Update(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        // Actualiza la fecha de último inicio de sesión
        public async Task UpdateLastLoginAsync(int idUsuario)
        {
            var usuario = await _context.Set<Usuario>()
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

            if (usuario != null)
            {
                usuario.UltimoInicioSesion = DateTime.Now;
                _context.Set<Usuario>().Update(usuario);
                await _context.SaveChangesAsync();
            }
        }
    }
}