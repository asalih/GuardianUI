using AutoMapper;
using FluentValidation;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Guardian.Infrastructure.Security.Specs;
using System.Net;
using System.Linq;

namespace Guardian.Domain.Target
{
    public class Add
    {
        public class Command : IRequest<CommandResult<TargetDto>>
        {
            public TargetDto Target { get; set; }
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
                var anyTarget = await _repository.GetTargetWithTheDomain(message.Target.Domain);

                if (anyTarget != null)
                {
                    return new CommandResult<TargetDto>()
                    {
                        IsSucceeded = false
                    };
                }

                var target = _mapper.Map<Infrastructure.Entity.Target>(message.Target);

                var sslCert = SSLHelper.CreateSSL(target.Domain);

                target.CertCrt = sslCert.CertCrt;
                target.CertKey = sslCert.CertKey;

                var ipAddress = await Dns.GetHostEntryAsync(target.Domain);

                target.OriginIpAddress = ipAddress.AddressList.FirstOrDefault()?.ToString() ?? null;

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
