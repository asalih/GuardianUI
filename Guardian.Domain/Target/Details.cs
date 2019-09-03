using AutoMapper;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian.Domain.Target
{
    public class Details
    {
        public class Query : IRequest<CommandResult<TargetDto>>
        {
            public Query(Guid id)
            {
                TargetId = id;
            }

            public Guid TargetId { get; }
        }

        public class QueryHandler : IRequestHandler<Query, CommandResult<TargetDto>>
        {
            private readonly ITargetRepository _repository;
            private readonly IMapper _mapper;

            public QueryHandler(ITargetRepository repository,
                IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<CommandResult<TargetDto>> Handle(Query message, CancellationToken cancellationToken)
            {
                var target = await _repository.GetById(message.TargetId);

                return new CommandResult<TargetDto>()
                {
                    Result = _mapper.Map<TargetDto>(target),
                    IsSucceeded = true
                };
            }
        }
    }
}
