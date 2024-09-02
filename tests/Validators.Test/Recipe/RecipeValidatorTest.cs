using ComonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe;
using MyRecipeBook.Communication.Enums;
using MyRecipeBook.Exceptions;

namespace Validators.Test.Recipe
{
    public class RecipeValidatorTest
    {
        [Fact]
        public void SuccessOnValidation()
        {
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ErrorOnValidationWhenInvalidCookingTime()
        {
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.CookingTime = (MyRecipeBook.Communication.Enums.CookingTime?)1000;
            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.COOKING_TIME_NOT_SUPPORTED));
        }
        [Fact]
        public void ErrorOnValidationWhenInvalidDifficulty()
        {
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Difficulty = (MyRecipeBook.Communication.Enums.Difficulty?)1000;
            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.DIFFICULTY_LEVEL_NOT_SUPPORTED));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("              ")]
        [InlineData("")]
        public void ErrorOnValidationWhenInvalidTitle(string title)
        {
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Title = title;
            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.RECIPE_TITLE_EMPTY));
        }

        [Fact]
        public void SuccessOnValidationWhenCookingTimeIsNull()
        {
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.CookingTime = null;
            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void SuccessOnValidationWhenDifficultyIsNull()
        {
            var validator = new RecipeValidator();
            var request = RequestRecipeJsonBuilder.Build();
            request.Difficulty = null;
            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }


        [Fact]
        public void SuccessWhenDishTypesIsEmpty()
        {
            var request = RequestRecipeJsonBuilder.Build();

            request.DishTypes.Clear();

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ErrorOnValidationWhenInvalidDishTypes()
        {
            var request = RequestRecipeJsonBuilder.Build();

            request.DishTypes.Add((DishType)1000);

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.DISH_TYPE_NOT_SUPPORTED));
        }

        [Fact]
        public void ErrorOnValidationWhenIngredientsAreEmpty()
        {
            var request = RequestRecipeJsonBuilder.Build();
            request.Ingredients.Clear();

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();

            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.AT_LEAST_ONE_INGREDIENT));

        }

        [Fact]
        public void ErrorOnValidationWhenInstructionsAreEmpty()
        {
            var request = RequestRecipeJsonBuilder.Build();

            request.Instructions.Clear();

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.AT_LEAST_ONE_INSTRUCTION));
        }


        [Theory]
        [InlineData("    ")]
        [InlineData("")]
        [InlineData(null)]
        public void ErrorWhenIngredientsValueAreEmpty(string ingredient)
        {
            var request = RequestRecipeJsonBuilder.Build();

            request.Ingredients.Add(ingredient);

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.INGREDIENT_EMPTY));

        }

        [Fact]
        public void ErrorOnValidationWhenSameStepInstructions()
        {
            var request = RequestRecipeJsonBuilder.Build();

            request.Instructions.First().Step = request.Instructions.Last().Step;

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.TWO_OR_MORE_INSTRUCTIONS_SAME_ORDER));
        }

        [Fact]
        public void ErrorOnValidationWhenNegativeStepInstructions()
        {
            var request = RequestRecipeJsonBuilder.Build();

            request.Instructions.First().Step = -1;

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.NON_NEGATIVE_INSTRUCTION_STEP));
        }

        [Theory]
        [InlineData("   ")]
        [InlineData("")]
        [InlineData(null)]
        public void ErrorOnValidationWhenEmptyValueInstructions(string instruction)
        {
            var request = RequestRecipeJsonBuilder.Build();

            request.Instructions.First().Text = instruction;

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.INSTRUCTION_EMPTY));
        }
        [Fact]
        public void ErrorWhenInstructionTooLong()
        {
            var request = RequestRecipeJsonBuilder.Build();

            request.Instructions.First().Text = RequestStringGenerator.Paragraphs(minCharacters: 2001);

            var validator = new RecipeValidator();

            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.INSTRUCTION_EXCEEDS_LIMIT_CHARACTERS));
        }

    }
}
