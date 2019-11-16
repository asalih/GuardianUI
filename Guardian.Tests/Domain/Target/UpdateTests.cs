using Guardian.Domain.Target;
using System.Threading.Tasks;
using Xunit;

namespace Guardian.Tests.Domain.Target
{
    public class UpdateTests : SliceFixture
    {
        [Fact]
        public async Task Expect_Edit_Target()
        {
            var targetCreateResult = await SendAsync(new Add.Command
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

            var updateTarget = targetCreateResult.Result;
            updateTarget.Domain = "www.fsparker.com";
            updateTarget.OriginIpAddress = "3.3.3.3";
            updateTarget.Proto = Infrastructure.Entity.Protocol.Https;
            updateTarget.State = Infrastructure.Entity.TargetState.Redirected;
            updateTarget.UseHttps = false;
            updateTarget.WAFEnabled = false;

            var updateTargetResult = await SendAsync(new Update.Command()
            {
                Target = updateTarget
            });

            Assert.NotNull(updateTargetResult);
            Assert.True(updateTargetResult.IsSucceeded);
            Assert.NotSame(updateTarget, updateTargetResult.Result);
        }
    }
}
