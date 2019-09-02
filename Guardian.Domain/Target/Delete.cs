using AutoMapper;
using FluentValidation;
using Guardian.Infrastructure.Repository.Specs;
using Guardian.Infrastructure.Security.Specs;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian.Domain.Target
{
    public class Delete
    {
        public class Command : IRequest<CommandResult<TargetDto>>
        {
            public TargetDto Target { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                DefaultValidatorExtensions.NotNull(RuleFor(x => x.Target.Id)).NotEmpty();
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
                var target = await _repository.GetById(message.Target.Id);

                if (target == null)
                {
                    return new CommandResult<TargetDto>()
                    {
                        IsSucceeded = false
                    };
                }

                return new CommandResult<TargetDto>()
                {
                    IsSucceeded = await _repository.Remove(target) > 0
            };
            }
        }
    }
}
