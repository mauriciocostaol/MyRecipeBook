using ComonTestUtilities;
using ComonTestUtilities.Entities;
using ComonTestUtilities.Mapper;
using ComonTestUtilities.Repositories;
using ComonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Update;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Test.Recipe.Update;

public class UpdateRecipeUseCaseTest
{
    [Fact]
    public async Task SuccessOnUpdateRecipe()
    {
        (var user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
        var request = RequestRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(user, recipe);

        Func<Task> act = async () => await useCase.Execute(recipe.Id, request);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ErrorOnUpdateRecipeWhenRecipeNotFound()
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => await useCase.Execute(recipeId: 1000, request);

        (await act.Should().ThrowAsync<NotFoundException>())
            .Where(e => e.Message.Equals(ResourceMessagesExceptions.RECIPE_NOT_FOUND));
    }

    [Fact]
    public async Task ErrorOnUpdateRecipeWhenRecipeTitleIsEmpty()
    {
        (var user, _) = UserBuilder.Build();
        var recipe = RecipeBuilder.Build(user);
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;

        var useCase = CreateUseCase(user, recipe);

        Func<Task> act = async () => await useCase.Execute(recipe.Id, request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1 &&
                e.GetErrorMessages().Contains(ResourceMessagesExceptions.RECIPE_TITLE_EMPTY));
    }

    private static UpdateRecipeUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user,
        MyRecipeBook.Domain.Entities.Recipe? recipe = null)
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var unitOfWork = UnitOfWorkBuilder.Build();
        var repository = new RecipeUpdateOnlyRepositoryBuilder().GetById(user, recipe).Build();

        return new UpdateRecipeUseCase(loggedUser, unitOfWork, mapper, repository);
    }
}

