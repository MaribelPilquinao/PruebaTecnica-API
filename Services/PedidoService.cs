using Prueba_crud.Models;
using Prueba_crud.Repositories;

namespace Prueba_crud.Services
{
    public class PedidoService
    {
        private readonly PedidoRepository _repo;

        public PedidoService(PedidoRepository repo)
        {
            _repo = repo;
        }

        //public Task<IEnumerable<Pedido>> GetAll()
        //{
        //    return _repo.GetAll();
        //}
        public Task<IEnumerable<Pedido>> GetAllConDetalles() => _repo.GetAllConDetalles();


        public Task<int> CrearPedido(PedidoCrearDTO pedido) => _repo.CrearPedido(pedido);
        public Task<Pedido?> GetPedidoById(int id) => _repo.GetPedidoById(id);
        public Task<int> UpdatePedido(Pedido pedido) => _repo.UpdatePedido(pedido);
        public Task<int> DeletePedido(int id) => _repo.DeletePedido(id);


    }
}
