using Discount.Grpc.Entities;
using Discount.Grpc.Protos;

namespace Discount.Grpc.Profile
{
    public class ProfileMapper : AutoMapper.Profile
    {
        public ProfileMapper()
        {
            CreateMap<Coupon, couponModel>().ReverseMap();
        }
    }
}
