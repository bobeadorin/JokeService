using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JokeService.Models.UserModels
{
    public class User
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int Likes { get; set; }
        public List<User>? Followers { get; set; }
        public int FollowersNumber { get; set; }
        public List<Guid>? Posts { get; set; }
        public int PostsNumber { get; set; }
    }
}
