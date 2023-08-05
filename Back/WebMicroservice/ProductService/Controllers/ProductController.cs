using Common.Entities;
using Common.Entities.Product;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs;
using ProductService.Services;
using System.Data;
using System.Security.Claims;

namespace ProductService.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IPublishEndpoint _publishEndpoint;

        public ProductController(IProductService productService, IPublishEndpoint publishEndpoint)
        {
            _productService = productService;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("get")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> Get([FromBody] long id)
        {
            return Ok(await _productService.GetProduct(id, GetUserIdFromToken(User)));
        }

        private long GetUserIdFromToken(ClaimsPrincipal user)
        {
            long id;
            long.TryParse(user.Identity.Name, out id);
            return id;
        }

        [HttpGet("get-all")]
        [Authorize(Roles = "BUYER, ADMIN")]
        public async Task<IActionResult> GetAll()
        {
            List<ProductGetDTO> articleDTOs = await _productService.GetAllProducts();
            return Ok(articleDTOs);
        }

        [HttpPut("new-article")]
        [Authorize(Roles = "SELLER")]
        public async Task<IActionResult> NewProduct([FromForm] ProductDTO productDTO)
        {
            await _productService.AddNewProduct(productDTO, GetUserIdFromToken(User));
            var p   = await _productService.GetLast(GetUserIdFromToken(User));
            
            await _publishEndpoint.Publish<ProductInfo>(new ProductInfo() { 
                Id = p.Id,
                SellerId = GetUserIdFromToken(User),
                Name = productDTO.Name,
                Price = productDTO.Price,
                MaxQuantity = productDTO.MaxQuantity
            
            });
            return Ok();
        }

        [HttpPut("update-article")]
        [Authorize(Roles = "SELLER")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductUpdateDTO productDTO)
        {
            await _productService.UpdateProduct(productDTO.Id, productDTO);
            var p = await _productService.GetProduct(productDTO.Id);
            await _publishEndpoint.Publish<ProductInfo>(new ProductInfo()
            {
                Id = p.Id,
                Name = p.Name,
                Price = productDTO.Price,
                SellerId = GetUserIdFromToken(User),
                MaxQuantity= productDTO.MaxQuantity 
            }) ;
            return Ok();
        }

        [HttpPut("delete")]
        [Authorize(Roles = "SELLER")]
        public async Task<IActionResult> DeleteProduct([FromForm] long id)
        {
            await _productService.DeleteProduct(id);
            await _publishEndpoint.Publish<ProductDeleted>(new ProductDeleted()
            {
                Id = id,
                SellerId = GetUserIdFromToken(User)
            });
            return Ok();
        }

        [HttpGet("seller-get")]
        [Authorize(Roles = "SELLER")]
        public async Task<IActionResult> GetSellerProducts()
        {
            return Ok(await _productService.GetSellerProducts(GetUserIdFromToken(User)));
        }

        [HttpPut("upload-image")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "SELLER")]
        public async Task<IActionResult> UploadImage([FromForm] ProductUploadImageDTO article)
        {
            await _productService.UploadImage(article.Id, article.File);
            return Ok();
        }

        [HttpGet("get-image")]
        [Authorize(Roles = "ADMIN, BUYER, SELLER")]
        public async Task<IActionResult> GetImage(long id)
        {
            ProductImageDTO articleDTO = await _productService.GetProductImage(id);
            return Ok(articleDTO);
        }
    }
}
