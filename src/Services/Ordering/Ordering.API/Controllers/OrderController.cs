using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Ordering.Application.Features.Orders.Commands.DeleteOrder;
using Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Ordering.Application.Features.Orders.Queries.GetOrdersList;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Ordering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("{userName}", Name = "GetOrder")]
        [ProducesResponseType(typeof(IEnumerable<OrdersVm>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrderByUserName(string userName)
        {

            //using the query we created with mediatR in
            //ordering.Application using irequest class.
            var query = new GetOrdersListQuery(userName);

            //sending query for IRequestHandler to
            //execute with mediatR nuget package from
            //ordering.application MedaitR nuget package. Clean architecture.
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }


        //testing purpose only.

        [HttpPost(Name = "CheckoutOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckoutOrder([FromBody] CheckoutOrderCommand command)
        {


            var result = await _mediator.Send(command);
            return Ok(result);
        }




        [HttpPut(Name = "UpdateOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderCommand command)
        {



            //sending command for IRequestHandler to
            //execute with mediatR nuget package from
            //ordering.application MedaitR nuget package. Clean architecture.
            var orders = await _mediator.Send(command);


            return NoContent();
        }


        [HttpDelete("{id}", Name = "DeleteOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(int Id)
        {
            var command = new DeleteOrderCommand() { Id = Id };

            await _mediator.Send(command);
            return NoContent();
        }
    }
}
