using System;
using System.IO;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Guardian.Domain.Account;
using Guardian.Infrastructure.Data;
using Guardian.Infrastructure.Entity;
using Guardian.Infrastructure.Security;
using Guardian.Web.UI;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Guardian.Tests
{
    public class SliceFixture
    {
        static readonly IConfiguration Config;

        static Account TestAccount;
        private static object Locker = new object();

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ServiceProvider _provider;
        private readonly string DbName = Guid.NewGuid() + ".db";

        static SliceFixture()
        {
            Config = new ConfigurationBuilder()
                .AddInMemoryCollection()
               .Build();
        }

        public SliceFixture()
        {
            var startup = new Startup(Config);
            var services = new ServiceCollection();

            var options = new DbContextOptionsBuilder<GuardianDataContext>()
               .UseInMemoryDatabase(databaseName: DbName)
               .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
               .Options;

            services.AddSingleton(new GuardianDataContext(options));

            startup.ConfigureServices(services);

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext()
            {
                // How mock RequestServices?
                RequestServices = serviceProviderMock.Object
            };

            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            services.AddSingleton(mockHttpContextAccessor.Object);
            services.AddSingleton(serviceProviderMock.Object);

            _provider = services.BuildServiceProvider();

            GetDbContext().Database.EnsureCreated();
            _scopeFactory = _provider.GetService<IServiceScopeFactory>();

            var identityHelper = new IdentityHelper(mockHttpContextAccessor.Object);
            context.User = identityHelper.CreateIdentity(GetGlobalTestAccount());
        }

        public GuardianDataContext GetDbContext()
        {
            return _provider.GetRequiredService<GuardianDataContext>();
        }

        public void Dispose()
        {
            File.Delete(DbName);
        }

        public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                await action(scope.ServiceProvider);
            }
        }

        public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                return await action(scope.ServiceProvider);
            }
        }

        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetService<IMediator>();

                return mediator.Send(request);
            });
        }

        public Task SendAsync(IRequest request)
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetService<IMediator>();

                return mediator.Send(request);
            });
        }

        public Task ExecuteDbContextAsync(Func<GuardianDataContext, Task> action)
        {
            return ExecuteScopeAsync(sp => action(sp.GetService<GuardianDataContext>()));
        }

        public Task<T> ExecuteDbContextAsync<T>(Func<GuardianDataContext, Task<T>> action)
        {
            return ExecuteScopeAsync(sp => action(sp.GetService<GuardianDataContext>()));
        }

        public Task InsertAsync(params object[] entities)
        {
            return ExecuteDbContextAsync(db =>
            {
                foreach (var entity in entities)
                {
                    db.Add(entity);
                }
                return db.SaveChangesAsync();
            });
        }

        private Account GetGlobalTestAccount()
        {
            lock (Locker)
            {
                if(TestAccount == null)
                {
                    var email = "testglobal@guardian-waf.com";
                    var pwd = "1q2w3e4r";
                    var command = new SignUp.Command()
                    {
                        Account = new SignUp.AccountSignUpDto
                        {
                            Email = email,
                            FullName = "Test User",
                            Password = pwd,
                            PasswordAgain = pwd
                        }
                    };

                    var result = SendAsync(command).GetAwaiter().GetResult();

                    var loginCmd = new Login.Command()
                    {
                        Account = new Login.AccountLoginDto()
                        {
                            Email = email,
                            Password = pwd
                        }
                    };

                    var accountDto = SendAsync(loginCmd)
                        .GetAwaiter()
                        .GetResult()
                        .Result;

                    TestAccount = new Account()
                    {
                        CreatedAt = DateTime.Now,
                        Email = accountDto.Email,
                        FullName = accountDto.FullName,
                        Id = accountDto.Id,
                        Role = "user"
                    };
                }

                return TestAccount;
            }
        }
    }
}
