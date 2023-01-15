namespace ParseTeamsFromPullRequests.Model
{
	public class PullRequestInfo
	{
		public string Owner { get; }
		public HashSet<string> Paths { get; }

		public PullRequestInfo(string owner)
		{
			Owner = owner;
			Paths = new HashSet<string>();
        }
	}
}

