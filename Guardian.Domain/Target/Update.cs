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

            public Guid TargetId { get; set; }

            public bool IsReCreateCertificateCommand { get; set; }
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
                if (message.IsReCreateCertificateCommand)
                {
                    return await ReCreateCertificate(message.TargetId);
                }

                var theTargetWithDomain = await _repository.GetTargetWithTheDomain(message.Target.Domain);

                var target = await _repository.GetById(message.Target.Id);

                if (target == null ||
                    (theTargetWithDomain != null &&
                    theTargetWithDomain.Id != target.Id))
                {
                    return new CommandResult<TargetDto>()
                    {
                        IsSucceeded = false
                    };
                }

                target.OriginIpAddress = message.Target.OriginIpAddress;
                target.UseHttps = message.Target.UseHttps;
                target.CertCrt = message.Target.CertCrt;
                target.CertKey = message.Target.CertKey;

                if (target.Domain != message.Target.Domain)
                {
                    var ssl = SSLHelper.CreateSSL(message.Target.Domain);

                }

                await _repository.Update(target);

                return new CommandResult<TargetDto>()
                {
                    IsSucceeded = true,
                    Result = _mapper.Map<TargetDto>(target)
                };
            }

            private async Task<CommandResult<TargetDto>> ReCreateCertificate(Guid id)
            {
                var target = await _repository.GetById(id);

                if (target == null)
                {
                    return new CommandResult<TargetDto>()
                    {
                        IsSucceeded = false
                    };
                }

                var ssl = SSLHelper.CreateSSL(target.Domain);

                target.CertKey = ssl.CertKey;
                target.CertCrt = ssl.CertCrt;

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
