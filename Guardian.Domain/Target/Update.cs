using AutoMapper;
using Guardian.Infrastructure.Repository.Specs;
using Guardian.Infrastructure.Security.Specs;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian.Domain.Target
{
    public class Update
    {
        public class Command : IRequest<CommandResult<TargetDto>>
        {
            public TargetDto Target { get; set; }
        }

        public class QueryHandler : IRequestHandler<Command, CommandResult<TargetDto>>
        {
            private readonly ITargetRepository _repository;
            private readonly IMapper _mapper;

            public QueryHandler(ITargetRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<CommandResult<TargetDto>> Handle(Command message, CancellationToken cancellationToken)
            {
                var theTargetWithDomain = await _repository.GetTargetWithTheDomain(message.Target.Domain);

                var target = await _repository.GetById(message.Target.Id);

                if(theTargetWithDomain != null && 
                    theTargetWithDomain.Id != target.Id)
                {
                    return new CommandResult<TargetDto>()
                    {
                        IsSucceeded = false
                    };
                }

                target.Port = message.Target.Port;
                target.OriginIpAddress = message.Target.OriginIpAddress;
                target.CertCrt = message.Target.CertCrt;
                target.CertKey = message.Target.CertKey;

                await _repository.Update(target);

                return new CommandResult<TargetDto>()
                {
                    IsSucceeded = true,
                    Result = _mapper.Map<TargetDto>(target)
                };
            }
        }
    }
}
