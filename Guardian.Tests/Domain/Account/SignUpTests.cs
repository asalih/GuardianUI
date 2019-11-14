using Guardian.Domain.Account;
using System.Threading.Tasks;
using Xunit;
using FluentValidation.TestHelper;

namespace Guardian.Tests.Domain.Account
{
    public class SignUpTests : SliceFixture
    {
        [Fact]
        public async Task Expect_Create_Account()
        {
            var command = new SignUp.Command()
            {
                Account = new SignUp.AccountSignUpDto
                {
                    Email = "test@guardian-waf.com",
                    FullName = "Test User",
                    Password = "1q2w3e",
                    PasswordAgain = "1q2w3e"
                }
            };

            var result = await SendAsync(command);

            Assert.NotNull(result);
            Assert.True(result.IsSucceeded);
        }

        [Fact]
        public async Task Expect_EmailAlreadyInUse_Account()
        {
            var command = new SignUp.Command()
            {
                Account = new SignUp.AccountSignUpDto
                {
                    Email = "test1@guardian-waf.com",
                    FullName = "Test User",
                    Password = "1q2w3e",
                    PasswordAgain = "1q2w3e"
                }
            };

            var result = await SendAsync(command);
            var resultInUse = await SendAsync(command);

            Assert.NotNull(result);
            Assert.True(result.IsSucceeded);
            Assert.NotNull(resultInUse);
            Assert.False(resultInUse.IsSucceeded);
            Assert.True(resultInUse.EmailInUse);
        }

        [Fact]
        public void Validation_Password_Not_Match_Account()
        {
            var account = new SignUp.AccountSignUpDto
            {
                Email = "test2@guardian-waf.com",
                FullName = "Test User",
                Password = "1q2w3e",
                PasswordAgain = "1q2w3e4r"
            };

            var validator = new SignUp.AccountSignUpDtoValidator();

            validator.ShouldHaveValidationErrorFor(acc => acc.PasswordAgain, account);
        }

        [Fact]
        public void Validation_Password_Length_Not_Enough_Account()
        {
            var account = new SignUp.AccountSignUpDto
            {
                Email = "test2@guardian-waf.com",
                FullName = "Test User",
                Password = "1",
                PasswordAgain = "1"
            };

            var validator = new SignUp.AccountSignUpDtoValidator();

            validator.ShouldHaveValidationErrorFor(acc => acc.Password, account);
        }

        [Fact]
        public void Validation_Email_Length_Not_Enough_Account()
        {
            var account = new SignUp.AccountSignUpDto
            {
                Email = "1",
                FullName = "Test User",
                Password = "1",
                PasswordAgain = "1"
            };

            var validator = new SignUp.AccountSignUpDtoValidator();

            validator.ShouldHaveValidationErrorFor(acc => acc.Email, account);
        }
    }
}
