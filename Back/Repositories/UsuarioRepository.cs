using Back.Data;
using Back.Models;

namespace Back.Repositories
{
    /// <summary>
    /// Implementação do repository para Usuários usando DataContext em memória
    /// </summary>
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DataContext _context;

        public UsuarioRepository(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todos os usuários
        /// </summary>
        public Task<IEnumerable<Usuario>> GetAllAsync()
        {
            var usuarios = _context.Usuarios.AsEnumerable();
            return Task.FromResult(usuarios);
        }

        /// <summary>
        /// Obtém um usuário por ID
        /// </summary>
        public Task<Usuario?> GetByIdAsync(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(usuario);
        }

        /// <summary>
        /// Obtém um usuário por email
        /// </summary>
        public Task<Usuario?> GetByEmailAsync(string email)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            return Task.FromResult(usuario);
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        public Task<Usuario> CreateAsync(Usuario usuario)
        {
            if (_context.Usuarios.Count == 0)
            {
                usuario.Id = 1;
            }
            else
            {
                usuario.Id = _context.Usuarios.Max(u => u.Id) + 1;
            }

            usuario.DataCriacao = DateTime.UtcNow;
            _context.Usuarios.Add(usuario);
            return Task.FromResult(usuario);
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        public Task<Usuario?> UpdateAsync(int id, Usuario usuario)
        {
            var usuarioExistente = _context.Usuarios.FirstOrDefault(u => u.Id == id);

            if (usuarioExistente == null)
                return Task.FromResult((Usuario?)null);

            usuarioExistente.Email = usuario.Email;
            usuarioExistente.Nome = usuario.Nome;
            usuarioExistente.Perfil = usuario.Perfil;
            usuarioExistente.SenhaHash = usuario.SenhaHash;
            usuarioExistente.Ativo = usuario.Ativo;
            usuarioExistente.DataAtualizacao = DateTime.UtcNow;

            return Task.FromResult((Usuario?)usuarioExistente);
        }

        /// <summary>
        /// Deleta um usuário
        /// </summary>
        public Task<bool> DeleteAsync(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);

            if (usuario == null)
                return Task.FromResult(false);

            _context.Usuarios.Remove(usuario);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Verifica se um usuário existe
        /// </summary>
        public Task<bool> ExistsAsync(int id)
        {
            var existe = _context.Usuarios.Any(u => u.Id == id);
            return Task.FromResult(existe);
        }

        /// <summary>
        /// Verifica se um email já está registrado
        /// </summary>
        public Task<bool> EmailExistsAsync(string email)
        {
            var existe = _context.Usuarios.Any(u => u.Email == email);
            return Task.FromResult(existe);
        }
    }
}
