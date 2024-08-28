using JokeService.Constants;
using JokeService.Utility;
using System.Text.Json;
using System.Text;
using JokeService.Models.LoginRequest;
using JokeService.Models.TokenRes;
using JokeService.Models.TokenReq;
using System.Net;


namespace JokeService.Token
{
    public class TokenManager
    {
        private static readonly Lazy<TokenManager> _instance = new Lazy<TokenManager>(() => new TokenManager());
        private string _accessToken;
        private string _refreshToken;
        private IConfiguration _configuration;
        private bool _tokensInitialized;
        private HttpClient _httpClient;
        private DateTime _tokenExpiration;

        private  TokenManager()
        {
            _httpClient = new HttpClient();
            _tokensInitialized = false;
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

        public static TokenManager Instance => _instance.Value;

       public string AccessToken 
        { 
            get 
            {
                EnsureTokensInitialize().Wait();

                if(IsNearExpiry())
                {
                    _ = RefreshToken();
                }
                return _accessToken;
            }
            
            private set => _accessToken = value;
       }

        
        private async Task EnsureTokensInitialize()
        {
            if (!_tokensInitialized)
            {
                await InitializeTokens();
                _tokensInitialized = true;
            }
        }

        private async Task InitializeTokens()
        {
            var tokenResponse = await Login();
            if (tokenResponse != null)
            {
                SetToken(tokenResponse.AccessToken, tokenResponse.RefreshToken.Token, JwtUtility.ExtractExpirationDate(tokenResponse.AccessToken));
            }
        }
        
        private bool IsNearExpiry()
        {
            return DateTime.UtcNow.AddMinutes(5) > _tokenExpiration.AddMinutes(-5);
        }

        private async Task RefreshToken()
        {
            string url = "https://localhost:7005/serviceRefresh";

            var refreshToken = new TokenRefreshRequest
            {
                RefreshToken = _refreshToken
            };

            var content = new StringContent(JsonSerializer.Serialize(_refreshToken), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            //response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();


                if (jsonResponse != null)
                {
                    SetToken(jsonResponse.AccessToken, jsonResponse.RefreshToken.Token, JwtUtility.ExtractExpirationDate(jsonResponse.AccessToken));
                }
            }
            else
            {
                await InitializeTokens();
            }
        }


        private void SetToken(string accessToken, string refreshToken, DateTime tokenExpiration)
        {
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            _tokenExpiration = tokenExpiration;
        }


        public async Task<TokenResponse?> Login()
        {
           using StringContent jsonContent = new(
                    JsonSerializer.Serialize<LoginRequest>(
                        new()
                        {
                            Username = "devService",
                            Password = "DevPassword123@"
                        }),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("https://localhost:7005/serviceLogin", jsonContent);
                response.EnsureSuccessStatusCode();
                var responseContent = response.Content.ReadFromJsonAsync<TokenResponse>().Result;
                
                
          
             Console.WriteLine(responseContent);
            return new TokenResponse
            {
                AccessToken = responseContent.AccessToken,
                RefreshToken = new RefreshToken { Token = responseContent.RefreshToken.Token, Expiration = responseContent.RefreshToken.Expiration }
            };
        }
    }
}

