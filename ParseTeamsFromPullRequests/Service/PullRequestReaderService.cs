using System;
using System.Linq;
using ParseTeamsFromPullRequests.Interfaces;
using ParseTeamsFromPullRequests.Model;
using ParseTeamsFromPullRequests.Model.Configuration;

namespace ParseTeamsFromPullRequests.Service
{
	public class PullRequestReaderService
	{
		private IRepositoryApi RepositoryApi { get; }

        public PullRequestReaderService(IRepositoryApi repositoryApi)
		{
            RepositoryApi = repositoryApi ?? throw new ArgumentNullException(nameof(repositoryApi));
        }

		public async Task<(bool isError, string errorDesc, List<PullRequestInfo> value)> GetPullRequestsInfo(HashSet<string> ignoreInDescription)
		{
            if (ignoreInDescription == null)
                throw new ArgumentNullException(nameof(ignoreInDescription));

            try
            {
                var pullRequestsInfo = new List<PullRequestInfo>();

                var responsePullRequests = await RepositoryApi.GetPullRequestsCompletedAsync();
                if (responsePullRequests.isError)
                    throw new Exception($"receiving pull requests error, details:{responsePullRequests.errorDesc}");
                if (responsePullRequests.pullRequests == null)
                    throw new ArgumentNullException(nameof(responsePullRequests.pullRequests));

                foreach (var pullRequest in responsePullRequests.pullRequests)
                {
                    if (pullRequest == null)
                        throw new ArgumentNullException(nameof(pullRequest));
                    if (pullRequest.createdBy == null)
                        throw new ArgumentNullException(nameof(pullRequest.createdBy));

                    if (string.IsNullOrWhiteSpace(pullRequest.createdBy.displayName))
                        continue;
                    if (string.IsNullOrEmpty(pullRequest.description) == false)
                        if (ignoreInDescription.Any(pullRequest.description.Contains))
                            continue;
                    if (string.IsNullOrEmpty(pullRequest.title) == false)
                        if (ignoreInDescription.Any(pullRequest.title.Contains))
                            continue;

                    var pullRequestInfo = new PullRequestInfo(pullRequest.createdBy.displayName);
                    pullRequestsInfo.Add(pullRequestInfo);

                    var responseCommits = await RepositoryApi.GetCommits(pullRequest.pullRequestId);
                    if (responseCommits.isError)
                        throw new Exception($"receiving commits failed, details:{responseCommits.errorDesc}");
                    if (responseCommits.commits == null)
                        throw new ArgumentNullException(nameof(responseCommits.commits));

                    foreach (var commit in responseCommits.commits)
                    {
                        if (commit == null)
                            throw new ArgumentNullException(nameof(commit));
                        if (string.IsNullOrWhiteSpace(commit.commitId))
                            throw new ArgumentNullException(nameof(commit.commitId));

                        var responseCommitChanges = await RepositoryApi.GetCommitChanges(commit.commitId);
                        if (responseCommitChanges.isError)
                            throw new Exception($"receiving commit changes failed, details:{responseCommitChanges.errorDesc}");
                        if (responseCommitChanges.commitChanges == null)
                            throw new ArgumentNullException(nameof(responseCommitChanges.commitChanges));

                        foreach (var commitChanges in responseCommitChanges.commitChanges)
                        {
                            if (commitChanges == null)
                                throw new ArgumentNullException(nameof(commitChanges));
                            if (commitChanges.item == null)
                                throw new ArgumentNullException(nameof(commitChanges.item));
                            if (string.IsNullOrWhiteSpace(commitChanges.item.path))
                                continue;

                            pullRequestInfo.Paths.Add(commitChanges.item.path);
                        }
                    }
                }
                return (false, string.Empty, pullRequestsInfo);
            }
            catch(Exception ex)
            {
                return (true, ex?.Message, null);
            }
        }
	}
}

