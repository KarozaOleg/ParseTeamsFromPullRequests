namespace ParseTeamsFromPullRequests.Model.Configuration
{
    public class TeamsConfiguration
    {
        public static string Name => "TeamsConfiguration";
        public List<Team> Teams { get; set; }
    }
}

