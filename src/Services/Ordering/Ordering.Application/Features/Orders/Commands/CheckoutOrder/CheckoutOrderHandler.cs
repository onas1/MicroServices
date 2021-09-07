using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{

    //IRequestHandler takes two parameter which is the command or query object known as TReqest and the expected response ie what the method returns knows as TResponse.
    //in this case, the IRequest is a CheckOutOrderCommand model (that inherited MediatR IRequest Interface) and the response is and integer. As seen in the handle method.
    public class CheckoutOrderHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<CheckoutOrderHandler> _logger;

        public CheckoutOrderHandler(ILogger<CheckoutOrderHandler> logger, IEmailService emailService, IMapper mapper, IOrderRepository repository)
        {
            _logger = logger;
            _emailService = emailService;
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = _mapper.Map<Order>(request);
            var newOrder = await _repository.AddAsync(orderEntity);
            _logger.LogInformation($"Order {newOrder.Id} is successfully created.");
            await SendMail(newOrder);

            return newOrder.Id;
        }

        private async Task SendMail(Order order)
        {
            var email = new Email() { To = "iriajenfrancis@gmail.com", Body = $"Order was created.", Subject = "Order was created" };

            try
            {
                await _emailService.SendEmail(email);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Order {order.Id} failed due to an error with the mail service: {ex.Message}");
            }
        }
    }
}
