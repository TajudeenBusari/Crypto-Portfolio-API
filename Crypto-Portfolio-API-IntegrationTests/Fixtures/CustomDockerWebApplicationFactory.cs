
using Crypto_Portfolio_API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;


namespace Crypto_Portfolio_API_IntegrationTests.Fixtures
{
    public class CustomDockerWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgreSqlContainer;

        public CustomDockerWebApplicationFactory()
        {
            _postgreSqlContainer = new PostgreSqlBuilder()
                .WithImage("postgres:16")
                .Build();

        }
        
        //Configure web host for the container
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var connectionString = _postgreSqlContainer.GetConnectionString();
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(service =>
            {
                service.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
                service.AddDbContext<ApplicationDbContext>(option =>
                    option.UseNpgsql(connectionString));
            });
        }
        
        //Initialize container
        public async Task InitializeAsync()
        {
            await _postgreSqlContainer.StartAsync();

            using (var scope = Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                await db.Database.EnsureCreatedAsync();
                await db.SaveChangesAsync();
                
            }
           
        }

        //stop container
        public async Task DisposeAsync()
        {
            await _postgreSqlContainer.StopAsync();
        }
    }
}
