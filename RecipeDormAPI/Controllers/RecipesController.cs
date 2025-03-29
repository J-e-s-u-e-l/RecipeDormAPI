using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Application.CQRS.Commands;
using RecipeDormAPI.Application.CQRS.Queries;
using RecipeDormAPI.Infrastructure.Config;
using RecipeDormAPI.Infrastructure.Data.Models.DTOs;
using RecipeDormAPI.Infrastructure.Infrastructure.Auth;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RecipeDormAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuthorize]
    public class RecipesController : ControllerBase
    {
        private readonly IMediator _mediator;
        ILogger<RecipesController> _logger;
        private readonly AppSettings _appSettings;

        public RecipesController(IMediator mediator, ILogger<RecipesController> logger, IOptions<AppSettings> appSettings)
        {
            _mediator = mediator;
            _logger = logger;
            _appSettings = appSettings.Value;
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchForRecipes([FromQuery]SearchForRecipeRequest request)
        {
            try
            {
                var modelxfmed = new SearchForRecipeRequest { SearchQuery = request.SearchQuery };
                var req = JsonConvert.SerializeObject(modelxfmed);

                _logger.LogInformation($"User attempt to SEARCH for a recipe \n{req}");
                var response = await _mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }

        [HttpPost("add-new-recipe")]
        public async Task<IActionResult> AddNewRecipe([FromForm]AddNewRecipeCommand request)
        {
            try
            {
                var modelxfmed = new AddNewRecipeCommand { Title = request.Title, IngredientsJson = request.IngredientsJson, StepsJson = request.StepsJson, Image = request.Image };
                var req = JsonConvert.SerializeObject(modelxfmed);

                _logger.LogInformation($"User attempt to ADD NEW RECIPE\n{req}");
                var response = await _mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }

        [HttpGet("get-my-recipes")]
        public async Task<IActionResult> GetMyRecipes([FromQuery]GetMyRecipesRequest request)
        {
            try
            {
                var modelxfmed = new GetMyRecipesRequest { page = request.page };
                var req = JsonConvert.SerializeObject(modelxfmed);

                _logger.LogInformation($"User attempt to GET ALL personally created RECIPES");
                var response = await _mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }

        [HttpGet("get-recipe-by-id")]
        public async Task<IActionResult> GetRecipeById([FromQuery]GetRecipeByIdRequest request)
        {
            try
            {
                var modelxfmed = new GetRecipeByIdRequest { RecipeId = request.RecipeId };
                var req = JsonConvert.SerializeObject(modelxfmed);

                _logger.LogInformation($"User attempt to GET RECIPE by ID\n{req}");
                var response = await _mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }

        [HttpPost("bookmark-recipe")]
        public async Task<IActionResult> SaveRecipeToBookmark(BookmarkRecipeCommand request)
        {
            try
            {
                var modelxfmed = new BookmarkRecipeCommand { RecipeId = request.RecipeId };
                var req = JsonConvert.SerializeObject(modelxfmed);

                _logger.LogInformation($"User attempt to SAVE RECIPE to BOOKMARK\n{req}");
                var response = await _mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }

        [HttpDelete("remove-bookmark")]
        public async Task<IActionResult> UnbookmarkRecipe([FromQuery]UnbookmarkRecipeCommand request)
        {
            try
            {
                var modelxfmed = new UnbookmarkRecipeCommand { RecipeId = request.RecipeId };
                var req = JsonConvert.SerializeObject(modelxfmed);

                _logger.LogInformation($"User attempt to UNBOOKMARK RECIPE\n{req}");
                var response = await _mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }

        [HttpGet("get-my-bookmarked-recipes")]
        public async Task<IActionResult> GetMyBookmarkedRecipes([FromQuery]GetMyBookmarkedRecipesRequest request)
        {
            try
            {
                var modelxfmed = new GetMyBookmarkedRecipesRequest { PageNumber = request.PageNumber };
                var req = JsonConvert.SerializeObject(modelxfmed);

                _logger.LogInformation($"User attempted to fetch their bookmarked recipes\n{req}");
                var response = await _mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }

        [HttpPost("like")]
        public async Task<IActionResult> LikeRecipe(LikeRecipeCommand request)
        {
            try
            {
                var modelxfmed = new LikeRecipeCommand { RecipeId = request.RecipeId };
                var req = JsonConvert.SerializeObject(modelxfmed);

                _logger.LogInformation($"User attempted to like recipe\n{req}");
                var response = await _mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }

        [HttpDelete("remove-like")]
        public async Task<IActionResult> RemoveRecipeLike(RemoveRecipeLikeCommand request)
        {
            try
            {
                var modelxfmed = new LikeRecipeCommand { RecipeId = request.RecipeId };
                var req = JsonConvert.SerializeObject(modelxfmed);

                _logger.LogInformation($"User attempted to like recipe\n{req}");
                var response = await _mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong\n {ex.StackTrace}: {ex.Message}");
                return StatusCode(500, $"{_appSettings.ProcessingError}");
            }
        }
    }
}
