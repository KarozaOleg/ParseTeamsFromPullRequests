﻿using System.Text;
using NLog;
using ParseTeamsFromPullRequests.Extensions;
using ParseTeamsFromPullRequests.Model;
using ParseTeamsFromPullRequests.Model.Configuration;
using ParseTeamsFromPullRequests.Service;
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
            Logger = LogManager.GetCurrentClassLogger() ?? throw new ArgumentNullException("logger");

            var appConfiguration = CustomConfigurationExtensions.ReturnAppConfiguration() ?? throw new ArgumentNullException("appConfiguration");

            RepositoryConfiguration = CustomConfigurationExtensions.ReturnConfiguration<RepositoryConfiguration>(appConfiguration, RepositoryConfiguration.Name) ?? throw new ArgumentNullException(RepositoryConfiguration.Name);
            PullRequestsConfiguraiton = CustomConfigurationExtensions.ReturnConfiguration<PullRequestsConfiguraiton>(appConfiguration, PullRequestsConfiguraiton.Name) ?? throw new ArgumentNullException(PullRequestsConfiguraiton.Name);
            TeamsConfiguration = CustomConfigurationExtensions.ReturnConfiguration<TeamsConfiguration>(appConfiguration, TeamsConfiguration.Name) ?? throw new ArgumentNullException(TeamsConfiguration.Name);
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
            var azureApi = new AzureRepositoryApi(RepositoryConfiguration.Organization, RepositoryConfiguration.Project, RepositoryConfiguration.RepositoryId, RepositoryConfiguration.PersonalAccessTokens, PullRequestsConfiguraiton.SearchDepthDays);
            var pullRequestReaderService = new PullRequestReaderService(azureApi);

            var pullRequestsInfo = await pullRequestReaderService.GetPullRequestsInfo(PullRequestsConfiguraiton.IgnoreInDescription);
            if (pullRequestsInfo.isError)
                throw new Exception($"receiving PullRequestsInfo error, details:{pullRequestsInfo.errorDesc}");

            var teamMembersConverterService = new TeamMembersConverterService();
            var memberByTeam = teamMembersConverterService.GetMemberByTeam(TeamsConfiguration.Teams);
            if (memberByTeam.isError)
                throw new Exception($"team converting error, details:{memberByTeam.errorDesc}");

            var analyticsService = new AnalyticsService();
            var rootByTeam = analyticsService.GetRootWithAmountByTeam(pullRequestsInfo.value, memberByTeam.value);
            if (rootByTeam.isError)
                throw new Exception($"rootByTeam analytics failed, details:{rootByTeam.errorDesc}");

            var serviceByTeam = analyticsService.GetServiceWithAmountByTeam(pullRequestsInfo.value, memberByTeam.value, PullRequestsConfiguraiton.IgnoredServiceName);
            if (serviceByTeam.isError)
                throw new Exception($"serviceByTeam analytics failed, details:{serviceByTeam.errorDesc}");

            PrintAnalytics(nameof(rootByTeam), rootByTeam.value);
            PrintAnalytics(nameof(serviceByTeam), serviceByTeam.value);

            Console.ReadKey();
        }
        catch(Exception ex)
        {
            Logger.Error($"main problem, details:{ex?.Message}");
            throw;
        }
    }

    static void PrintAnalytics(string name, Dictionary<string, Dictionary<string, int>> analytics)
    {
        Console.WriteLine($"Going to print analytic for {name}");
        foreach (var item in analytics)
        {
            var teamsWithAmount = new StringBuilder(1000);
            foreach (var itemFromArray in item.Value)
                teamsWithAmount.Append($"{itemFromArray.Key}[{itemFromArray.Value}]");
            Console.WriteLine($"{item.Key} - {teamsWithAmount.ToString()}");
        }
    }
}

