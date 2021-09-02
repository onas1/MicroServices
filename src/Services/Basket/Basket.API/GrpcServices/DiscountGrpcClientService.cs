using Discount.Grpc.Protos;
using System.Threading.Tasks;

namespace Basket.API.GrpcServices
{
    public class DiscountGrpcClientService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient _discountGrpcProtoService;

        public DiscountGrpcClientService(DiscountProtoService.DiscountProtoServiceClient discountGrpcProtoService)
        {
            _discountGrpcProtoService = discountGrpcProtoService;
        }

        public async Task<couponModel> GetDiscount(string productName)
        {
            //Setting discount Grpc parameter
            var discountRequest = new GetDiscountRequest { ProductName = productName };

            //consuming discount Grpc.
            return await _discountGrpcProtoService.GetDiscountAsync(discountRequest);
        }
    }
}
