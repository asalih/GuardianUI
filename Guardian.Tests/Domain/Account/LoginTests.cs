using Guardian.Domain.Account;
using System.Threading.Tasks;
using Xunit;
using FluentValidation.TestHelper;

namespace Guardian.Tests.Domain.Account
{
    public class LoginTests : SliceFixture
    {
        [Fact]
        public async Task Expect_Login_Account()
        {
            var email = "test-login@guardian-waf.com";
            var pwd = "1q2w3e4r";

            var accountCreate = new SignUp.Command()
            {
                Account = new SignUp.AccountSignUpDto
                {
                    Email = email,
                    FullName = "Test User",
                    Password = pwd,
                    PasswordAgain = pwd
                }
            };

            var accountCreateResult = await SendAsync(accountCreate);

            Assert.NotNull(accountCreateResult);
            Assert.True(accountCreateResult.IsSucceeded);

            var command = new Login.Command()
            {
                Account = new Login.AccountLoginDto()
                {
                    Email = email,
                    Password = pwd
                }
            };

            var loginResult = await SendAsync(command);

            Assert.NotNull(loginResult);
            Assert.True(loginResult.IsSucceeded);
        }

        [Fact]
        public void Validatipn_Login_Email_Not_Match_Account()
        {
            var login = new Login.AccountLoginDto()
            {
                Email = "a",
                Password = "1q2w3e4r"
            };

            var validator = new Login.AccountLoginDtoValidator();

            validator.ShouldHaveValidationErrorFor(acc => acc.Email, login);
        }

        [Fact]
        public void Validatipn_Login_Password_Length_Not_Match_Account()
        {
            var login = new Login.AccountLoginDto()
            {
                Email = "test@guardian-waf.com",
                Password = "1"
            };

            var validator = new Login.AccountLoginDtoValidator();

            validator.ShouldHaveValidationErrorFor(acc => acc.Password, login);
        }
    }
}
