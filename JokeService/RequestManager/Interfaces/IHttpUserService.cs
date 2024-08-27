using JokeService.Models.UserModels;

namespace JokeService.RequestManager.Interfaces
{
    public interface IHttpUserService
    {
        public Task<User> GetUserById(Guid id);
    }
}
