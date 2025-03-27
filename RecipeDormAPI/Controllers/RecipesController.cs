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
        public async Task<IActionResult> SearchForRecipes([FromBody]SearchForRecipeRequest request)
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
        //[Consumes("multipart/form-data")]
        public async Task<IActionResult> AddNewRecipe([FromForm]AddNewRecipeCommand request)
        //public async Task<IActionResult> AddNewRecipe([FromForm] string title, [FromForm] string ingredients, [FromForm] string steps, [FromForm] IFormFile? image)
        {
            try
            {
                // Deserialize the ingredients and steps from the request
                /*var ingredientsList = JsonSerializer.Deserialize<List<IngredientsDto>>(ingredients);
                var stepsList = JsonSerializer.Deserialize<List<StepsDto>>(steps);

                var request = new AddNewRecipeCommand 
                {
                    Title = title, 
                    Ingredients = ingredientsList, 
                    Steps = stepsList, 
                    Image = image 
                };*/

                /*var modelxfmed = new AddNewRecipeCommand { Title = request.Title, Ingredients = request.Ingredients, Steps = request.Steps, Image = request.Image };
                var req = JsonConvert.SerializeObject(modelxfmed);*/

                //_logger.LogInformation($"User attempt to ADD NEW RECIPE\n{req}");
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
