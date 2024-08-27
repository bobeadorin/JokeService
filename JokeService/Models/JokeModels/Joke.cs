using System.Text.Json.Serialization;

namespace JokeService.Models.JokeModels
{
    public class Joke
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorUsername { get; set; }
        public int Likes { get; set; }
        public List<Guid> LikedBy { get; set; }
    }
}
//{
//    "text": ".NET developers are picky when it comes to food. They only like chicken NuGet. ",
//  "category": "Programming",
//  "authorId": "d3e37998-8cbf-4162-ba83-b7f28758b033",
//  "authorUsername": "JoeDoeTheFirst",
//  "likes": 0,
//  "likedBy": []
//}