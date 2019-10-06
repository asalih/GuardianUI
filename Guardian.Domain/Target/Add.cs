using AutoMapper;
using FluentValidation;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Guardian.Infrastructure.Security.Specs;
using System.Net;
using System.Linq;
using Guardian.Infrastructure.Entity;
using RestSharp;

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
                        IsSucceeded = false,
                        Message = "Target in use."
                    };
                }

                message.Target.UseHttps = true;
                message.Target.WAFEnabled = true;

                var target = _mapper.Map<Infrastructure.Entity.Target>(message.Target);
                //TODO: Add verification process.
                target.State = TargetState.Redirected;

                if (message.Target.CreateSelfSignedCertificate)
                {
                    var sslCert = SSLHelper.CreateSSL(target.Domain);

                    target.CertCrt = sslCert.CertCrt;
                    target.CertKey = sslCert.CertKey;
                }

                var ipAddress = await Dns.GetHostEntryAsync(target.Domain);

                target.OriginIpAddress = ipAddress.AddressList.FirstOrDefault()?.ToString() ?? null;
                target.Proto = await ObtainProtocol(target.Domain);

                await _repository.Add(target);

                return new CommandResult<TargetDto>()
                {
                    IsSucceeded = true,
                    Result = _mapper.Map<TargetDto>(target)
                };
            }

            private async Task<Protocol> ObtainProtocol(string domain)
            {
                var client = new RestClient("http://" + domain);
                var request = new RestRequest("/", Method.GET);

                request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36");
                request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
                request.AddHeader("Accept-Encoding", "gzip, deflate, br");
                request.AddHeader("Accept-Language", "en,tr;q=0.9,en-US;q=0.8,tr-TR;q=0.7");
                request.AddHeader("Cache-Control", "max-age=0");

                client.FollowRedirects = false;
                var response = await client.ExecuteGetTaskAsync(request);

                var code = (int)response.StatusCode;
                if (response.ResponseStatus == ResponseStatus.Completed &&
                    code >= 300 && code < 400)
                {
                    return response.Headers.Any(s => s.Name.ToLowerInvariant() == "location" && s.Value.ToString().StartsWith("https:")) ?
                        Protocol.Https :
                        Protocol.Http;
                }

                return Protocol.Http;
            }
        }
    }
}
