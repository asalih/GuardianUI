using Guardian.Domain.Target;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Guardian.Tests.Domain.Target
{
    public class CreateTests : SliceFixture
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
                    Domain = "www.example.com",
                    OriginIpAddress = "2.2.2.2",
                    Proto = Infrastructure.Entity.Protocol.Http,
                    State = Infrastructure.Entity.TargetState.Created,
                    UseHttps = true,
                    WAFEnabled = true
                }
            };

            var targetCreateResult = await SendAsync(targetCreate);

            Assert.NotNull(targetCreateResult);
            Assert.True(targetCreateResult.IsSucceeded);
        }

        [Fact]
        public async Task Expect_Create_WithSelfCert_Target()
        {
            var targetCreate = new Add.Command()
            {
                Target = new TargetDto()
                {
                    AutoCert = false,
                    CreateSelfSignedCertificate = true,
                    Domain = "www.testsparker.com",
                    OriginIpAddress = "2.2.2.2",
                    Proto = Infrastructure.Entity.Protocol.Https,
                    State = Infrastructure.Entity.TargetState.Redirected,
                    UseHttps = true,
                    WAFEnabled = true
                }
            };

            var targetCreateResult = await SendAsync(targetCreate);

            Assert.NotNull(targetCreateResult);
            Assert.True(targetCreateResult.IsSucceeded);
        }
    }
}
