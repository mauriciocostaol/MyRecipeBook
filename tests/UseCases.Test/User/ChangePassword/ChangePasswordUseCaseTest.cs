﻿using ComonTestUtilities;
using ComonTestUtilities.Cryptography;
using ComonTestUtilities.Entities;
using ComonTestUtilities.Repositories;
using ComonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Test.User.ChangePassword;
public class ChangePasswordUseCaseTest
{
    [Fact]
    public async Task SuccessOnChangePassword()
    {
        (var user, var password) = UserBuilder.Build();
       
        var request = RequestChangePasswordJsonBuilder.Build();
        request.Password = password;

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => await  useCase.Execute(request);

        await act.Should().NotThrowAsync();

        var passwordEncripter = PasswordEncripterBuilder.Build();

        user.Password.Should().Be(passwordEncripter.Encrypt(request.NewPassword));
    }

    [Fact]
    public async Task ErrorWhenNewPasswordIsEmpty()
    {
        (var user, var password) = UserBuilder.Build();
        var request = new RequestChangePasswordJson
        {
            Password = password,
            NewPassword = string.Empty
        };


        var useCase = CreateUseCase(user);

        Func<Task> act = async () => { await useCase.Execute(request); };

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1 && e.GetErrorMessages().Contains(ResourceMessagesExceptions.PASSWORD_EMPTY));

        var passwordEncripter = PasswordEncripterBuilder.Build();

        user.Password.Should().Be(passwordEncripter.Encrypt(password));

    }

    [Fact]
    public async Task ErrorWhenCurrentPasswordIsDifferent()
    {
        (var user, var password) = UserBuilder.Build();
        var request = RequestChangePasswordJsonBuilder.Build();
        

        var useCase = CreateUseCase(user);

        Func<Task> act = async () => { await useCase.Execute(request); };

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1 && e.GetErrorMessages().Contains(ResourceMessagesExceptions.PASSWORD_DIFFERENT_CURRENT_PASSWORD));

        var passwordEncripter = PasswordEncripterBuilder.Build();

        user.Password.Should().Be(passwordEncripter.Encrypt(password));

    }

    private static ChangePasswordUseCase CreateUseCase(MyRecipeBook.Domain.Entities.User user)
    {
        var unitOfWork = UnitOfWorkBuilder.Build();
        var userUpdateRepository = new UserUpdateOnlyRepositoryBuilder().GetById(user).Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var passwordEncripter = PasswordEncripterBuilder.Build();

        return new ChangePasswordUseCase(loggedUser, userUpdateRepository, unitOfWork, passwordEncripter);
    }
}
