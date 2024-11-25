namespace HaveBreak.API.Controllers.Posts.Dtos
{
    public class PostDto
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public List<string> Comments { get; set; }

        public int Likes { get; set; }
    }
}
