using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UdemyRabbitMQWebExcelCreate.Models;

namespace UdemyRabbitMQWebExcelCreate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using(var scope = host.Services.CreateScope() )
            {
                //getrequired servis getservis den farký bulamazsa hata fýrlatýr. 
                var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                appDbContext.Database.Migrate();

                if (!appDbContext.Users.Any())
                {
                    UserManager.CreateAsync ( new IdentityUser() { UserName="Deneme",Email="deneme@outlook.com"},"Password12").Wait();
                    UserManager.CreateAsync ( new IdentityUser() { UserName="Deneme2",Email="deneme2@outlook.com"},"Password12").Wait();
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
