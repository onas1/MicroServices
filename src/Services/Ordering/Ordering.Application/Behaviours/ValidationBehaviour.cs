using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValidationException = Ordering.Application.Exceptions.ValidationException;

namespace Ordering.Application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validotors)
        {
            _validators = validotors;
        }
        //implementation of validation behaviour using handle method inherited from IPiplineVehaviour<Tr, Trp> class.
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                //this code means select all validators one by one and perform validation,
                //using ValidateAsync() and when all validation is complete,
                //we returns a collection of validateResult;
                var validationResults =
                    await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));


                var failures = validationResults.SelectMany(r => r.Errors);
                if (failures.Count() != 0)
                {
                    throw new ValidationException(failures);
                }

            }
            //ver important to call the next method to continue
            //our requests pipeline or request transmitted intermediator.
            return await next();
        }
    }
}
