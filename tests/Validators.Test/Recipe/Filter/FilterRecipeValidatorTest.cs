using ComonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Exceptions;

namespace Validators.Test.Recipe.Filter;

public class FilterRecipeValidatorTest
{
    [Fact]
    public void SuccessOnValidationToFilterRecipe()
    {
        var validator = new FilterRecipeValidator();

        var request = RequestFilterRecipeJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();

    }

    [Fact]
    public void ErrorOnValidationToFilterRecipeWhenInvalidCookingTime()
    {
        var validator = new FilterRecipeValidator();

        var request = RequestFilterRecipeJsonBuilder.Build();
        request.CookingTimes.Add((MyRecipeBook.Communication.Enums.CookingTime)1000);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.COOKING_TIME_NOT_SUPPORTED));
    }


    [Fact]
    public void ErrorOnValidationToFilterRecipeWhenInvalidDifficulty()
    {
        var validator = new FilterRecipeValidator();

        var request = RequestFilterRecipeJsonBuilder.Build();
        request.Difficulties.Add((MyRecipeBook.Communication.Enums.Difficulty)1000);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.DIFFICULTY_LEVEL_NOT_SUPPORTED));
    }

    [Fact]
    public void ErrorOnValidationToFilterRecipeWhenInvalidDishtype()
    {
        var validator = new FilterRecipeValidator();

        var request = RequestFilterRecipeJsonBuilder.Build();
        request.DishTypes.Add((MyRecipeBook.Communication.Enums.DishType)1000);

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.DISH_TYPE_NOT_SUPPORTED));
    }

    

}
