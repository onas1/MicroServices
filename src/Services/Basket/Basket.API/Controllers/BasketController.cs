using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcClientService _discountGrpcClientService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public BasketController(IBasketRepository repository, DiscountGrpcClientService discountGrpcClientService
            , IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _discountGrpcClientService = discountGrpcClientService;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);

            //basket ?? new ShoppingCart(userName) means if basket is null
            //create a new basket with the username for the user.
            return Ok(basket ?? new ShoppingCart(userName));
        }


        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket(ShoppingCart basket)
        {
            //TODO: Communicate with Discount.Grpc. this is done by connecting the gRPC server application in add connected service  to the client application.
            //and calculate latest process of product into shopping cart.
            //consume Discount gRpc. create a new class for this (DiscountGrpcClientService).

            foreach (var item in basket.Items)
            {
                //consuming the class we created.
                var coupon = await _discountGrpcClientService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }

            return Ok(await _repository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return Ok();
        }


        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> checkout([FromBody] BasketCheckout basketCheckout)
        {
            //get existing basket with total price
            //create basketCheckoutEvent -- set TotalPrice on basketCheckout eventMessage
            //send checkout event to rabbitmq using massTransit.
            //remove the basket

            //get existing basket with total price
            var basket = await _repository.GetBasket(basketCheckout.UserName);
            if (basket == null) return NotFound();


            //create basketCheckoutEvent -- set TotalPrice on basketCheckout eventMessage


            //send checkout event to rabbitmq using massTransit.

            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMessage);
            //_eventBus.PublisBasketCheckout;

            //remove the basket
            await _repository.DeleteBasket(basket.UserName);
            return Accepted();
        }

    }
}
