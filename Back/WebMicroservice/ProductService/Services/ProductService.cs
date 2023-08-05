using AutoMapper;
using Common.Entites;
using Common.Exceptions;
using ProductService.DTOs;
using ProductService.Models;
using ProductService.Repository;

namespace ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public ProductService(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<ProductDTO> AddNewProduct(ProductDTO newProduct, long sellerId)
        {
            var l= await _productRepository.GetAll();
            List<Product> list = l.ToList();
            var product = list.Find(a => a.Name == newProduct.Name);
            if (product != null) { throw new ConflictException($"Product with Name: {newProduct.Name} already exists."); }

            var seller = await _productRepository.GetSeller(sellerId);
            if (seller == null) { throw new NotFoundException("User doesn't exist."); }

            if (!seller.Approved)
            {
                throw new ConflictException("Seller isn't approved. Wait for admin approval!");
            }


            product = _mapper.Map<Product>(newProduct);
            using (var ms = new MemoryStream())
            {
                newProduct.FormFile.CopyTo(ms);
                var fileBytes = ms.ToArray();

                product.PhotoUrl = fileBytes; 
                product.SellerId = sellerId;
             //   _productRepository.Update(product);
            }
           

            var p = await _productRepository.Create(product);  
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<bool> DeleteProduct(long id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null) { return false; }
            else
            {
                _productRepository.Delete(product.Id);
                return true;
            }
        }

        public async Task<List<ProductGetDTO>> GetAllProducts()
        {
            return _mapper.Map<List<ProductGetDTO>>(await _productRepository.GetAll());

        }

        public async Task<Product> GetLast(long userId)
        {
            var p = await _productRepository.GetAll();
            Product product = p.LastOrDefault();
            if (product == null) { throw new NotFoundException("Product doesn't exist."); }

            UserInfo user = await _productRepository.GetSeller(userId);
            if (user.UserType == "BUYER")
            {
                return product;
            }
            if (user.UserType.Contains("SELLER") && user.Id == product.SellerId)
            {
                return product;
            }
            else
            {
                throw new ConflictException("Product not available.");
            }
        }

        public async Task<ProductDTO> GetProduct(long id, long userId)
        {
            var product = await _productRepository.GetById(id);
            if (product == null) { throw new NotFoundException("Product doesn't exist."); }

            UserInfo user = await _productRepository.GetSeller(userId);
            if (user.UserType == "BUYER")
            {
                return _mapper.Map<ProductDTO>(product);
            }
            if (user.UserType == "SELLER" && user.Id == product.SellerId)
            {
                return _mapper.Map<ProductDTO>(product);
            }
            else
            {
                throw new ConflictException("Product not available.");
            }
        }

        public async Task<Product> GetProduct(long id)
        {
            var product = await _productRepository.GetById(id);
            if (product == null) { throw new NotFoundException("Product doesn't exist."); }
            return product;

        }

        public async Task<ProductImageDTO> GetProductImage(long id)
        {
            var product = await _productRepository.GetById(id) ?? throw new NotFoundException("Product doesn't exist.");

            byte[] imageBytes = await _productRepository.GetProductImage(product.Id);

            ProductImageDTO productImage = new ProductImageDTO()
            {
                ImageBytes = imageBytes
            };
            return productImage;
        }

        public async Task<List<ProductGetDTO>> GetSellerProducts(long id)
        {
            var seller = await _productRepository.GetSeller(id);
            if (seller == null) { throw new NotFoundException("User doesn't exist."); }
            if (!seller.Approved)
            {
                throw new ConflictException("Seller isn't approved. Wait for admin approval!");
            }
            else return _mapper.Map<List<ProductGetDTO>>(await _productRepository.GetSellerProducts(id));
        }

        public async Task<ProductUpdateDTO> UpdateProduct(long id, ProductUpdateDTO newProduct)
        {
            var product = await _productRepository.GetById(id);
            if (product == null) { throw new NotFoundException("Product doesn't exist."); }

            string name = product.Name;
            long ids = product.SellerId;
            var photo = product.PhotoUrl;
            product = _mapper.Map<Product>(newProduct);
            product.Name = name;
            product.SellerId = ids;
            product.PhotoUrl = photo;
            _productRepository.Update(product);
            return _mapper.Map<ProductUpdateDTO>(product);
        }

        public async Task UploadImage(long id, IFormFile file)
        {
            var product = await _productRepository.GetById(id);
            if (product == null) { throw new NotFoundException("Product doesn't exist."); }

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();

                product.PhotoUrl = fileBytes;
                _productRepository.Update(product);
            }
        }
    }
}
