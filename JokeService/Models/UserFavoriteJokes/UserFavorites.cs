namespace JokeService.Models.UserFavoriteJokes
{
    public class UserFavorites
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int JokeId { get; set; }
    }
}
