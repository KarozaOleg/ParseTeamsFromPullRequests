using System;
using ParseTeamsFromPullRequests.Model;

namespace ParseTeamsFromPullRequests.Interfaces
{
	public interface IRepositoryApi
	{
		public Task<(bool isError, string errorDescription, List<PullRequest> pullRequests)> GetPullRequestsAsync();
		public string GetCommits(ulong pullRequestId);
		public string GetChanges(ulong commitId);
	}
}

