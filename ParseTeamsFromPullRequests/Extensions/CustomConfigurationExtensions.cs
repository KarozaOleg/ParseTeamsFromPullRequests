using Microsoft.Extensions.Configuration;

namespace ParseTeamsFromPullRequests.Extensions
{
	public static class CustomConfigurationExtensions
	{
        public static IConfiguration ReturnAppConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("ENVIRONMENT");
            var baseDirectoryName = Directory.GetParent(AppContext.BaseDirectory).FullName;

            return new ConfigurationBuilder()
                .SetBasePath(baseDirectoryName)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json")
                .AddEnvironmentVariables()
                .Build();
        }

        public static T ReturnConfiguration<T>(IConfiguration configuration, string name) where T : class
        {
            return configuration
                .GetSection(name)
                .Get<T>();
        }
    }
}

