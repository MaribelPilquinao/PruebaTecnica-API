using Prueba_crud.Models;
using Prueba_crud.Repositories;

namespace Prueba_crud.Services
{
    public class ClienteService
    {
        private readonly ClienteRepository _repo;

        public ClienteService(ClienteRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Cliente>> GetAll() => _repo.GetAll();
        public Task<Cliente?> GetById(int id) => _repo.GetById(id);
        public Task<int> Create(Cliente c) => _repo.Create(c);
        public Task<int> Update(Cliente c) => _repo.Update(c);
        public Task<int> Delete(int id) => _repo.Delete(id);
    }
}
