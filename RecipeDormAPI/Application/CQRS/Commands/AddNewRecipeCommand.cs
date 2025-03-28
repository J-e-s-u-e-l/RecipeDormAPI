using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeDormAPI.Infrastructure.Data.Models.DTOs;
using RecipeDormAPI.Infrastructure.Data.Models.Responses;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RecipeDormAPI.Application.CQRS.Commands
{
    public class AddNewRecipeCommand : IRequest<BaseResponse<AddNewRecipeResponse>>
    {
        [FromForm]
        public string Title { get; set; }
        [FromForm]
        public IFormFile? Image { get; set; }
        [FromForm]
        public string? Description { get; set; }
        [FromForm(Name = "ingredients")]
        /*public List<IngredientsDto> Ingredients { get; set; }
        [FromForm(Name = "steps")]
        public List<StepsDto> Steps { get; set; }*/
        public string IngredientsJson { get; set; }
        [FromForm(Name = "steps")]
        public string StepsJson { get; set; }

        [JsonIgnore]
        public List<IngredientsDto> Ingredients
        => string.IsNullOrEmpty(IngredientsJson)
            ? new List<IngredientsDto>()
            : JsonSerializer.Deserialize<List<IngredientsDto>>(IngredientsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        [JsonIgnore]
        public List<StepsDto> Steps
            => string.IsNullOrEmpty(StepsJson)
                ? new List<StepsDto>()
                : JsonSerializer.Deserialize<List<StepsDto>>(StepsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        public class AddNewRecipeCommandValidator : AbstractValidator<AddNewRecipeCommand>
        {
            private const int MaxFileSize = 5 * 1024 * 1024; // 5 MB

            public AddNewRecipeCommandValidator()
            {
                // Title validation
                RuleFor(x => x.Title)
                    .NotEmpty().WithMessage("Title is required")
                    .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

                // Image validation
                RuleFor(x => x.Image)
                    .Must(file => file.Length <= MaxFileSize)
                    .When(x => x.Image != null)
                    .WithMessage("File size must be less than 5 MB.")
                    .Must(file => IsValidImageType(file))
                    .When(x => x.Image != null)
                    .WithMessage("Invalid file type. Allowed types: .jpg, .png, jpeg.");

                // Steps validation
                RuleFor(x => x.Steps)
                    .NotNull().WithMessage("Steps list cannot be null")
                    .NotEmpty().WithMessage("Steps list is required")
                    .Must(s => s.Count > 0).WithMessage("At least one step is required")
                    .ForEach(step =>
                    {
                        step.ChildRules(child =>
                        {
                            child.RuleFor(x => x.StepNumber);
                                //.GreaterThanOrEqualTo(1).WithMessage("Step number must be greater than or equal to 1");

                            child.RuleFor(x => x.Description)
                                .NotEmpty().WithMessage("Step description is required")
                                .MaximumLength(500).WithMessage("Step description must not exceed 500 characters");
                        });
                    });

                // Ingredients validation
                RuleFor(x => x.Ingredients)
                    .NotNull().WithMessage("Ingredients list cannot be null")
                    .NotEmpty().WithMessage("Ingredients is required")
                    .Must(i => i.Count > 0).WithMessage("At least one ingredient is required")
                    .ForEach(ingredient =>
                    {
                        ingredient.ChildRules(child =>
                        {
                            child.RuleFor(x => x.Name)
                                .NotEmpty().WithMessage("Ingredient name is required");

                            child.RuleFor(x => x.Quantity)
                                .NotEmpty().WithMessage("Ingredient quantity is required");
                        });
                    });

                // Description validation
                RuleFor(x => x.Description)
                    .MaximumLength(200).WithMessage("Description must not exceed 500 characters");
            }

            private bool IsValidImageType(IFormFile file)
            {
                if (file == null) return false;

                var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                return allowedExtensions.Contains(fileExtension);
            }
        }
    }
}
