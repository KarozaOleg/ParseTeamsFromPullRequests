using NLog;
using ParseTeamsFromPullRequests.Extensions;
using ParseTeamsFromPullRequests.Model.Configuration;
using ParseTeamsFromPullRequests.Storage;

static class Program
{
    static ILogger Logger { get; }
    static RepositoryConfiguration RepositoryConfiguration { get; }
    static PullRequestsConfiguraiton PullRequestsConfiguraiton { get; }
    static TeamsConfiguration TeamsConfiguration { get; }

    static Program()
    {
        try
        {
            Logger = LogManager.GetCurrentClassLogger();

            var configuration = CustomConfigurationExtensions.ReturnAppConfiguration();

            RepositoryConfiguration = CustomConfigurationExtensions.ReturnConfiguration<RepositoryConfiguration>(configuration, RepositoryConfiguration.Name);
            PullRequestsConfiguraiton = CustomConfigurationExtensions.ReturnConfiguration<PullRequestsConfiguraiton>(configuration, PullRequestsConfiguraiton.Name);
            TeamsConfiguration = CustomConfigurationExtensions.ReturnConfiguration<TeamsConfiguration>(configuration, TeamsConfiguration.Name);
        }
        catch (Exception ex)
        {
            Logger?.Error($".ctor problem, details:{ex?.Message}");
            throw;
        }
    }

    static async Task Main()
    {
        try
        {
            var azureApi = new AzureRepositoryApi(RepositoryConfiguration.Organization, RepositoryConfiguration.Project, RepositoryConfiguration.RepositoryId);
            var response = await azureApi.GetPullRequestsAsync();
            if (response.isError)
                throw new Exception($"did't receive pull requests, error description:{response.errorDescription}");

            var pullRequests = response.pullRequests;
        }
        catch(Exception ex)
        {
            Logger.Error($"main problem, details:{ex?.Message}");
            throw;
        }
    }
}

