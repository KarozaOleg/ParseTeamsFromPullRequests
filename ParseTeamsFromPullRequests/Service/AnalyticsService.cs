using System;
using System.IO;
using System.Linq;
using ParseTeamsFromPullRequests.Model;

namespace ParseTeamsFromPullRequests.Service
{
	public class AnalyticsService
	{
        public (bool isError, string errorDesc, Dictionary<string, Dictionary<string, int>> value) GetServiceWithAmountByTeam(List<PullRequestInfo> pullRequestsInfo, Dictionary<string, string> memberByTeam, HashSet<string> ignoredServiceNames)
        {
            if (ignoredServiceNames == null)
                throw new ArgumentNullException(nameof(ignoredServiceNames));

            try
            {
                string returnServiceFromPath(string path)
                {
                    var splittedPath = path.Split("/");

                    var service = default(string);
                    if (splittedPath.Length > 2)
                    {
                        if (ignoredServiceNames.Any(splittedPath[2].Contains) == false)
                            service = splittedPath[2];
                    }

                    return service;
                }

                var teamWithAmountByPath = ReturnTeamWithAmountByPath(pullRequestsInfo, memberByTeam, returnServiceFromPath);

                return (false, string.Empty, teamWithAmountByPath);
            }
            catch (Exception ex)
            {
                return (true, ex?.Message, null);
            }
        }

        public (bool isError, string errorDesc, Dictionary<string, Dictionary<string, int>> value) GetRootWithAmountByTeam(List<PullRequestInfo> pullRequestsInfo, Dictionary<string, string> memberByTeam)
        {
            try
            {
                string returnRootFromPath(string path)
                {
                    var splittedPath = path.Split("/");

                    var service = default(string);
                    if (splittedPath.Length > 1)
                        service = splittedPath[1];

                    return service;
                }

                var teamWithAmountByRoot = ReturnTeamWithAmountByPath(pullRequestsInfo, memberByTeam, returnRootFromPath);

                return (false, string.Empty, teamWithAmountByRoot);
            }
            catch (Exception ex)
            {
                return (true, ex?.Message, null);
            }
        }

        private Dictionary<string, Dictionary<string, int>> ReturnTeamWithAmountByPath(List<PullRequestInfo> pullRequestsInfo, Dictionary<string, string> memberByTeam, Func<string, string> pathHandler)
        {
            if (pullRequestsInfo == null)
                throw new ArgumentNullException(nameof(pullRequestsInfo));
            if (memberByTeam == null)
                throw new ArgumentNullException(nameof(memberByTeam));
            if (pathHandler == null)
                throw new ArgumentNullException(nameof(pathHandler));

            var teamWithAmountByPath = new Dictionary<string, Dictionary<string, int>>();
            foreach (var pullRequestInfo in pullRequestsInfo)
            {
                var team = ReturnTeamByMember(pullRequestInfo.Owner, memberByTeam);
                var usedPaths = new HashSet<string>();

                foreach (var path in pullRequestInfo.Paths)
                {
                    if (string.IsNullOrWhiteSpace(path))
                        throw new ArgumentNullException(nameof(path));

                    var pathHandled = pathHandler(path);
                    if (string.IsNullOrWhiteSpace(pathHandled))
                        continue;
                    if (usedPaths.Contains(pathHandled))
                        continue;
                    usedPaths.Add(pathHandled);

                    if (teamWithAmountByPath.ContainsKey(pathHandled) == false)
                        teamWithAmountByPath.Add(pathHandled, new Dictionary<string, int>());

                    if (teamWithAmountByPath[pathHandled].ContainsKey(team) == false)
                        teamWithAmountByPath[pathHandled].Add(team, 0);

                    teamWithAmountByPath[pathHandled][team]++;
                }
            }
            return teamWithAmountByPath;
        }

		private string ReturnTeamByMember(string member, Dictionary<string, string> memberByTeam)
		{
            if (string.IsNullOrWhiteSpace(member))
                throw new ArgumentNullException(nameof(member));

			var team = "unknown";
			if (memberByTeam.ContainsKey(member))
				team = memberByTeam[member];
			return team;
        }
	}
}

