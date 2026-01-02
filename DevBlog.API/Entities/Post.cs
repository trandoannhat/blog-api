namespace DevBlog.API.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string Thumbnail { get; set; } = null!;

        public DateTime PublishedAt { get; set; }
    }
}
