using Guardian.Domain.Target;
using System.Threading.Tasks;
using Xunit;

namespace Guardian.Tests.Domain.Target
{
    public class DeleteTests : SliceFixture
    {
        [Fact]
        public async Task Expect_Delete_Target()
        {
            var targetCreateResult = await SendAsync(new Guardian.Domain.Target.Add.Command
            {
                Target = new TargetDto
                {
                    AutoCert = true,
                    CreateSelfSignedCertificate = false,
                    Domain = "www.gsparker.com",
                    OriginIpAddress = "2.2.2.2",
                    Proto = Infrastructure.Entity.Protocol.Http,
                    State = Infrastructure.Entity.TargetState.Created,
                    UseHttps = true,
                    WAFEnabled = true
                }
            });

            Assert.NotNull(targetCreateResult);
            Assert.True(targetCreateResult.IsSucceeded);

            var deleteResult = await SendAsync(new Guardian.Domain.Target.Delete.Command()
            {
                Target = targetCreateResult.Result
            });

            Assert.NotNull(deleteResult);
            Assert.True(deleteResult.IsSucceeded);

            var queryResult = await SendAsync(new Guardian.Domain.Target.Details.Query(targetCreateResult.Result.Id));

            Assert.NotNull(queryResult);
            Assert.True(queryResult.IsSucceeded);
            Assert.Null(queryResult.Result);
        }

        [Fact]
        public async Task Expect_Delete_Target_With_FirewallRule()
        {
            var targetCreateResult = await SendAsync(new Guardian.Domain.Target.Add.Command
            {
                Target = new TargetDto
                {
                    AutoCert = true,
                    CreateSelfSignedCertificate = false,
                    Domain = "www.gsparker.com",
                    OriginIpAddress = "2.2.2.2",
                    Proto = Infrastructure.Entity.Protocol.Http,
                    State = Infrastructure.Entity.TargetState.Created,
                    UseHttps = true,
                    WAFEnabled = true
                }
            });

            Assert.NotNull(targetCreateResult);
            Assert.True(targetCreateResult.IsSucceeded);

            var firewallRuleCreateResult = await SendAsync(new Guardian.Domain.FirewallRule.Add.Command
            {
                FirewallRule = new Guardian.Domain.FirewallRule.FirewallRuleDto
                {
                    Expression = "SecRule TX:EXECUTING_PARANOIA_LEVEL \"@lt 1\" \"id:913011,phase:1,pass,nolog,skipAfter:END-REQUEST-913-SCANNER-DETECTION\"",
                    IsActive = true,
                    RuleFor = Infrastructure.Entity.RuleFor.Request,
                    TargetId = targetCreateResult.Result.Id,
                    Title = "title"
                }
            });

            Assert.NotNull(firewallRuleCreateResult);
            Assert.True(firewallRuleCreateResult.IsSucceeded);

            var deleteResult = await SendAsync(new Guardian.Domain.Target.Delete.Command()
            {
                Target = targetCreateResult.Result
            });

            Assert.NotNull(deleteResult);
            Assert.True(deleteResult.IsSucceeded);

            var queryResult = await SendAsync(new Guardian.Domain.Target.Details.Query(targetCreateResult.Result.Id));

            Assert.NotNull(queryResult);
            Assert.True(queryResult.IsSucceeded);
            Assert.Null(queryResult.Result);

            var firewallRuleQueryResult = await SendAsync(new Guardian.Domain.FirewallRule.Details.Query(firewallRuleCreateResult.Result.Id));

            Assert.NotNull(firewallRuleQueryResult);
            Assert.True(firewallRuleQueryResult.IsSucceeded);
            Assert.Null(firewallRuleQueryResult.Result);
        }
    }
}
