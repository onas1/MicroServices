using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services;
using System.Net;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketService;
        private readonly IOrderingService _orderingService;

        public ShoppingController(ICatalogService catalogService, IBasketService basketService, IOrderingService orderingService)
        {
            _catalogService = catalogService;
            _basketService = basketService;
            _orderingService = orderingService;
        }

        [HttpGet("{userName}", Name = "GetShopping")]
        [ProducesResponseType(typeof(ShoppingModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingModel>> GetShopping(string userName)
        {
            //get basket with username
            //iterate basket items and consume products with basket item productId member.
            //map product related members into basketItem dto with extended columns
            //consume ordering microservices to retrieve order list
            //return root shoppingModel dto class which includes all responses.

            var basket = await _basketService.GetBasket(userName);

            foreach (var item in basket.Items)
            {
                var product = await _catalogService.GetCatalog(item.ProductId);
                //set additional product fields into basket item
                item.ProductName = product.Name;
                item.Category = product.Category;
                item.Summery = product.Summary;
                item.ImageFile = product.ImageFile;
            }

            var orders = await _orderingService.GetOrdersByUserName(userName);

            var shoppingModel = new ShoppingModel
            {
                UserName = userName,
                BasketWithProducts = basket,
                Orders = orders
            };
            return Ok(shoppingModel);
        }
    }
}
