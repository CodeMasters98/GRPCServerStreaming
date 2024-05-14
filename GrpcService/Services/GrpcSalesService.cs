using Grpc.Core;
using GrpcService.Protos;

namespace GrpcService.Services
{
    public class GrpcSalesService:SalesService.SalesServiceBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GrpcSalesService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public override async Task GetSalesData(GetSalesRequest request, IServerStreamWriter<SalesDataDto> responseStream, ServerCallContext context)
        {
            try
            {
                string line = string.Empty;
                bool isFirstLine = true;
                using var reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "App_Data", "sales_records.csv"));
                while (!reader.EndOfStream)
                {
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        continue;
                    }
                    line = await reader.ReadLineAsync();
                    var data = line.Split(',');
                    var model = new SalesDataDto();
                    model.Region= data[0];
                    model.Country= data[1];
                    model.OrderId = int.TryParse(data[6], out int _orderId) ? _orderId : 0;
                    model.UnitPrice = float.TryParse(data[9], out float _unitPrice) ? _unitPrice : 0;
                    model.ShipDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime
                         ((DateTime.TryParse(data[7], out DateTime _dateShip) ? _dateShip : DateTime.MinValue).ToUniversalTime());
                    model.UnitsSold = int.TryParse(data[8], out int _unitsSold) ? _unitsSold : 0;

                    model.UnitCost = float.TryParse(data[10], out float _unitCost) ? _unitCost : 0;

                    model.TotalRevenue = int.TryParse(data[11], out int _totalRevenue) ? _totalRevenue : 0;
                    model.TotalCost = int.TryParse(data[13], out int _totalCost) ? _totalCost : 0;

                    await responseStream.WriteAsync(model);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
