using AutoMapper;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian.Domain.Account
{
    public class Details
    {
        public class Query : IRequest<CommandResult<AccountDto>>
        {
            public Query(Guid id)
            {
                AccountId = id;
            }

            public Guid AccountId { get; }
        }

        public class QueryHandler : IRequestHandler<Query, CommandResult<AccountDto>>
        {
            private readonly IAccountRepository _repository;
            private readonly IMapper _mapper;

            public QueryHandler(IAccountRepository repository,
                IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<CommandResult<AccountDto>> Handle(Query message, CancellationToken cancellationToken)
            {
                var account = await _repository.GetById(message.AccountId);

                return new CommandResult<AccountDto>()
                {
                    Result = _mapper.Map<AccountDto>(account),
                    IsSucceeded = true
                };
            }
        }
    }
}
