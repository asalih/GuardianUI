using Guardian.Domain.Target;
using System.Threading.Tasks;
using Xunit;

namespace Guardian.Tests.Domain.Target
{
    public class ListTests : SliceFixture
    {
        [Fact]
        public async Task Expect_List_Of_Targets()
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

            var listResult = await SendAsync(new List.Query());

            Assert.NotNull(listResult);
            Assert.True(listResult.IsSucceeded);
            Assert.True(listResult.Count > 0);
            Assert.True(listResult.Result.Count > 0);
        }
    }
}
