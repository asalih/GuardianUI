using AutoMapper;
using FluentValidation;
using Guardian.Domain.CryptoUtility;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian.Domain.Account
{
    public class SignUp
    {
        public class AccountSignUpDto
        {
            public string Email { get; set; }

            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Password Again")]
            public string PasswordAgain { get; set; }

            [Display(Name = "Full Name")]
            public string FullName { get; set; }
        }

        public class Command : IRequest<AccountCommandResult>
        {
            public AccountSignUpDto Account { get; set; }
        }

        public class AccountSignUpDtoValidator : AbstractValidator<AccountSignUpDto>
        {
            public AccountSignUpDtoValidator()
            {
                DefaultValidatorExtensions.NotNull(RuleFor(x => x.Email)).NotEmpty().MinimumLength(4).EmailAddress();
                DefaultValidatorExtensions.NotNull(RuleFor(x => x.Password)).NotEmpty().MinimumLength(8);
                DefaultValidatorExtensions.NotNull(RuleFor(x => x.PasswordAgain)).NotEmpty().MinimumLength(8).Equal(x => x.Password);
            }
        }

        public class AccountCommandResult : CommandResult<AccountSignUpDto>
        {
            public bool EmailInUse { get; set; }
        }

        public class QueryHandler : IRequestHandler<Command, AccountCommandResult>
        {
            private readonly IAccountRepository _repository;
            private readonly IMapper _mapper;

            public QueryHandler(IAccountRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<AccountCommandResult> Handle(Command message, CancellationToken cancellationToken)
            {
                var hasEmail = await _repository.Query().CountAsync(s => s.Email == message.Account.Email) > 0;

                if (hasEmail)
                {
                    return new AccountCommandResult()
                    {
                        IsSucceeded = false,
                        EmailInUse = true
                    };
                }

                var account = _mapper.Map<Infrastructure.Entity.Account>(message.Account);
                account.Role = "User";

                account.Password = PasswordHelper.GeneratePassword(account.Password, out var salt);
                account.Salt = salt;
                account.Token = TokenHelper.GenerateToken();

                await _repository.Add(account);

                return new AccountCommandResult()
                {
                    IsSucceeded = true,
                    Result = _mapper.Map<AccountSignUpDto>(account)
                };
            }
        }
    }
}
