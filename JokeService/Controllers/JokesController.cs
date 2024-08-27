
using JokeService.SqlJokeRepository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using JokeService.Models.JokeModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;
using JokeService.RequestManager.Interfaces;

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

        [Authorize]
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
        [HttpPut("/likeJokeById/{id}")]
        public IActionResult LikeJokeById(int id)
        {
            var userClaims = User.Claims;
            var userId = userClaims.FirstOrDefault(c => c.Type == "id")?.Value;
            
            if(userId == null) return Unauthorized();

            var joke = _jokeRepository.LikeJokeById(id, new Guid(userId));

            return Ok(id);
        }

        
        [HttpGet("/getUser/{id}")]
        public async Task <IActionResult> GetUserById(Guid id)
        {
            try
            {
                var userData = await _httpUserService.GetUserById(id);
                return Ok(userData);

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

        [HttpGet("/getAllJokesByPagination{pageNumber},{pageSize}")]
        public IActionResult GetJokesByPagination(int pageNumber, int pageSize)
        { 
            var jokes = _jokeRepository.GetJokesByPagination(pageNumber, pageSize);

            if(jokes.Count == 0) return NotFound();

            return Ok(jokes);
        }

        [HttpPost("/postJoke")]
        public async Task<IActionResult> PostJoke([FromBody] Joke joke)
        {
            try
            {
                var isJokePosted = await _jokeRepository.AddJoke(joke);

                if(!isJokePosted) return BadRequest();

                return Ok(isJokePosted);
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }   
        }

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
    }
}
