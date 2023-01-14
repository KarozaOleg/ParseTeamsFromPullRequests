using System.Text.Json;
using ParseTeamsFromPullRequests.Interfaces;
using ParseTeamsFromPullRequests.Model;

namespace ParseTeamsFromPullRequests.Storage
{
	public class AzureRepositoryApi : IRepositoryApi
    {
		private string Organization { get; }
		private string Project { get; }
		private string RepositoryId { get; }

        public AzureRepositoryApi(string organization, string project, string repositoryId)
		{
			Organization = organization;
			Project = project;
			RepositoryId = repositoryId;
        }

        public async Task<(bool isError, string errorDescription, List<PullRequest> pullRequests)> GetPullRequestsAsync()
        {
            try
            {
                var url = $"https://dev.azure.com/{Organization}/{Project}/_apis/git/repositories/{RepositoryId}/pullrequests";
                using (var httpClient = new HttpClient())
                {
                    var jsonString = await httpClient.GetStringAsync(url);
                    var pullRequests = JsonSerializer.Deserialize<List<PullRequest>>(jsonString);

                    return (false, string.Empty, pullRequests);
                }
            }
            catch(Exception ex)
            {
                return (true, ex?.Message, null);
            }
        }

        public string GetCommits(ulong pullRequestId)
        {
            throw new NotImplementedException();
        }

        public string GetChanges(ulong commitId)
        {
            throw new NotImplementedException();
        }
    }
}

