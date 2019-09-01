using AutoMapper;
using FluentValidation;
using Guardian.Infrastructure.Repository.Specs;
using Guardian.Infrastructure;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Guardian.Domain.Security.Specs;

namespace Guardian.Domain.Target
{
    public class Add
    {
        public class Command : IRequest<CommandResult<TargetDto>>
        {
            public TargetDto Target { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                DefaultValidatorExtensions.NotNull(RuleFor(x => x.Target.Domain)).NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Command, CommandResult<TargetDto>>
        {
            private readonly ITargetRepository _repository;
            private readonly IMapper _mapper;
            private readonly IIdentityHelper _identityHelper;

            public QueryHandler(ITargetRepository repository, IMapper mapper, IIdentityHelper identityHelper)
            {
                _repository = repository;
                _mapper = mapper;
                _identityHelper = identityHelper;
            }

            public async Task<CommandResult<TargetDto>> Handle(Command message, CancellationToken cancellationToken)
            {
                var anyTarget = await _repository.GetByDomain(message.Target.Domain);

                if (anyTarget != null)
                {
                    return new CommandResult<TargetDto>()
                    {
                        IsSucceeded = false
                    };
                }

                var target = _mapper.Map<Infrastructure.Entity.Target>(message.Target);

                target.AccountId = _identityHelper.GetAccountId();

                await _repository.Add(target);

                return new CommandResult<TargetDto>()
                {
                    IsSucceeded = true,
                    Result = _mapper.Map<TargetDto>(target)
                };
            }
        }
    }
}
