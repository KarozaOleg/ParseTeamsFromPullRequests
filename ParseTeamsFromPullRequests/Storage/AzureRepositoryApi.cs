using System.Net.Http.Headers;
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
        private string PersonalAccessTokens {get;}
        private int SearchDepthDays { get; }

        public AzureRepositoryApi(string organization, string project, string repositoryId, string personalAccessTokens, int searchDepthDays)
		{
            Organization = organization ?? throw new ArgumentNullException(nameof(organization));
			Project = project ?? throw new ArgumentNullException(nameof(project));
            RepositoryId = repositoryId ?? throw new ArgumentNullException(nameof(repositoryId));
            PersonalAccessTokens = personalAccessTokens ?? throw new ArgumentNullException(nameof(personalAccessTokens));
            SearchDepthDays = searchDepthDays;
            if (searchDepthDays < 0)
                throw new ArgumentOutOfRangeException(nameof(searchDepthDays), "less than zero");
        }

        public async Task<(bool isError, string errorDesc, List<Model.PullRequest.Value> pullRequests)> GetPullRequestsCompletedAsync()
        {
            try
            {
                var lastDateTime = DateTime.Now.AddDays(-SearchDepthDays);
                var pullRequests = new List<Model.PullRequest.Value>();
                var top = 50;
                var skip = 0;

                var stop = false;
                while (stop == false)
                {
                    var url = $"https://dev.azure.com/{Organization}/{Project}/_apis/git/repositories/{RepositoryId}/pullrequests?searchCriteria.status=completed&$top={top}&$skip={skip}";
                    var responseBody = await ReturnResponseBody(url);
                    var root = JsonSerializer.Deserialize<Model.PullRequest.Root>(responseBody);

                    
                    foreach (var item in root.value)
                    {
                        if (item.creationDate < lastDateTime)
                        {
                            stop = true;
                            break;
                        }
                        pullRequests.Add(item);
                    }
                    skip += top;
                }

                return (false, string.Empty, pullRequests);
            }
            catch(Exception ex)
            {
                return (true, ex?.Message, null);
            }
        }

        public async Task<(bool isError, string errorDesc, List<Model.Commit.Value> commits)> GetCommits(int pullRequestId)
        {
            try
            {
                var url = $"https://dev.azure.com/{Organization}/{Project}/_apis/git/repositories/{RepositoryId}/pullrequests/{pullRequestId}/commits";
                var responseBody = await ReturnResponseBody(url);
                var root = JsonSerializer.Deserialize<Model.Commit.Root>(responseBody);
                var commits = root.value;

                return (false, string.Empty, commits);
            }
            catch (Exception ex)
            {
                return (true, ex?.Message, null);
            }
        }

        public async Task<(bool isError, string errorDesc, List<Model.CommitChanges.Change> commitChanges)> GetCommitChanges(string commitId)
        {
            try
            {
                var url = $"https://dev.azure.com/{Organization}/{Project}/_apis/git/repositories/{RepositoryId}/commits/{commitId}/changes";
                var responseBody = await ReturnResponseBody(url);
                var root = JsonSerializer.Deserialize<Model.CommitChanges.Root>(responseBody);
                var commitChanges = root.changes;

                return (false, string.Empty, commitChanges);
            }
            catch (Exception ex)
            {
                return (true, ex?.Message, null);
            }
        }

        private async Task<string> ReturnResponseBody(string url)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", PersonalAccessTokens))));

                using (var response = httpClient.GetAsync(url).Result)
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}

