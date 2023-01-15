namespace ParseTeamsFromPullRequests.Model.Configuration
{
	public class PullRequestsConfiguraiton 
    {
		public static string Name => "PullRequestsConfiguraiton";
		public int SearchDepthDays { get; set; }
        public HashSet<string> IgnoreInDescription { get; set; }
        public HashSet<string> IgnoredServiceName { get; set; }
    }
}

