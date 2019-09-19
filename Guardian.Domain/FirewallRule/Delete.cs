using AutoMapper;
using FluentValidation;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian.Domain.FirewallRule
{
    public class Delete
    {
        public class Command : IRequest<CommandResult<FirewallRuleDto>>
        {
            public FirewallRuleDto FirewallRule { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                DefaultValidatorExtensions.NotNull(RuleFor(x => x.FirewallRule.Id)).NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Command, CommandResult<FirewallRuleDto>>
        {
            private readonly IFirewallRuleRepository _repository;
            private readonly IMapper _mapper;

            public QueryHandler(IFirewallRuleRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<CommandResult<FirewallRuleDto>> Handle(Command message, CancellationToken cancellationToken)
            {
                var wafRule = await _repository.GetById(message.FirewallRule.Id);

                if (wafRule == null)
                {
                    return new CommandResult<FirewallRuleDto>()
                    {
                        IsSucceeded = false
                    };
                }

                return new CommandResult<FirewallRuleDto>()
                {
                    IsSucceeded = await _repository.Remove(wafRule) > 0
                };
            }
        }
    }
}
