using Common;
using Common.Entites;
using Microsoft.EntityFrameworkCore;
using ProductService.Database;
using ProductService.Models;

namespace ProductService.Repository
{
    public class ProductRepository : IProductRepository
    {
        ProductDBContext _dbContext;

        public ProductRepository(ProductDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<Product> Create(Product entity)
        {
            await _dbContext.Products.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(long id)
        {
            _dbContext.Products.Remove(_dbContext.Products.Find(id));
            await _dbContext.SaveChangesAsync();
            //return  _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<Product> GetById(long id)
        {
            return await _dbContext.Products.FindAsync(id);
        }

        public async Task<byte[]> GetProductImage(long id)
        {
            Product product = await _dbContext.Products.FindAsync(id);
            return product.PhotoUrl;
        }

        public async Task<List<Product>> GetSellerProducts(long id)
        {
            return await _dbContext.Products.Where(p => p.SellerId == id).ToListAsync();
        }

        public async Task Update(Product entity)
        {
            Product newProduct = _dbContext.Products.Find(entity.Id);
            newProduct = entity;
            _dbContext.Products.Update(newProduct);
           // return _dbContext.SaveChangesAsync();
           await _dbContext.SaveChangesAsync();
        }

       public  async Task<UserInfo> GetSeller(long id)
        {
            return await _dbContext.UsersInfo.FindAsync(id);
           
       }
    }
}
