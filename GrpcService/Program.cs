
using GrpcService.Data;
using GrpcService.Model;
using GrpcService.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("codecell"));
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GrpcProductsService>();
app.MapGrpcService<GrpcSalesService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.MapGet("/import", async (AppDbContext _context) =>
{
    List<Product> products = new();
	for (int i = 1; i <= 100_000; i++)
	{
		products.Add(new Product
		{
			Id = i,
			Name = $"product{i}"
		});
	}
	await _context.AddRangeAsync(products);
	await _context.SaveChangesAsync();
	return "data imported";
});

app.MapGet("/data", async (AppDbContext _context) =>
{
    var data=await _context.Products.ToListAsync();
    return JsonSerializer.Serialize(data,options:new JsonSerializerOptions
	{
		WriteIndented=true
	});
});
app.Run();
