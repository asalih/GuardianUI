using Guardian.Domain.Target;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Guardian.Tests.Domain.Target
{
    public class DetailTests : SliceFixture
    {
        [Fact]
        public async Task Expect_Create_WithAutoCert_Target()
        {
            var targetCreate = new Add.Command()
            {
                Target = new TargetDto()
                {
                    AutoCert = true,
                    CreateSelfSignedCertificate = false,
                    Domain = "www.netsparker.com",
                    OriginIpAddress = "6.6.6.6",
                    Proto = Infrastructure.Entity.Protocol.Http,
                    State = Infrastructure.Entity.TargetState.Created,
                    UseHttps = true,
                    WAFEnabled = true
                }
            };

            var targetCreateResult = await SendAsync(targetCreate);

            Assert.NotNull(targetCreateResult);
            Assert.True(targetCreateResult.IsSucceeded);

            var detailResult = await SendAsync(new Details.Query(targetCreateResult.Result.Id));

            Assert.NotNull(detailResult);
            Assert.True(detailResult.IsSucceeded);
            Assert.Equal("www.netsparker.com", detailResult.Result.Domain);
        }
    }
}
