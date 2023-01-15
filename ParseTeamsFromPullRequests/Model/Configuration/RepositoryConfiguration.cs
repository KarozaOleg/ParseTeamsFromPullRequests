namespace ParseTeamsFromPullRequests.Model.Configuration
{
	public class RepositoryConfiguration
    {
        public static string Name => "RepositoryConfiguration";
        public string Organization { get; set; }
        public string Project { get; set; }
        public string RepositoryId { get; set; }
        public string PersonalAccessTokens { get; set; }
    }
}

