using JokeService.Models.JokeModels;

namespace JokeService.SqlJokeRepository.Interfaces
{
    public interface IJokeRepository
    {
        public List<Joke> GetAllJokes();
        public Joke? GetJokeById(int id);
        public Task<bool> AddJoke(Joke joke);
        public Task<bool> UpdateJoke(Joke joke);
        public Task<bool> DeleteJoke(int id);
        public List<Joke> GetJokesByPagination(int lastId, int pageSize);
        public bool LikeJokeById(int jokeId, Guid userId);
        public bool DislikeJokeById(int jokeId, Guid userId);
    }
}
