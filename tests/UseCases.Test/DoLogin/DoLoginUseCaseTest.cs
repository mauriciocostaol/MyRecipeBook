using ComonTestUtilities.Cryptography;
using ComonTestUtilities.Entities;
using ComonTestUtilities.Repositories;
using ComonTestUtilities.Requests;
using ComonTestUtilities.Tokens;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Login.DoLogin;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;


namespace UseCases.Test.DoLogin;
public class DoLoginUseCaseTest
{
    [Fact]
    public async Task SuccessOnDoLoginWithEmailAndPassword()
    {
        (var user,var password) = UserBuilder.Build();
        var useCase = CreateUseCase(user);

        var result = await useCase.Execute(new RequestLoginJson
        {
            Email = user.Email,
            Password = password
        });

        result.Should().NotBeNull();
        result.Tokens.Should().NotBeNull();
        result.Name.Should().NotBeNullOrWhiteSpace().And.Be(user.Name);
        result.Tokens.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ErrorOnDoLoginWhenInvalidUser()
    {
        var request = RequestLoginJsonBuilder.Build();
        var useCase = CreateUseCase();

        Func<Task> act = async () => { await useCase.Execute(request); };

        await act.Should().ThrowAsync<InvalidLoginException>()
            .Where(e => e.Message.Equals(ResourceMessagesExceptions.EMAIL_OR_PASSWORD_INVALID));
    }


    private static DoLoginUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User? user = null)
    {
        var passwordEncripter = PasswordEncripterBuilder.Build();
        var userReadOnlyRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
        var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();

        if (user is not null)
            userReadOnlyRepositoryBuilder.GetByEmailAndPassword(user);

        return new DoLoginUseCase(userReadOnlyRepositoryBuilder.Build(),passwordEncripter,accessTokenGenerator);
    }
}
