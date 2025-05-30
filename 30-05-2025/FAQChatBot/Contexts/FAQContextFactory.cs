using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using FAQChatBot.Contexts;
using System.IO;

namespace FAQChatBot.Contexts
{
    public class FAQContextFactory : IDesignTimeDbContextFactory<FAQContext>
    {
        public FAQContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FAQContext>();
            optionsBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"));

            return new FAQContext(optionsBuilder.Options);
        }
    }
}