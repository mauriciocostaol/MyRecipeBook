using ComonTestUtilities;
using ComonTestUtilities.Entities;
using ComonTestUtilities.Repositories;
using ComonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Test.User.Update;
public class UpdateUserUseCaseTest
{
    [Fact]
    public async Task SuccessOnUpdateUserData()
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => await useCase.Execute(request);

        await act.Should().NotThrowAsync();

        user.Name.Should().Be(request.Name);
        user.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task ErrorOnUpdateUserWhenNameIsEmpty()
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = string.Empty;
        var useCase = CreateUseCase(user);

        Func<Task> act = async () => { await useCase.Execute(request); };

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
        .Where(e => e.GetErrorMessages().Count == 1 && e.GetErrorMessages().Contains(ResourceMessagesExceptions.NAME_EMPTY));

        user.Name.Should().NotBe(request.Name);
        user.Name.Should().NotBe(request.Email);


    }


    [Fact]
    public async Task ErrorOnUpdateUserWhenEmailIsEmpty()
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = string.Empty;
        var useCase = CreateUseCase(user);

        Func<Task> act = async () => { await useCase.Execute(request); };

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
        .Where(e => e.GetErrorMessages().Count == 1 && e.GetErrorMessages().Contains(ResourceMessagesExceptions.EMAIL_EMPTY));

        user.Name.Should().NotBe(request.Name);
        user.Name.Should().NotBe(request.Email);


    }

    [Fact]
    public async Task ErrorOnUpdateUserWhenEmailAlreadyRegistered()
    {

        (var user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = CreateUseCase(user, request.Email);

        Func<Task> act = async () => { await useCase.Execute(request); };

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
        .Where(e => e.GetErrorMessages().Count == 1 && e.GetErrorMessages().Contains(ResourceMessagesExceptions.EMAIL_ALREADY_REGISTERED));

        user.Name.Should().NotBe(request.Name);
        user.Name.Should().NotBe(request.Email);

    }
    
    private static UpdateUserUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, string? email = null)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var userUpdateOnlyRepository = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        var userReadOnlyRepositoryBuilder = new UserReadOnlyRepositoryBuilder();
        if (email.NotEmpty())
            userReadOnlyRepositoryBuilder.ExistActiveUserWithEmail(email!);

        return new UpdateUserUseCase(loggedUser, userUpdateOnlyRepository, userReadOnlyRepositoryBuilder.Build(), unitOfWork);
    }
}
