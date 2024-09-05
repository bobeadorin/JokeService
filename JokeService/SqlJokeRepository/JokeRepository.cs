using JokeService.DbConnection;
using JokeService.Models.JokeModels;
using JokeService.Models.UserFavoriteJokes;
using JokeService.Models.UserModels;
using JokeService.RequestManager;
using JokeService.RequestManager.Interfaces;
using JokeService.SqlJokeRepository.Interfaces;
using JokeService.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace JokeService.SqlJokeRepository
{
    public class JokeRepository : IJokeRepository
    {
        private readonly AppDbContext _context;
        private readonly IHttpUserService _userService;

        public JokeRepository(AppDbContext context, IHttpUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public List<Joke> GetFavoriteJokeList(Guid userId) {
                var jokes = _context.UserFavorites.Where(u => u.UserId == userId).Select( j => j.JokeId).ToList();
                var jokesById = _context.Jokes.Where(j => jokes.Contains(j.Id)).ToList();
            
                return jokesById;
        }

        public List<Joke> GetPostedJokes(Guid userId)
        {
            var jokesById = _context.Jokes.Where(j => j.AuthorId == userId).ToList();

            return jokesById;
        }

        public List<Joke> GetAllJokes()
        {
            var jokes = _context.Jokes.ToList();

            return jokes;
        }

        public Joke? GetJokeById(int id)
        {
            var joke = _context.Jokes.FirstOrDefault(x => x.Id == id);

            if (joke == null) throw new Exception("Joke not found");


            return joke;
        }

        public async Task<bool> AddJoke(JokePost joke , Guid authorId)
        {
            try
            {
                var user = await  _userService.GetUserById(authorId);

                var jokePost = new Joke
                {
                    Text = joke.Text,
                    Category = joke.Category,
                    AuthorId = authorId,
                    AuthorUsername = user.Username,
                    LikedBy = new List<Guid?> { }
                };

                _context.Jokes.Add(jokePost);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            };
        }

        public async Task<bool> UpdateJoke(Joke joke)
        {
            try
            {
                _context.Jokes.Update(joke);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<bool> DeleteJoke(int id)
        {
            try
            {
                var jokeThatWillBeDeleted = _context.Jokes.FirstOrDefault(x => x.Id == id);

                if (jokeThatWillBeDeleted == null) throw new Exception("Joke not found");

                _context.Jokes.Remove(jokeThatWillBeDeleted);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Joke> GetJokesByCategory(string category)
        {
            var jokes = _context.Jokes.Where(x => x.Category == category).ToList();

            return jokes;
        }

        public void AddOrPopJokeToFavorite(int jokeId, Guid userId)
        {
            var favoriteJoke = _context.UserFavorites.FirstOrDefault(x => x.UserId == userId && x.JokeId == jokeId);

            if (favoriteJoke == null)
            {
                _context.UserFavorites.Add(new UserFavorites { JokeId = jokeId, UserId = userId });
                _context.SaveChanges();
            }
            else
            {
                _context.UserFavorites.Remove(favoriteJoke);
                _context.SaveChanges();
            }
        }

        public List<JokeWithFavoriteFlag> GetJokesByPagination(int pageNumber, int pageSize, string category, string? accessToken)
        {
            var userId = JwtUtility.RetriveDataFromTokenWithPossibleNull(accessToken);

            var favoriteJokes = new HashSet<int>();

            if (userId.HasValue)
            {
                favoriteJokes = _context.UserFavorites
                    .Where(f => f.UserId == userId.Value)
                    .Select(f => f.JokeId)
                    .ToHashSet();
            }

            List<JokeWithFavoriteFlag> jokesByPage;

            var query = _context.Jokes.AsQueryable();

            if (category != "home")
            {
                query = query.Where(j => j.Category == category);
            }

            jokesByPage = query
                .OrderBy(j => j.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(j => new JokeWithFavoriteFlag
                {
                    Joke = new Joke
                    {
                        Id = j.Id,
                        Text = j.Text,
                        Category = j.Category,
                        AuthorUsername = j.AuthorUsername,
                        Likes = j.Likes,
                        LikedBy = j.LikedBy,
                    },
                    IsFavorite = userId.HasValue && favoriteJokes.Contains(j.Id),
                    IsLiked = userId.HasValue && j.LikedBy.Contains(userId)
                })
                .ToList();

            return jokesByPage;
        }


        public List<Joke> GetAllJokesByAuthorId(Guid AuthorId)
        {
            var jokes = _context.Jokes.Select(x => x).Where(x => x.AuthorId == AuthorId).ToList();

            return jokes;
        }


        public bool LikeJokeById(int jokeId, Guid userId)
        {
            var joke = _context.Jokes.FirstOrDefault(x => x.Id == jokeId);

            if (joke == null) return false;

            if (joke.LikedBy.Contains(userId))
            {
                joke.LikedBy?.Remove(userId);
                joke.Likes--;
                _context.Jokes.Update(joke);
                _context.SaveChanges();
            }
            else
            {
                joke.LikedBy?.Add(userId);
                joke.Likes++;
                _context.Jokes.Update(joke);
                _context.SaveChanges();
            }
          
            return true;

        }

        public bool DislikeJokeById(int jokeId, Guid userId)
        {
            var joke = _context.Jokes.FirstOrDefault(x => x.Id == jokeId);

            if (joke == null) return false;

            joke.LikedBy.Remove(userId);
            joke.Likes--;
            _context.Jokes.Update(joke);
            _context.SaveChanges();
            return true;
        }



    }
}
