namespace ParseTeamsFromPullRequests.Model.Configuration
{
	public class PullRequestsConfiguraiton 
    {
		public static string Name => "PullRequestsConfiguraiton";
        public HashSet<string> StopWords { get; set; }
	}
}

