using AutoMapper;
using FluentValidation;
using Guardian.Domain.CryptoUtility;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Threading;
using System.Threading.Tasks;
using Guardian.Infrastructure.Security.Specs;

namespace Guardian.Domain.Account
{
    public class TokenLogin
    {
        public class Command : IRequest<CommandResult<AccountDto>>
        {
            public Guid Id { get; set; }
            public string Token { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Token).NotNull().NotEmpty();
                RuleFor(x => x.Id).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, CommandResult<AccountDto>>
        {
            private readonly IAccountRepository _accountRepository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IIdentityHelper _identityHelper;

            public Handler(IAccountRepository accountRepository,
                IMapper mapper,
                IHttpContextAccessor httpContextAccessor,
                IIdentityHelper identityHelper)
            {
                _accountRepository = accountRepository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
                _identityHelper = identityHelper;
            }

            public async Task<CommandResult<AccountDto>> Handle(Command message, CancellationToken cancellationToken)
            {
                var account = await _accountRepository.GetByToken(message.Id, message.Token);

                if (account == null)
                {
                    return new CommandResult<AccountDto>(false);
                }

                var principal = _identityHelper.CreateIdentity(account);

                await _httpContextAccessor.HttpContext.SignInAsync(principal);

                return new CommandResult<AccountDto>(_mapper.Map<AccountDto>(account), true);
            }
        }
    }
}
