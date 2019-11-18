using AutoMapper;
using FluentValidation;
using Guardian.Infrastructure.Repository.Specs;
using Guardian.Infrastructure.Security.Specs;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian.Domain.Account
{
    public class Update
    {
        public class Command : IRequest<CommandResult<AccountDto>>
        {
            public AccountDto Account { get; set; }
        }

        public class AccountSignUpDtoValidator : AbstractValidator<AccountDto>
        {
            public AccountSignUpDtoValidator()
            {
                DefaultValidatorExtensions.NotNull(RuleFor(x => x.Email)).NotEmpty().MinimumLength(4).EmailAddress();

                RuleFor(x => x.Password).MinimumLength(8);
                RuleFor(x => x.PasswordAgain).MinimumLength(8).Equal(x => x.Password).WithMessage("Passwords must match");
            }
        }

        public class QueryHandler : IRequestHandler<Command, CommandResult<AccountDto>>
        {
            private readonly IAccountRepository _repository;
            private readonly IMapper _mapper;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IIdentityHelper _identityHelper;


            public QueryHandler(IAccountRepository repository, 
                IMapper mapper, 
                IHttpContextAccessor httpContextAccessor,
                IIdentityHelper identityHelper)
            {
                _repository = repository;
                _mapper = mapper;
                _httpContextAccessor = httpContextAccessor;
                _identityHelper = identityHelper;
            }

            public async Task<CommandResult<AccountDto>> Handle(Command message, CancellationToken cancellationToken)
            {
                var account = await _repository.GetById(message.Account.Id);

                if (account == null)
                {
                    return new CommandResult<AccountDto>()
                    {
                        IsSucceeded = false
                    };
                }

                if (message.Account.ReGenerateToken)
                {
                    return await UpdateToken(account);
                }

                if (account.Email != message.Account.Email)
                {
                    var newEmailAccount = await _repository.GetByEmailAddress(message.Account.Email);

                    if (newEmailAccount != null)
                    {
                        return new CommandResult<AccountDto>()
                        {
                            IsSucceeded = false,
                            Message = "Email address is in use!"
                        };
                    }
                }

                account.FullName = message.Account.FullName;
                account.Email = message.Account.Email;

                if(!string.IsNullOrEmpty(message.Account.Password) &&
                    !string.IsNullOrEmpty(message.Account.PasswordAgain))
                {
                    account.Password = PasswordHelper.GeneratePassword(message.Account.Password, out var salt);
                    account.Salt = salt;
                }

                await _repository.Update(account);

                var principal = _identityHelper.CreateIdentity(account);

                await _httpContextAccessor.HttpContext.SignInAsync(principal);

                return new CommandResult<AccountDto>()
                {
                    IsSucceeded = true,
                    Result = _mapper.Map<AccountDto>(account)
                };
            }

            private async Task<CommandResult<AccountDto>> UpdateToken(Infrastructure.Entity.Account account)
            {
                account.Token = TokenHelper.GenerateToken();

                await _repository.Update(account);

                return new CommandResult<AccountDto>()
                {
                    IsSucceeded = true,
                    Result = _mapper.Map<AccountDto>(account)
                };
            }
        }
    }
}
