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
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }

        /// <summary>
        /// Obtém todos os usuários
        /// </summary>
        public Task<IEnumerable<Usuario>> GetAllAsync()
        {
            var usuarios = _context.Usuarios.AsEnumerable();
            return Task.FromResult(usuarios);
        }

        public Task<Usuario?> GetByIdAsync(string id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(usuario);
        }

        public Task<Usuario?> GetByEmailAsync(string email)
        {
            // Log temporário para depuração
            Console.WriteLine($"[DEBUG] Procurando email: '{email}'");
            Console.WriteLine($"[DEBUG] Usuários na memória: {_context.Usuarios.Count}");
            foreach (var u in _context.Usuarios)
            {
                Console.WriteLine($"[DEBUG] Usuario: Id='{u.Id}', Email='{u.Email}'");
            }

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            Console.WriteLine(usuario == null ? "[DEBUG] Usuario não encontrado" : $"[DEBUG] Usuario encontrado: {usuario.Email}");
            return Task.FromResult(usuario);
        }

        public Task<Usuario> CreateAsync(Usuario usuario)
        {
            usuario.DataCriacao = DateTime.UtcNow;
            _context.Usuarios.Add(usuario);
            return Task.FromResult(usuario);
        }

        public Task<Usuario?> UpdateAsync(string id, Usuario usuario)
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

        public Task<bool> DeleteAsync(string id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);

            if (usuario == null)
                return Task.FromResult(false);

            _context.Usuarios.Remove(usuario);
            return Task.FromResult(true);
        }

        public Task<bool> ExistsAsync(string id)
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
