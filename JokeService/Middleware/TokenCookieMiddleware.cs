using Microsoft.AspNetCore.Authorization;

namespace JokeService.Middleware
{
    public class TokenCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

       public async Task Invoke(HttpContext context)
        {
             var endpoint = context.GetEndpoint();

            if (endpoint != null)
            {
                var authorizeAttribute = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();

                if (authorizeAttribute != null)
                {
                    var token = context.Request.Cookies["AccessToken"];

                    if (!string.IsNullOrEmpty(token))
                    {
                        if (!context.Request.Headers.ContainsKey("Authorization"))
                        {
                            context.Request.Headers.Add("Authorization", $"Bearer {token}");
                        }
                        else if (string.IsNullOrEmpty(context.Request.Headers["Authorization"]))
                        {
                            context.Request.Headers["Authorization"] = $"Bearer {token}";
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}
