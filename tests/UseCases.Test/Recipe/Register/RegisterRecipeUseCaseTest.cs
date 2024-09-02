using ComonTestUtilities;
using ComonTestUtilities.Entities;
using ComonTestUtilities.Mapper;
using ComonTestUtilities.Repositories;
using ComonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Test.Recipe.Register;

public class RegisterRecipeUseCaseTest
{
    [Fact]
    public async Task SuccessOnRegisterRecipe()
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestRecipeJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrWhiteSpace();
        result.Title.Should().Be(request.Title);
    }
    [Fact]
    public async Task ErrorOnRegisterRecipeWhenTitleIsEmpty()
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestRecipeJsonBuilder.Build();

        request.Title = string.Empty;
       
        var useCase = CreateUseCase(user);
       
        Func<Task> act = async () => { await useCase.Execute(request);};

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
        .Where(e=> e.GetErrorMessages().Count == 1 &&
        e.GetErrorMessages().Contains(ResourceMessagesExceptions.RECIPE_TITLE_EMPTY));
    }

    private static RegisterRecipeUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        var mapper = MapperBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var recipeWriteOnlyRepository = RecipeWriteOnlyRepositoryBuilder.Build();

        return new RegisterRecipeUseCase(recipeWriteOnlyRepository,loggedUser,unitOfWork,mapper);
    }
}
