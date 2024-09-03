using JokeService.Models.JokeModels;

namespace JokeService.SqlJokeRepository.Interfaces
{
    public interface IJokeRepository
    {
        public List<Joke> GetAllJokes();
        public Joke? GetJokeById(int id);
        public Task<bool> AddJoke(JokePost joke, Guid authorId);
        public Task<bool> UpdateJoke(Joke joke);
        public Task<bool> DeleteJoke(int id);
        public List<JokeWithFavoriteFlag> GetJokesByPagination(int lastId, int pageSize, string category, string? userId);
        public bool LikeJokeById(int jokeId, Guid userId);
        public bool DislikeJokeById(int jokeId, Guid userId);
        public void AddOrPopJokeToFavorite(int jokeId, Guid userId);
    }
}
