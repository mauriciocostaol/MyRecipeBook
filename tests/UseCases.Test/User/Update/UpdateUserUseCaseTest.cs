using ComonTestUtilities;
using ComonTestUtilities.Entities;
using ComonTestUtilities.Repositories;
using ComonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Update;
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
        .Where(e => e.ErrorMessages.Count == 1 && e.ErrorMessages.Contains(ResourceMessagesExceptions.NAME_EMPTY));

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
        .Where(e => e.ErrorMessages.Count == 1 && e.ErrorMessages.Contains(ResourceMessagesExceptions.EMAIL_EMPTY));

        user.Name.Should().NotBe(request.Name);
        user.Name.Should().NotBe(request.Email);


    }

    public async Task ErrorOnUpdateUserWhenEmailAlreadyRegistered()
    {

        (var user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = CreateUseCase(user, user.Email);

        Func<Task> act = async () => { await useCase.Execute(request); };

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
        .Where(e => e.ErrorMessages.Count == 1 && e.ErrorMessages.Contains(ResourceMessagesExceptions.EMAIL_ALREADY_REGISTERED));

        user.Name.Should().NotBe(request.Name);
        user.Name.Should().NotBe(request.Email);

    }
    
    private UpdateUserUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user, string? email = null)
    {
        var loggedUser = LoggedUserBuilder.Build(user);
        var userUpdateOnlyRepository = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
        var userReadOnlyRepository = new UserReadOnlyRepositoryBuilder();
        var unitOfWork = UnitOfWorkBuilder.Build();



        return new UpdateUserUseCase(loggedUser, userUpdateOnlyRepository, userReadOnlyRepository.Build(), unitOfWork);
    }
}
