namespace ParseTeamsFromPullRequests.Model.CommitChanges
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Change
    {
        public Item item { get; set; }
        public string changeType { get; set; }
    }

    public class ChangeCounts
    {
        public int Edit { get; set; }
    }

    public class Item
    {
        public string objectId { get; set; }
        public string originalObjectId { get; set; }
        public string gitObjectType { get; set; }
        public string commitId { get; set; }
        public string path { get; set; }
        public bool isFolder { get; set; }
        public string url { get; set; }
    }

    public class Root
    {
        public ChangeCounts changeCounts { get; set; }
        public List<Change> changes { get; set; }
    }


}

