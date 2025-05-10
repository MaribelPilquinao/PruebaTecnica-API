using Dapper;
using MySql.Data.MySqlClient;
using Prueba_crud.Models;


namespace Prueba_crud.Repositories
{
    public class ProductRepository
    {
        private readonly IConfiguration _config;

        public ProductRepository(IConfiguration config)
        {
            _config = config;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_config.GetConnectionString("Default"));
        }
        //trae todos los productos
        public async Task<IEnumerable<Product>> GetAll()
        {
            var sql = "SELECT * FROM Productos";
            using var connection = GetConnection();
            return await connection.QueryAsync<Product>(sql);
        }
        //trae por id
        public async Task<Product?> GetById(int id)
        {
            var sql = "SELECT * FROM Productos WHERE Id = @Id";
            using var connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
        }
        //crea un nuevo producto
        public async Task<int> Create(Product producto)
        {
            var sql = "INSERT INTO Productos (Nombre, Precio, Stock) VALUES (@Nombre, @Precio, @Stock)";
            using var connection = GetConnection();
            return await connection.ExecuteAsync(sql, producto);
        }
        //actualiza
        public async Task<int> Update(Product producto)
        {
            var sql = "UPDATE Productos SET Nombre = @Nombre, Precio = @Precio, Stock = @Stock WHERE Id = @Id";
            using var connection = GetConnection();
            return await connection.ExecuteAsync(sql, producto);
        }
        //elimina
        public async Task<int> Delete(int id)
        {
            var sql = "DELETE FROM Productos WHERE Id = @Id";
            using var connection = GetConnection();
            return await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}

