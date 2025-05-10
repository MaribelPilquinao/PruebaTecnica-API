using Prueba_crud.Models;
using Prueba_crud.Repositories;

namespace Prueba_crud.Services
{
    public class ProductService
    {
        private readonly ProductRepository _repo;

        public ProductService(ProductRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Product>> GetAll() =>_repo.GetAll();
        public Task<Product?> GetById(int id) => _repo.GetById(id);
        public Task<int> Create(Product p) => _repo.Create(p);
        public Task<int> Update(Product p) => _repo.Update(p);
        public Task<int> Delete(int id) => _repo.Delete(id);

    }
}
