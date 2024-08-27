using JokeService.DbConnection;
using JokeService.SqlJokeRepository;
using JokeService.SqlJokeRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using JokeService.Middleware;
using JokeService.RequestManager.Interfaces;
using JokeService.RequestManager;
using JokeService.Token;


namespace JokeService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            var secret = builder.Configuration.GetSection("JwtToken").GetSection("SecretKey").Value;
            var key = Encoding.ASCII.GetBytes(secret);
            Console.WriteLine(key);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                Console.WriteLine(options.TokenValidationParameters.IssuerSigningKey);
                Console.WriteLine(options.TokenValidationParameters.ValidateIssuerSigningKey);
                Console.WriteLine(options.TokenValidationParameters.ValidateIssuerSigningKey);
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IJokeRepository, JokeRepository>();
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IHttpUserService, HttpUserService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<TokenCookieMiddleware>();


            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
