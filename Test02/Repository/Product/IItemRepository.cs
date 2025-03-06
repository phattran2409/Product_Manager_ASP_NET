
using Microsoft.EntityFrameworkCore;
using Test02.Models;
using Test02.Payload.Response;

namespace Test02.Repository.Product
{
    public interface IItemRepository
    {
        Task<int> GetTotalCountAsync();
        Task<List<ProductDTO>> GetItemsAsync(int skip, int take);
    }
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbcontext _dbcontext;
        public ItemRepository(AppDbcontext appDbcontext)
        {
            _dbcontext = appDbcontext;
        }
        public async Task<int> GetTotalCountAsync()
        {
            return await _dbcontext.Products.CountAsync();
        }
        public async Task<List<ProductDTO>> GetItemsAsync(int skip, int take)
        {
            return await _dbcontext.Products.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                image = p.image,
                Quantity = p.Quantity,
                Price = p.Price,
                CategoryId = p.CategoryId
            }).Skip(skip).Take(take).ToListAsync();
        }
    }   
}
