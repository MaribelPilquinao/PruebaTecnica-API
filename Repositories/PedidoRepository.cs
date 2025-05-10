using MySql.Data.MySqlClient;
using Prueba_crud.Models;
using Dapper;

namespace Prueba_crud.Repositories
{
    public class PedidoRepository
    {
        private readonly IConfiguration _config;

        public PedidoRepository(IConfiguration config)
        {
            _config = config;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(_config.GetConnectionString("Default"));
        }

        public async Task<IEnumerable<Pedido>> GetAll()
        {
            using var connection = GetConnection();
            var pedidos = await connection.QueryAsync<Pedido>(
                "SELECT Id, IdCliente, Fecha, Total FROM Pedidos ORDER BY Fecha DESC");
            return pedidos;
        }

        public async Task<IEnumerable<Pedido>> GetAllConDetalles()
        {
            using var connection = GetConnection();

            var pedidos = (await connection.QueryAsync<Pedido>(
                "SELECT Id, IdCliente, Fecha, Total FROM Pedidos ORDER BY Fecha DESC")).ToList();

            foreach (var pedido in pedidos)
            {
                var detalles = await connection.QueryAsync<PedidoDetalle>(
                    @"SELECT pd.IdProducto, pd.Cantidad, pd.PrecioUnitario, pd.Subtotal,
                             p.Nombre AS NombreProducto
                      FROM PedidoDetalle pd
                      INNER JOIN Productos p ON pd.IdProducto = p.Id
                      WHERE pd.IdPedido = @Id",
                    new { Id = pedido.Id });

                pedido.Detalles = detalles.ToList();
            }

            return pedidos;
        }

        public async Task<Pedido?> GetPedidoById(int id)
        {
            using var connection = GetConnection();

            var pedido = await connection.QueryFirstOrDefaultAsync<Pedido>(
                "SELECT * FROM Pedidos WHERE Id = @Id",
                new { Id = id });

            if (pedido == null)
                return null;

            var detalles = await connection.QueryAsync<PedidoDetalle>(
                @"SELECT pd.IdProducto, pd.Cantidad, pd.PrecioUnitario, pd.Subtotal,
                         p.Nombre AS NombreProducto
                  FROM PedidoDetalle pd
                  INNER JOIN Productos p ON pd.IdProducto = p.Id
                  WHERE pd.IdPedido = @Id",
                new { Id = id });

            pedido.Detalles = detalles.ToList();

            return pedido;
        }

        //creaer pedido
        public async Task<int> CrearPedido(PedidoCrearDTO pedido)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                //validar si el cliente exite
                var clienteExiste = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Clientes WHERE Id = @Id",
                 new { Id = pedido.IdCliente }, transaction);

                    if (clienteExiste == 0)
                        throw new Exception($"El cliente ID {pedido.IdCliente} no existe.");

                decimal total = 0;

                foreach (var item in pedido.Detalles)
                {
                    var producto = await connection.QueryFirstOrDefaultAsync<dynamic>(
                        "SELECT Precio, Stock FROM Productos WHERE Id = @Id",
                        new { Id = item.IdProducto }, transaction);

                    if (producto == null)
                        throw new Exception($"El producto ID {item.IdProducto} no existe.");

                    if (producto.Stock < item.Cantidad)
                        throw new Exception($"No hay stock suficiente para el producto ID {item.IdProducto}");

                    total += (decimal)producto.Precio * item.Cantidad;
                }

                var pedidoId = await connection.ExecuteScalarAsync<int>(
                    @"INSERT INTO Pedidos (IdCliente, Fecha, Total)
                      VALUES (@IdCliente, @Fecha, @Total);
                      SELECT LAST_INSERT_ID();",
                    new { pedido.IdCliente, Fecha = DateTime.Now, Total = total }, transaction);

                foreach (var item in pedido.Detalles)
                {
                    var precio = await connection.ExecuteScalarAsync<decimal>(
                        "SELECT Precio FROM Productos WHERE Id = @Id",
                        new { Id = item.IdProducto }, transaction);

                    await connection.ExecuteAsync(
                        @"INSERT INTO PedidoDetalle (IdPedido, IdProducto, Cantidad, PrecioUnitario, Subtotal)
                          VALUES (@IdPedido, @IdProducto, @Cantidad, @Precio, @Subtotal)",
                        new
                        {
                            IdPedido = pedidoId,
                            item.IdProducto,
                            item.Cantidad,
                            Precio = precio,
                            Subtotal = precio * item.Cantidad
                        }, transaction);

                    await connection.ExecuteAsync(
                        "UPDATE Productos SET Stock = Stock - @Cantidad WHERE Id = @IdProducto",
                        new { item.Cantidad, item.IdProducto }, transaction);
                }

                await transaction.CommitAsync();
                return pedidoId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        //solo modifico fecha y cliente
        public async Task<int> UpdatePedido(Pedido pedido)
        {
            using var connection = GetConnection();
            var result = await connection.ExecuteAsync(
                @"UPDATE Pedidos
          SET IdCliente = @IdCliente,
              Fecha = @Fecha
          WHERE Id = @Id",
                pedido);

            return result;
        }

        //elimina el pedido y los productos relacionados
        public async Task<int> DeletePedido(int id)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            // validar si el pedido exite
            var pedidoExiste = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Pedidos WHERE Id = @Id", new { Id = id });

            if (pedidoExiste == 0)
                return 0;

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                await connection.ExecuteAsync(
                    "DELETE FROM PedidoDetalle WHERE IdPedido = @Id",
                    new { Id = id }, transaction);

                var result = await connection.ExecuteAsync(
                    "DELETE FROM Pedidos WHERE Id = @Id",
                    new { Id = id }, transaction);

                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
