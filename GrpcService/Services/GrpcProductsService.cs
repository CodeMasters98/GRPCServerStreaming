using Grpc.Core;
using GrpcService.Data;
using GrpcService.Protos;
using Microsoft.EntityFrameworkCore;

namespace GrpcService.Services
{
    public class GrpcProductsService:ProductsService.ProductsServiceBase
    {
        private readonly AppDbContext _dbContext;

        public GrpcProductsService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task GetAllProducts(GetProductsRequestDto request, IServerStreamWriter<ProductDto> responseStream, ServerCallContext context)
        {
            var products = await _dbContext.Products.ToListAsync();
            foreach (var product in products)
            {
                await responseStream.WriteAsync(new ProductDto
                {
                    Id= product.Id,
                    Name= product.Name
                });
            }
        }
    }
}
