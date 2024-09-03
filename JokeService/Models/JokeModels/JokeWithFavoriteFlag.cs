namespace JokeService.Models.JokeModels
{
    public class JokeWithFavoriteFlag
    {
        public bool IsFavorite { get; set;}
        public bool IsLiked { get; set;}
        public Joke Joke { get; set;}
    }
}
