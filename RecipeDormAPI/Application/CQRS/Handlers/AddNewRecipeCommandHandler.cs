using MediatR;
using Microsoft.Extensions.Options;
using RecipeAPI.Infrastructure.Data.Entities;
using RecipeDormAPI.Application.CQRS.Commands;
using RecipeDormAPI.Infrastructure.Config;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using RecipeDormAPI.Infrastructure.Infrastructure.Persistence;
using RecipeDormAPI.Infrastructure.Infrastructure.Services.Interfaces;
using System.Security.Claims;

namespace RecipeDormAPI.Application.CQRS.Handlers
{
    public class AddNewRecipeCommandHandler : IRequestHandler<AddNewRecipeCommand, BaseResponse<AddNewRecipeResponse>>
    {
        private readonly DataDbContext _dbContext;
        private readonly ILogger<AddNewRecipeCommandHandler> _logger;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileService _fileService;

        public AddNewRecipeCommandHandler(DataDbContext dbContext, ILogger<AddNewRecipeCommandHandler> logger, IOptions<AppSettings> appSettings, IFileService fileService, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _appSettings = appSettings.Value;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponse<AddNewRecipeResponse>> Handle(AddNewRecipeCommand request, CancellationToken cancellationToken)
        {

            if (_httpContextAccessor.HttpContext?.Items["UserId"] is not Guid userId)
            {
                _logger.LogError("UserId not found in HttpContext");
                return new BaseResponse<AddNewRecipeResponse>(false, "User authentication required");
            }

            //var userId = (Guid)_httpContextAccessor.HttpContext!.Items["UserId"];

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                //var userId = (Guid)_httpContextAccessor.HttpContext!.Items["UserId"];

                var newRecipe = new Recipes
                {
                    UserId = userId,
                    Title = request.Title,
                    ImageUrl = (request.Image != null) ? await _fileService.SaveImageAsync(request.Image) : null,
                    Ingredients = request.Ingredients.Select(i => new Ingredients
                    {
                        RecipeId = Guid.Empty,
                        Name = i.Name,
                        Quantity = i.Quantity,
                    }).ToList(),
                    Steps = request.Steps.Select(s => new Steps
                    {
                        RecipeId = Guid.Empty,
                        StepNumber = s.StepNumber,
                        Description = s.Description,
                    }).ToList()
                };

                newRecipe.Ingredients.ForEach(i => i.RecipeId = newRecipe.Id);
                newRecipe.Steps.ForEach(s => s.RecipeId = newRecipe.Id);

                await _dbContext.Recipes.AddAsync(newRecipe, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                var response = new AddNewRecipeResponse
                {
                    RecipeId = newRecipe.Id,
                    Title = newRecipe.Title,
                    ImageUrl = newRecipe.ImageUrl,
                    Ingredients = request.Ingredients,
                    Steps = request.Steps
                };
                
                _logger.LogInformation($"Recipe {newRecipe.Id} added successfully for user {userId}");
                return new BaseResponse<AddNewRecipeResponse>(true, "New recipe added successfully!", response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong\n{ex.StackTrace}: {ex.Message}");
                await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                return new BaseResponse<AddNewRecipeResponse>(false, _appSettings.ProcessingError);
            }
        }
    }
}