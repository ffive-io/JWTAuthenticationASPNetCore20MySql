using Microsoft.Extensions.Configuration;
using System.IO;

namespace FFive.API.Utils
{
    public static class Configuration
    {
        public static IConfigurationRoot Config { get; set; }

        static Configuration()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            Config = builder.Build();

            Configuration.Config = Config;
        }

        public static string DbConnection => Config["DefaultConnection"];
    }
}