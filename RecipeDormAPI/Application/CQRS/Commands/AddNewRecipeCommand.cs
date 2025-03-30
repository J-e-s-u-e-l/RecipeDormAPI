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
        public string IngredientsJson { get; set; }

        [FromForm(Name = "steps")]
        public string StepsJson { get; set; }

        [JsonIgnore]
        public List<IngredientsDto> Ingredients =>
            TryDeserializeJson<IngredientsDto>(IngredientsJson, out var ingredients) ? ingredients : new List<IngredientsDto>();

        [JsonIgnore]
        public List<StepsDto> Steps =>
            TryDeserializeJson<StepsDto>(StepsJson, out var steps) ? steps : new List<StepsDto>();

        private bool TryDeserializeJson<T>(string json, out List<T> result)
        {
            result = new List<T>();
            if (string.IsNullOrEmpty(json)) return true; // Empty is valid, returns empty list

            try
            {
                result = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return true;
            }
            catch (JsonException)
            {
                return false; // Invalid JSON, return false
            }
        }
    }

    public class AddNewRecipeCommandValidator : AbstractValidator<AddNewRecipeCommand>
    {
        private const int MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public AddNewRecipeCommandValidator()
        {
            // Title validation
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

            // Description validation
            RuleFor(x => x.Description)
                .MaximumLength(200).WithMessage("Description must not exceed 200 characters");

            // Image validation
            RuleFor(x => x.Image)
                .Must(file => file.Length <= MaxFileSize)
                .When(x => x.Image != null)
                .WithMessage("File size must be less than 5 MB")
                .Must(file => IsValidImageType(file))
                .When(x => x.Image != null)
                .WithMessage("Invalid file type. Allowed types: .jpg, .png, .jpeg");

            // Validate IngredientsJson
            RuleFor(x => x.IngredientsJson)
                .NotNull().WithMessage("Ingredients JSON cannot be null")
                .NotEmpty().WithMessage("Ingredients JSON is required")
                .Must(BeValidJson<IngredientsDto>).WithMessage("Ingredients JSON is invalid or malformed");

            // Validate StepsJson
            RuleFor(x => x.StepsJson)
                .NotNull().WithMessage("Steps JSON cannot be null")
                .NotEmpty().WithMessage("Steps JSON is required")
                .Must(BeValidJson<StepsDto>).WithMessage("Steps JSON is invalid or malformed");

            // Validate deserialized Ingredients
            RuleFor(x => x.Ingredients)
                .NotNull().WithMessage("Ingredients list cannot be null")
                .NotEmpty().WithMessage("Ingredients list is required")
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

            // Validate deserialized Steps
            RuleFor(x => x.Steps)
                .NotNull().WithMessage("Steps list cannot be null")
                .NotEmpty().WithMessage("Steps list is required")
                .Must(s => s.Count > 0).WithMessage("At least one step is required")
                .Must(steps => IsStepSequenceValid(steps)).WithMessage("Step numbers must start from 1 and increase chronologically (e.g., 1, 2, 3, not 1, 2, 4)")
                .ForEach(step =>
                {
                    step.ChildRules(child =>
                    {
                        child.RuleFor(x => x.StepNumber)
                            .GreaterThanOrEqualTo(1).WithMessage("Step number must be greater than or equal to 1");

                        child.RuleFor(x => x.Description)
                            .NotEmpty().WithMessage("Step description is required")
                            .MaximumLength(500).WithMessage("Step description must not exceed 500 characters");
                    });
                });
        }

        private static bool IsValidImageType(IFormFile file)
        {
            if (file == null) return false;

            var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return allowedExtensions.Contains(fileExtension);
        }

        private static bool IsStepSequenceValid(List<StepsDto> steps)
        {
            if (steps == null || steps.Count == 0) return false;

            var stepNumbers = steps.Select(s => s.StepNumber).OrderBy(n => n).ToList();
            if (stepNumbers[0] != 1) return false; // Must start from 1

            for (int i = 1; i < stepNumbers.Count; i++)
            {
                if (stepNumbers[i] != stepNumbers[i - 1] + 1)
                {
                    return false; // Not sequential
                }
            }
            return true;
        }

        private static bool BeValidJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json)) return false;

            try
            {
                JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}