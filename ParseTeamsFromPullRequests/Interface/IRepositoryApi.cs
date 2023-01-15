namespace ParseTeamsFromPullRequests.Interfaces
{
	public interface IRepositoryApi
	{
		public Task<(bool isError, string errorDesc, List<Model.PullRequest.Value> pullRequests)> GetPullRequestsCompletedAsync();
		public Task<(bool isError, string errorDesc, List<Model.Commit.Value> commits)> GetCommits(int pullRequestId);
		public Task<(bool isError, string errorDesc, List<Model.CommitChanges.Change> commitChanges)> GetCommitChanges(string commitId);
	}
}

