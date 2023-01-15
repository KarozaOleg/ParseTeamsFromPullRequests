using System;
using ParseTeamsFromPullRequests.Model;

namespace ParseTeamsFromPullRequests.Service
{
	public class TeamMembersConverterService
	{
		public (bool isError, string errorDesc, Dictionary<string, string> value) GetMemberByTeam(List<Team> teams)
		{
			if (teams == null)
				throw new ArgumentNullException(nameof(teams));

			try
			{
				var membersByTeam = new Dictionary<string, string>();
				foreach (var team in teams)
				{
					foreach (var member in team.Members)
					{
						if (membersByTeam.ContainsKey(member))
							throw new Exception($"membersByTeam already has member: {member}");

						membersByTeam.Add(member, team.Name);
                    }
				}
				return (false, string.Empty, membersByTeam);
            }
			catch(Exception ex)
			{
				return (true, ex?.Message, null);
			}
		}
	}
}

