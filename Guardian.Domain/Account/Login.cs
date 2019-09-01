using AutoMapper;
using FluentValidation;
using Guardian.Domain.CryptoUtility;
using Guardian.Domain.Security.Specs;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian.Domain.Account
{
    public class Login
    {
        public class AccountLoginDto
        {
            public string Email { get; set; }

            public string Password { get; set; }
        }

        public class AccountLoginDtoValidator : AbstractValidator<AccountLoginDto>
        {
            public AccountLoginDtoValidator()
            {
                RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
                RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(8);
            }
        }

        public class Command : IRequest<CommandResult<AccountDto>>
        {
            public AccountLoginDto Account { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Account).NotNull().SetValidator(new AccountLoginDtoValidator());
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
                var account = await _accountRepository.GetByEmailAddress(message.Account.Email);

                if (account == null)
                {
                    return new CommandResult<AccountDto>(false);
                }

                if (!CryptoHelper.CompareHash(message.Account.Password, account.Password, Convert.FromBase64String(account.Salt)))
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
