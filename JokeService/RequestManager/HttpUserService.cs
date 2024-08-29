using JokeService.Constants;
using JokeService.Models.UserModels;
using JokeService.RequestManager.Interfaces;
using JokeService.Token;
using System.Text.Json;


namespace JokeService.RequestManager
{
    public class HttpUserService:IHttpUserService
    {
        private readonly TokenManager _tokenManager;
        private readonly HttpClient _httpClient;

        public HttpUserService( HttpClient httpClient)
        {
            _tokenManager = TokenManager.Instance;
            _httpClient = httpClient;
        }

        
        public async Task<User> GetUserById(Guid id)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenManager.AccessToken);

            var response = await _httpClient.GetAsync(BaseEndpoints.userServiceBaseURL + "/getUserById/" + id);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadFromJsonAsync<User>(); 
                if (json is null) throw new Exception("User is null");
                return json;
            }
            throw new Exception("User not found");
        }
    }
}
