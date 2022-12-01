namespace SeanProfile.Api.Model
{
    public class BlogModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

}
