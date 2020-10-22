using SW.FluentOlap.AnalyticalNode;

namespace UtilityUnitTests.Models
{
    public class PostMessage
    {
        public string Id { get; set; }
    }
    public class Post
    {
        public string userId { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    public class PostAnalyzer : AnalyticalObject<Post>
    {
        public PostAnalyzer()
        {
            Handles(nameof(PostMessage), nameof(PostMessage.Id));
        }
    }
}