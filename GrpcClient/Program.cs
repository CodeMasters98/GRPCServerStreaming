using Grpc.Core;
using Grpc.Net.Client;
using GrpcService.Protos;
using System.Diagnostics;

var sp = new Stopwatch();
var channel = GrpcChannel.ForAddress("https://localhost:7161");

var client = new SalesService.SalesServiceClient(channel);

using var call = client.GetSalesData(new GetSalesRequest
{
    Input = new Google.Protobuf.WellKnownTypes.Empty(),
});

await foreach (var data in call.ResponseStream.ReadAllAsync())
{
    Console.WriteLine($"New Order Receieved from {data.Country}-{data.Region},Order ID = {data.OrderId}, Unit Price ={data.UnitPrice}, Ship Date={data.ShipDate}");
}

//var client=new ProductsService.ProductsServiceClient(channel);

//using var call = client.GetAllProducts(new GetProductsRequestDto { Input = new Google.Protobuf.WellKnownTypes.Empty()});
//sp.Start();
//await foreach (var product in call.ResponseStream.ReadAllAsync())
//{
//    Console.WriteLine($"product {product.Id} with name {product.Name} recived");
//}
//sp.Stop();
//Console.WriteLine($"streaming ended in {sp.Elapsed.TotalSeconds} sec");
Console.ReadLine();