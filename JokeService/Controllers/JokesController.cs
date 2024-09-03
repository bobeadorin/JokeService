
using JokeService.SqlJokeRepository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using JokeService.Models.JokeModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;
using JokeService.RequestManager.Interfaces;
using JokeService.Utility;

namespace JokeService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JokesController : ControllerBase
    {
        public readonly IJokeRepository _jokeRepository;
        public readonly IHttpUserService _httpUserService;

        public JokesController( IJokeRepository jokeRepository , IHttpUserService httpUserService)
        {
            _jokeRepository = jokeRepository;
            _httpUserService = httpUserService;
        }

 
        [HttpGet("/getAllJokes")]
        public IActionResult GetAllJokes()
        {
            var userClaims = User.Claims;
            var userId = userClaims.FirstOrDefault(c => c.Type == "id")?.Value;
            Console.WriteLine(userId);                       

            var jokes = _jokeRepository.GetAllJokes();

            if(jokes.Count == 0) return NotFound();

            return Ok(jokes);
        }

        [Authorize]
        [HttpGet("/getUser/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var userData = await _httpUserService.GetUserById(id);
                return Ok(userData);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("/likeJokeById")]
        public IActionResult LikeJokeById([FromBody]int id)
        {
            var userClaims = User.Claims;
            var userId = userClaims.FirstOrDefault(c => c.Type == "id")?.Value;
            
            if(userId == null) return Unauthorized();

            var joke = _jokeRepository.LikeJokeById(id, new Guid(userId));

            return Ok(id);
        }

        [Authorize]
        [HttpPut("/dislikeJokeById/{id}")]
        public IActionResult DislikeJokeById(int id)
        {
            var userClaims = User.Claims;
            var userId = userClaims.FirstOrDefault(c => c.Type == "id")?.Value;

            if (userId == null) return Unauthorized();

            var joke = _jokeRepository.LikeJokeById(id, new Guid(userId));

            return Ok(id);
        }

        [HttpGet("/getAllJokesByPagination/{pageNumber}/{pageSize}/category/{category}")]
        public IActionResult GetJokesByPagination(int pageNumber, int pageSize,string category)
        {   
            var accessToken = Request.Cookies["AccessToken"];

            var jokes = _jokeRepository.GetJokesByPagination(pageNumber, pageSize, category, accessToken);

            if (jokes.Count == 0) return NotFound();

            return Ok(jokes);
        }

        [Authorize]
        [HttpPost("/postJoke")]
        public async Task<IActionResult> PostJoke([FromBody] JokePost joke)
        {
            try
            {
                var userClaims = User.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == "id")?.Value;

                if (joke == null || string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid joke data or user ID");
                }

                var isJokePosted = await _jokeRepository.AddJoke(joke, new Guid(userId));

                if (!isJokePosted) return BadRequest("Failed to post joke");

                return Ok(isJokePosted);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("/deleteJokeById/{Id}")]
        public async Task<IActionResult> DeleteJokeById(int Id)
        {
            try
            {
                var isJokeDeleted = await _jokeRepository.DeleteJoke(Id);

                if(!isJokeDeleted) return NotFound();

                return Ok(isJokeDeleted);

            }
            catch(Exception ex) when (ex.Message == "Joke Not Found")
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }


        [Authorize]
        [HttpPut("/addToFavorite")]
        public IActionResult AddToFavorite([FromBody] int jokeId)
        {
            var accessToken = Request.Cookies["AccessToken"];

            if (accessToken != null)
            {
                var userId = JwtUtility.RetriveDataFromToken(accessToken);
                _jokeRepository.AddOrPopJokeToFavorite(jokeId , userId);

                return Ok();
             
            }
            return BadRequest();
        }
    }
}
