using MySql.Data.MySqlClient;
using Prueba_crud.Models;
using Dapper;

namespace Prueba_crud.Repositories
{
    public class ClienteRepository
    {
        private readonly IConfiguration _config;

        public ClienteRepository(IConfiguration config)
        {
            _config = config;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_config.GetConnectionString("Default"));
        }

        public async Task<IEnumerable<Cliente>> GetAll()
        {
            var sql = "SELECT * FROM Clientes";
            using var connection = GetConnection();
            return await connection.QueryAsync<Cliente>(sql);
        }

        public async Task<Cliente?> GetById(int id)
        {
            var sql = "SELECT * FROM Clientes WHERE Id = @Id";
            using var connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync<Cliente>(sql, new { Id = id });
        }

        public async Task<int> Create(Cliente cliente)
        {
            var sql = "INSERT INTO Clientes (Nombre, Correo, FechaNacimiento) VALUES (@Nombre, @Correo, @FechaNacimiento)";
            using var connection = GetConnection();
            return await connection.ExecuteAsync(sql, cliente);
        }

        public async Task<int> Update(Cliente cliente)
        {
            var sql = "UPDATE Clientes SET Nombre = @Nombre, Correo = @Correo, FechaNacimiento = @FechaNacimiento WHERE Id = @Id";
            using var connection = GetConnection();
            return await connection.ExecuteAsync(sql, cliente);
        }

        public async Task<int> Delete(int id)
        {
            var sql = "DELETE FROM Clientes WHERE Id = @Id";
            using var connection = GetConnection();
            return await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
